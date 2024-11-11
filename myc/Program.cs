// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// program file path required as argument
if(args.Length == 0) {
    Console.WriteLine("\nProgram File path missing from arguments\n");
    return -1;
}

string programFile = args[0];
Console.WriteLine("\nProgram File: {0}\n", programFile);

return 0;