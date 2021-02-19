using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace HelloGenerator
{
    [Generator]
    public class LinkedInLearning : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var code = @"
                public static class LinkedInLearning {
                    public static void Hello() => 
                       System.Console.WriteLine(""Hola LinkedIn Learning"");
                }
            ";

            context.AddSource("linked.learning.generator",
            SourceText.From(code, Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
