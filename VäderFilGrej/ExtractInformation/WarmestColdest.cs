using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VäderFilGrej.ExtractInformation
{
    internal class WarmestColdest
    {
        public static void WarmCold()
        {
            string[] lines = File.ReadAllLines(@"C:\Users\n01re\Source\Repos\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");

            string pattern = @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\s(?<time>\d{2}:\d{2}:\d{2}),(?<plats>\w+),(?<temp>\d+\.\d+),(?<humidity>\d+)";

            Dictionary<string, List<double>> tempPerDay = new Dictionary<string, List<double>>();

            foreach (string line in lines)
            {
                Match temp = Regex.Match(line, pattern);

                if (temp.Success && temp.Groups["plats"].ToString() == "Ute")
                {
                    if (temp.Groups["month"].ToString() == "05" || temp.Groups["month"].ToString() == "01")
                    {

                    }
                    else
                    {
                        string yearMonth = $"{temp.Groups["year"].Value}-{temp.Groups["month"].Value}-{temp.Groups["day"].Value}";

                        double tempCheck = double.Parse(temp.Groups["temp"].Value, CultureInfo.InvariantCulture);

                        if (!tempPerDay.ContainsKey(yearMonth))
                        {
                            tempPerDay[yearMonth] = new List<double>();
                        }
                        tempPerDay[yearMonth].Add(tempCheck);

                    }
                }

                
            }

            var groupedSum = tempPerDay
            .GroupBy(x => x.Key) // Gruppera efter nyckeln
            .Select(group => new
            {
                Key = group.Key,
                Sum = group.Sum(x => x.Value.Sum() / x.Value.Count())
                // Summera värdena för varje grupp
            })
            .OrderByDescending(x => x.Sum);

            
            foreach (var temp in groupedSum)
            {
                Console.WriteLine($"Månad:{temp.Key}, Temperaturer: {temp.Sum.ToString("F2")}");
            }
            
        }
    }
}
