namespace HashBenchmark
{
    using System;
    using System.IO;

    public static class InputFile
    {
        private static string inputFileContent = string.Empty;
        public static string defaultInputFilePath = @"./input.txt";

        public static string Content
        {
            get
            {
                return inputFileContent;
            }
        }

        public static void Load(string path)
        {
            using (var f = File.Open(path, FileMode.Open))
            {
                var r = new StreamReader(f);
                inputFileContent = r.ReadToEnd();
            }
        }
    }
}
