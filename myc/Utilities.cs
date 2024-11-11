using System.Diagnostics;

namespace myc
{
    public static class Utilities
    {
        internal static void RunGCCPreprocessor(string fileName)
        {
            string cmd = string.Format("-E -P {0} -o {1}.prep", fileName, fileName.Split('.')[0]);
            RunGccCommand(cmd);
        }

        internal static void CleanUpGCCPreprocessor(string filePath)
        {
            string prepFilePath = filePath.Split('.')[0] + ".prep";
            DeleteFile(prepFilePath);
        }

        internal static void GCCCompile(string filePath)
        {
            string cmd = string.Format("-S -O -fno-asynchronous-unwind-tables -fcf-protection=none {0}", filePath);
            RunGccCommand(cmd);
        }

        internal static void AssembleAndLink(string fileName)
        {
            string cmd = string.Format("{0}.s -o {1}", fileName.Split('.')[0], fileName.Split('.')[0]);
            RunGccCommand(cmd);
        }
        internal static void CleanUpAssembleAndLink(string filePath)
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

    }
}