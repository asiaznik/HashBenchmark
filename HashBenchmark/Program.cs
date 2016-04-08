using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using App.Security.Cryptography;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Data.HashFunction;

namespace HashBenchmark
{
    public class Program
    {
        private static int iterations = 100000;
        
        public enum Algorithm : int
        {
            MD5 = 1,
            CRC32,
            ELF32,
            HMAC,
            SHA1,
            KeyedHashAlgorithm,
            RIPEMD160
        }

        public static void Main(string[] args)
        {
            Terminal.Log("Hashbenchmark Mark I");
            
            Terminal.Log("Loading input file...");
            try
            {
                InputFile.Load(InputFile.defaultInputFilePath);
            }
            catch (FileNotFoundException e)
            {
                Terminal.Error(e.Message);
                return;
            }

            Terminal.Success("Input file loaded");
            Terminal.Info(string.Format("Length: {0}", InputFile.Content.Length));

            // xxHash algorithm test
            RunXxHashBenchmark();

            var algorithms = Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>();
            foreach (var i in algorithms)
            {
                RunBenchmark(i);
            }
        }

        private static string ComputeHash(string input, HashAlgorithm ha, out TimeSpan elapsed)
        {
            var stopwatch = Stopwatch.StartNew();
            string hash = ComputeHash(input, ha);
            stopwatch.Stop();
            elapsed = stopwatch.Elapsed;
            return hash;
        }

        private static string ComputeHash(string input, HashAlgorithm ha)
        {
            string output = string.Empty;
            byte[] data = Encoding.Default.GetBytes(input);
            try
            {
                byte[] rawHash = ha.ComputeHash(data);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < rawHash.Length; i++)
                {
                    sb.Append(rawHash[i].ToString("x2"));
                }

                output = sb.ToString();
            }
            finally
            {
                ha.Dispose();
            }

            return output;
        }

        private static double[] ComputeAndMeasureTask(int iterations, string input, Algorithm alg)
        {
            double[] times = new double[iterations];
            using (var p = new ProgressBar())
            {

                for (var i = 0; i < iterations; i++)
                {
                    HashAlgorithm a = GetHashAlgorithm(alg);
                    TimeSpan time;
                    string hash = ComputeHash(input, a, out time);
                    times[i] = time.TotalMilliseconds;
                    p.Report((double)(i + 1) / iterations);
                }

                Terminal.CleanLine();
                Terminal.Success("Done.");
            }

            return times;
        }

        private static HashAlgorithm GetHashAlgorithm(Algorithm alg)
        {
            HashAlgorithm a;
            switch (alg)
            {
                case Algorithm.CRC32:
                    a = Crc32.Create();
                    break;
                case Algorithm.ELF32:
                    a = Elf32.Create();
                    break;
                case Algorithm.HMAC:
                    a = HMAC.Create();
                    break;
                case Algorithm.SHA1:
                    a = SHA1.Create();
                    break;
                case Algorithm.KeyedHashAlgorithm:
                    a = KeyedHashAlgorithm.Create();
                    break;
                case Algorithm.RIPEMD160:
                    a = RIPEMD160.Create();
                    break;
                default:
                case Algorithm.MD5:
                    a = MD5.Create();
                    break;
            }

            return a;
        }

        private static void GenerateReport(Algorithm alg, double[] times)
        {
            double sum = times.Sum();
            double totalTimeInSeconds = sum / 1000;
            double rps = iterations / totalTimeInSeconds;
            double avg = times.Sum() / iterations;
            Terminal.Info(string.Format("{0}: avg time for {1} iterations: {2} ms (total {3} s, RPS: {4})", alg, iterations.ToString(), avg.ToString(), totalTimeInSeconds.ToString(), rps.ToString()));
        }

        private static void RunBenchmark(Algorithm alg)
        {
            string hash = ComputeHash(InputFile.Content, GetHashAlgorithm(alg));
            Terminal.Success(string.Format("{0}: {1}", alg, hash));
            Terminal.Log(string.Format("Computing hash: {0} (x{1})", alg, iterations));
            var times = ComputeAndMeasureTask(iterations, InputFile.Content, alg);
            GenerateReport(alg, times);
        }

        private static void RunXxHashBenchmark()
        {
            TimeSpan t;
            string hash = ComputeXxHash(InputFile.Content, out t);
            Terminal.Success(string.Format("{0}: {1}", "xxHash", hash));

            Terminal.Log(string.Format("Computing hash: {0} (x{1})", "xxHash", iterations));
            double[] times = new double[iterations];
            using (var p = new ProgressBar())
            {

                for (var i = 0; i < iterations; i++)
                {
                    TimeSpan time;
                    ComputeXxHash(InputFile.Content, out time);
                    times[i] = time.TotalMilliseconds;
                    p.Report((double)(i + 1) / iterations);
                }

                Terminal.CleanLine();
                Terminal.Success("Done.");
            }

            double sum = times.Sum();
            double totalTimeInSeconds = sum / 1000;
            double rps = iterations / totalTimeInSeconds;
            double avg = times.Sum() / iterations;
            Terminal.Info(string.Format("{0}: avg time for {1} iterations: {2} ms (total {3} s, RPS: {4})", "xxHash", iterations.ToString(), avg.ToString(), totalTimeInSeconds.ToString(), rps.ToString()));
        }

        private static string ComputeXxHash(string input, out TimeSpan time)
        {
            var s = Stopwatch.StartNew();
            string output = string.Empty;
            byte[] data = Encoding.Default.GetBytes(input);
            var xx = new xxHash(32);
            byte[] rawHash = xx.ComputeHash<byte[]>(data);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rawHash.Length; i++)
            {
                sb.Append(rawHash[i].ToString("x2"));
            }

            output = sb.ToString();
            s.Stop();
            time = s.Elapsed;

            return output;
        }
    }
}
