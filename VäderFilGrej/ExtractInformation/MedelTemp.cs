using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VäderFilGrej.ExtractInformation
{
    internal class MedelTemp
    {
        public static void ExtractMedelTemp() 
        {
            //string[] lines = File.ReadAllLines(@"C:\Users\noelb\Desktop\System24\Filer\tempdata5-medfel.txt");

            string[] lines = File.ReadAllLines(@"C:\Users\Johan\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");

            string pattern = @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\s(?<time>\d{2}:\d{2}:\d{2}),(?<plats>\w+),(?<temp>\d+\.\d+),(?<humidity>\d+)";

            Dictionary<string, (List<double> tempList, List<double> humidityList)> tempPerMonth = new Dictionary<string, (List<double>, List<double>)>();

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

                        double humidityCheck = double.Parse(temp.Groups["humidity"].Value);

                        if (!tempPerMonth.ContainsKey(yearMonth))
                        {
                            tempPerMonth[yearMonth] = (new List<double>(), new List<double>());
                        }
                        tempPerMonth[yearMonth].tempList.Add(tempCheck);
                        tempPerMonth[yearMonth].humidityList.Add(humidityCheck);
                    }
                }

            }

            foreach(var entry in tempPerMonth)
            {
               Console.WriteLine($"Månad:{entry.Key}, Temperaturer: {(entry.Value.tempList.Sum() / entry.Value.tempList.Count).ToString("F2")}" +
                   $" Luftfuktighet: {(entry.Value.humidityList.Sum() / entry.Value.humidityList.Count).ToString("F2")}");

                
                var hum = (entry.Value.humidityList.Sum() / entry.Value.humidityList.Count);
                var temp = (entry.Value.tempList.Sum() / entry.Value.tempList.Count); 

                
                Console.WriteLine($"Mold: {CalculateMoldRisk(temp, hum).ToString("F2")}");

            }
            
        }
        public static double CalculateMoldRisk(double temperature, double humidity)
        {
            if (humidity < 78) return 0;
            if (temperature < 0) return 0;

            double risk = (humidity - 78) * (temperature / 15) / 0.22;
            return risk;
        }

    }
}
