namespace myc
{
    public class MyCompiler(Settings settings)
    {
        Settings mySettings = settings;

        public void Compile(Settings settings)
        {
            mySettings = settings;

            Console.WriteLine("\nProgram File: {0}\n", mySettings.ProgramFile);


            Console.WriteLine("Preprocess");
            string prepFilePath = Utilities.RunGCCPreprocessor(mySettings.ProgramFile);

            //Compile
            Console.WriteLine("Compile");
            //Utilities.GCCCompile(programFile);
            Compile(prepFilePath);

            //always cleanup preprocessor file
            Utilities.GCCPreprocessorCleanUp(mySettings.ProgramFile);

            //Assemble/Link only if no cmd line parms
            if (mySettings.RunAssembleLink && !settings.DebugMode)
            {
                //Assemble and Link
                Console.WriteLine("Assemble and Link {0}", mySettings.ProgramFile);
                Utilities.AssembleAndLink(mySettings.ProgramFile);

                //cleanup asm file
                Utilities.AssembleAndLinkCleanUp(mySettings.ProgramFile);
            }

            Console.WriteLine("\nComplete\n");
        }

        internal void Compile(string programFile)
        {
            string? source = ReadFile(programFile);
            string tokens;

            if (source != null)
            {
                tokens = Lex.Process(source);

                Console.WriteLine("\n{0}", tokens);

                //If only running Lexer then step out now
                if (mySettings.RunThruLexer) return;

                AST_Program ast = Parse.Process(tokens);
                Console.WriteLine("\n{0}",ast.Print());

                // If Only running Lexer and Parser then step out now
                if (mySettings.RunThruParser) return;

                AST_Program validAST = SEM_Resolve.Resolve(ast);
                Console.WriteLine("\n{0}",ast.Print());

                // If Only running Validation and Parser then step out now
                if (mySettings.RunThruValidate) return;

                TAC_Program tac_ast = TACGen.Generate(validAST);
                Console.WriteLine("\n{0}",tac_ast.Print());

                // If Only running Lexer, Parser, CodeGen and TAC then step out now
                if (mySettings.RunThruTac) return;

                // Assembly generation has three steps
                // 1. Convert TACto assembly 
                ASM_Program asm_ast = CodeGen.Generate(tac_ast);
                // 2. Replace pseudo registers with stack operands
                int lastStackSlot = ReplacePseudos.Replace(asm_ast);
                // 3. Fixup instructions
                FixInstructions.Fix(asm_ast, lastStackSlot);

                // Console.WriteLine("\n{0}",asm_ast);

                // If Only running Lexer, Parser and CodeGen then step out now
                if (mySettings.RunThruCodeGen) return;

                CodeEmit.Emit(asm_ast, programFile);
            }
        }

        internal static string? ReadFile(string filePath)
        {
            try
            {
                // Open the text file using a stream reader.
                using StreamReader reader = new(filePath);

                // Read the stream as a string.
                string source = reader.ReadToEnd().TrimStart();

                // Write the text to the console.
                //Console.WriteLine(source);

                return source;
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read: {0}", filePath);
                Console.WriteLine(e.Message);

                return null;
            }

        }

    }

}