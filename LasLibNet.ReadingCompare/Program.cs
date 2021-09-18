using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.ReadingCompare
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(" Starting test ...");

            LasReader reader = new LasReader();
            if (reader.OpenReader(@"d:\temp\！shcg.las") == false)
            {
                Console.WriteLine("Read file failed : " + reader.Error);
                Console.ReadKey();
                return;
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 10; i++)
            {
                reader.SeekPoint(0);
                // Loop through number of points indicated
                for (int pointIndex = 0; pointIndex < reader.Header.number_of_point_records; pointIndex++)
                {
                    reader.ReadPoint2();
                }
            }
            sw.Stop();
            Console.WriteLine("  C# Traditional reading method elapsed time avg : {0} ms", sw.Elapsed.TotalMilliseconds / 10);

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            for (int i = 0; i < 10; i++)
            {
                reader.SeekPoint(0);
                // Loop through number of points indicated
                for (int pointIndex = 0; pointIndex < reader.Header.number_of_point_records; pointIndex++)
                {
                    reader.ReadPoint();
                }
            }
            sw2.Stop();
            Console.WriteLine("  Time elapsed avg : {0} ms", sw2.Elapsed.TotalMilliseconds / 10);

            Console.ReadKey();
        }
    }
}
