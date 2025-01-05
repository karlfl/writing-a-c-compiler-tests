using System.Diagnostics;

namespace myc
{
    public static class Utilities
    {
        static int UniqueIdCounter = 0;
        internal static string RunGCCPreprocessor(string filePath)
        {
            string cmd = string.Format("-E -P {0} -o {1}.prep", filePath, filePath.Split('.')[0]);
            RunGccCommand(cmd);
            return string.Format("{0}.prep",filePath.Split('.')[0]);
        }

        internal static void GCCPreprocessorCleanUp(string filePath)
        {
            string prepFilePath = filePath.Split('.')[0] + ".prep";
            DeleteFile(prepFilePath);
        }

        internal static void GCCCompile(string filePath)
        {
            string cmd = string.Format("-S -O -fno-asynchronous-unwind-tables -fcf-protection=none {0}", filePath);
            RunGccCommand(cmd);
        }

        internal static void AssembleAndLink(string filePath)
        {
            string cmd = string.Format("{0}.s -o {1}", filePath.Split('.')[0], filePath.Split('.')[0]);
            RunGccCommand(cmd);
        }
        internal static void AssembleAndLinkCleanUp(string filePath)
        {
            string prepFilePath = filePath.Split('.')[0] + ".s";
            DeleteFile(prepFilePath);
        }



        private static void RunGccCommand(string command)
        {
            // Console.WriteLine("Executing command: gcc {0}", command);
            // Create a new process
            Process process = new();
            process.StartInfo.FileName = "gcc";
            process.StartInfo.Arguments = command;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Start the process
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Console.WriteLine(output);
        }

        private static void DeleteFile(string prepFilePath)
        {
            try
            {
                if (File.Exists(prepFilePath))
                {
                    File.Delete(prepFilePath);
                    //Console.WriteLine("File deleted successfully.");
                }
                else
                {
                    //Console.WriteLine("File not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static string GenerateUniqueId(){
            //* Note: including "." ensures that this label won't conflict
            //* with real function or variable names in the symbol table,
            //* when we start tracking symbols in later chapters
            return string.Format("tmp.{0}", UniqueIdCounter++);
        }

        public static string GenerateUniqueLabel(string prefix){
            return string.Format("{0}.{1}",prefix, UniqueIdCounter++);
        }
    }
}