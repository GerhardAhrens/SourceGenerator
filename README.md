# Source Generator auf Basis von IIncrementalGenerator

![NET](https://img.shields.io/badge/NET-10.0-green.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![VS2026](https://img.shields.io/badge/Visual%20Studio-2026-white.svg)
![Version](https://img.shields.io/badge/Version-1.0.2026.0-yellow.svg)]

Das rudimentäre Beispiel eines Source Generators, der das Interface *IIncrementalGenerator* implementiert. Dieser Generator fügt jedem Projekt, in dem er eingebunden ist, eine einfache Klasse zur Laufzeit hinzu.

Der **Roslyn** eigene Source Generator wird in .NET 10 und Visual Studio 2026 unterstützt. Hiermit wird ermöglicht, Klassen Zur Laufzeit zu erstellen. Leider ist diese Methode keinen Ersatz für den im Classic Framework enthalteten T4 Template Generator. Erher ist das als Trickkiste zu sehen, um Sourcen nachträglich in eine Anwendung einzufügen, analog zu *PostSharp*.

Die Anwedung des Source Generator ist auf den ersten Blick relativ einfach, da der Generator nur in einem eigenen Projekt eingebunden werden muss. Allerdings sind die Debugging-Möglichkeiten noch sehr eingeschränkt, so dass es sich empfiehlt, den Source Generator in einer eigenen Lösung zu entwickeln und dort zu testen. \
Bei komplexeren Generatoren empfiehlt es sich, die Generierung in einzelne Schritte zu unterteilen und diese dann nacheinander zu testen.</br>

Allerdings kann man schnell an die Grenzen des Source Generators stoßen, wenn z.B. auf Variabeln oder andere Klassen zugegriffen werden soll,.


# Beispielsource zum Generator Teil
```csharp
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
```

# Verwendung des generierten Source
```csharp
Console.WriteLine(HelloGenerated.SayHello());
```
