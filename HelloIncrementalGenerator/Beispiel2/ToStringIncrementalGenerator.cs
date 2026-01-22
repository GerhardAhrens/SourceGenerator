namespace HelloIncrementalGenerator
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    using System.Linq;
    using System.Text;

    [Generator]
    public sealed class ToStringIncrementalGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 1️⃣ Attribut automatisch erzeugen
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource(
                    "GenerateToStringAttribute.g.cs",
                    """
            namespace GeneratorDemo;

            [System.AttributeUsage(System.AttributeTargets.Class)]
            public sealed class GenerateToStringAttribute : System.Attribute
            {
            }
            """
                );
            });

            // 2️⃣ Klassen mit Attribut filtern
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) =>
                        node is ClassDeclarationSyntax cds &&
                        cds.AttributeLists.Count > 0,
                    transform: static (ctx, _) => GetSemanticTarget(ctx))
                .Where(static c => c != null)!;

            // 3️⃣ Combine Compilation + Klasse (kein LINQ, nur Roslyn API)
            var compilationAndClasses = classDeclarations
                .Combine(context.CompilationProvider);

            // 4️⃣ Source Output
            context.RegisterSourceOutput(compilationAndClasses, (spc, item) =>
            {
                var classDecl = item.Left;
                var compilation = item.Right;

                Execute(compilation, classDecl, spc);
            });
        }

        // 🔍 Semantische Prüfung
        private static ClassDeclarationSyntax? GetSemanticTarget(GeneratorSyntaxContext context)
        {
            var classDecl = (ClassDeclarationSyntax)context.Node;

            foreach (var attrList in classDecl.AttributeLists)
                foreach (var attr in attrList.Attributes)
                {
                    var symbol = context.SemanticModel.GetSymbolInfo(attr).Symbol;
                    if (symbol?.ContainingType.ToDisplayString() == "GeneratorDemo.GenerateToStringAttribute")
                    {
                        return classDecl;
                    }
                }

            return null;
        }

        // 🧠 Hauptlogik
        private static void Execute(Compilation compilation, ClassDeclarationSyntax classDecl, SourceProductionContext context)
        {
            /*
            if (System.Diagnostics.Debugger.IsAttached == false)
            {
                System.Diagnostics.Debugger.Launch();
            }
            */

            // Semantic Model für die Klasse
            var model = compilation.GetSemanticModel(classDecl.SyntaxTree);
            var classSymbol = model.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;

            if (classSymbol == null) return;

            // Klasse muss partial sein
            if (!classDecl.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "GEN001",
                        "Klasse muss partial sein",
                        $"Die Klasse '{classSymbol.Name}' muss partial sein",
                        "Generator",
                        DiagnosticSeverity.Error,
                        true),
                    classDecl.Identifier.GetLocation()));
                return;
            }

            // Alle öffentlichen Properties
            var properties = classSymbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.DeclaredAccessibility == Accessibility.Public).ToArray();
            string names = string.Join(";", properties.Select(p => p.Name));

            // StringBuilder für generierten Code
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {classSymbol.ContainingNamespace.ToDisplayString()};");
            sb.AppendLine();
            sb.AppendLine($"public partial class {classSymbol.Name}");
            sb.AppendLine("{");
            sb.AppendLine("    public override string ToString()");
            sb.AppendLine("    {");
            sb.AppendLine($"        return \"{names}\";");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            // Quelle hinzufügen
            context.AddSource($"{classSymbol.Name}.ToString.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
