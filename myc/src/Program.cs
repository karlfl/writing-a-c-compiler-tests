// See https://aka.ms/new-console-template for more information
using myc;

internal class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        Console.WriteLine("\nArguments:");
        foreach (string arg in args)
        {
            Console.WriteLine("\t{0}", arg);
        }

        // program file path required as argument
        if (args.Length == 0)
        {
            Console.WriteLine("\nProgram file path missing from arguments\n");
            return -1;
        }

        // Check for arg flags
        // --codeGen = Run Lexer, Parser and Assembly Generator
        // --parser = Run Lexer and Parser
        // --lex = Run lexer
        bool runThruCodeGen = args.Contains("--codeGen");
        bool runThruParser  = args.Contains("--parse")    || runThruCodeGen;
        bool runThruLexer   = args.Contains("--lex")      || runThruCodeGen || runThruParser;

        string programFile = args[^1];

        try
        {
            MyCompiler.Compile(programFile, runThruLexer, runThruParser, runThruCodeGen);           
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("Error Found: {0}",ex.Message);
            return -1;
        }


        return 0;

    }
}