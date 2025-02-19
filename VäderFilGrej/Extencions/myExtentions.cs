using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VäderFilGrej.ExtractInformation;

namespace VäderFilGrej.Extencions
{
    internal static class myExtentions
    {
        public static void CW(this string input)
        {
            Console.WriteLine(input);
        }


        
        public static void Search(this string input)
        {
            var data = new GetData();
            var list = data.TempList(false, null);

            foreach (var entry in list)
            {
                if (entry.Key == input)
                {
                    Console.WriteLine($"Månad:{entry.Key}, Temperaturer: {(entry.Value.temp.Sum() / entry.Value.temp.Count).ToString("F2")}" +
                   $" Luftfuktighet: {(entry.Value.humidity.Sum() / entry.Value.humidity.Count).ToString("F2")}");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"Ingen information hittades på datumet: {input}");
                }
            }

        }

    }
}
