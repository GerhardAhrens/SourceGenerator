namespace HelloIncrementalGenerator
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    using System.Text;

    [Generator]
    public sealed class FromGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            /*
            if (System.Diagnostics.Debugger.IsAttached == false)
            {
                System.Diagnostics.Debugger.Launch();
            }
            */

            // Fester Output, wird nur einmal erzeugt (Static Source)
            context.RegisterPostInitializationOutput(ctx =>
            {
                var source = """
            namespace Generated;

            public static class HelloGenerated
            {
                public static string SayHello()
                    => "Hallo aus dem Incremental Source Generator (NET 10)!";
            }
            """;

                ctx.AddSource("HelloGenerated.g.cs", SourceText.From(source, Encoding.UTF8));
            });
        }
    }
}
