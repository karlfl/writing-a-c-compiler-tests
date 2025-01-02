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
        // --codeGen = Run Lexer, Parser, Tacky and Assembly Generator
        // --tacky = Run lexer, parser, Tacky
        // --parser = Run Lexer and Parser
        // --lex = Run lexer
        bool runThruCodeGen = args.Contains("--codegen");
        bool runThruTac     = args.Contains("--tacky");
        bool runThruParser  = args.Contains("--parse");
        bool runThruLexer   = args.Contains("--lex");

        string programFile = args[^1];

        try
        {
            Settings settings = new (programFile, runThruCodeGen, runThruParser, runThruLexer, runThruTac);
            MyCompiler compiler = new(settings);
            compiler.Compile(settings);           
        }

        catch (System.Exception ex)
        {
            Console.WriteLine("Error Found: {0}",ex.Message);
            return -1;
        }

        return 0;

    }
}