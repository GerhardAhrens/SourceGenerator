//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Lifeprojects.de">
//     Class: Program
//     Copyright © Lifeprojects.de 2026
// </copyright>
// <Template>
// 	Version 3.0.2026.1, 08.1.2026
// </Template>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>21.01.2026 15:53:11</date>
//
// <summary>
// Konsolen Applikation mit Menü
// </summary>
//-----------------------------------------------------------------------

namespace SourceGenerator
{
    /* Imports from NET Framework */
    using System;

    using Generated;

    using GeneratorDemo;

    public class Program
    {
        private static void Main(string[] args)
        {
            ConsoleMenu.Add("1", "Einfaches Beispiel IIncrementalGenerator", () => MenuPoint1());
            ConsoleMenu.Add("2", "Komplexes Beispiel IIncrementalGenerator", () => MenuPoint2());
            ConsoleMenu.Add("X", "Beenden", () => ApplicationExit());

            do
            {
                _ = ConsoleMenu.SelectKey(2, 2);
            }
            while (true);
        }

        private static void ApplicationExit()
        {
            Environment.Exit(0);
        }

        private static void MenuPoint1()
        {
            Console.Clear();

            Console.WriteLine(HelloGenerated.SayHello());

            ConsoleMenu.Wait();
        }

        private static void MenuPoint2()
        {
            Console.Clear();

            var p = new Person { Name = "Charlie", Age = 3 };
            Console.WriteLine(p);
            ConsoleMenu.Wait();
        }
    }

    [GenerateToString]
    public partial class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
