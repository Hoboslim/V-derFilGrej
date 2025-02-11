using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VäderFilGrej.ExtractInformation
{
    internal class GetData
    {
        public Dictionary<string, (List<double> temp, List<double> humidity)> TempList()
        {
            string[] lines = File.ReadAllLines(@"C:\Users\n01re\Source\Repos\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");

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
            return tempPerMonth;
        }
        public void Mold()
        {
            double risk;
            Dictionary<string, double> mold = new Dictionary<string, double>(); 

            var tempPerday = TempList();
            foreach (var entry in tempPerday)
            {
                var hum = (entry.Value.humidity.Sum() / entry.Value.humidity.Count);
                var temp = (entry.Value.temp.Sum() / entry.Value.temp.Count);

                if (hum >= 78 && temp >= 0)
                {
                    risk = (hum - 78) * (temp / 15) / 0.22;
                }
                else
                {
                    risk = 0;
                }

                mold.Add(entry.Key, risk);
            }


            foreach (var entry in mold.OrderBy(entry => entry.Value))
            {
                if (entry.Value > 0)
                {
                    Console.WriteLine($"Dag {entry.Key} risk: {entry.Value.ToString("F2")}");
                }
            }
        }
        public void DryWet()
        {
            var tempDay = TempList();

            var groupedSum = tempDay
                .GroupBy(x => x.Key)
                .Select(group => new
                {
                    Key = group.Key,
                    Sum = group.Sum(x => x.Value.humidity.Sum() / x.Value.humidity.Count())
                })
                .OrderBy(x => x.Sum);
            foreach (var group in groupedSum)
            {
                Console.WriteLine($"Månad:{group.Key}, Fuktighet: {group.Sum.ToString("F2")}");
            }

        }
        public void WarmCold()
        {
            var tempPerDay = TempList();

                var groupedSum = tempPerDay
                .GroupBy(x => x.Key)
                .Select(group => new
                {
                    Key = group.Key,
                    Sum = group.Sum(x => x.Value.temp.Sum() / x.Value.temp.Count())
                })
                .OrderByDescending(x => x.Sum);
            foreach (var temp in groupedSum)
            {
                Console.WriteLine($"Månad:{temp.Key}, Temperaturer: {temp.Sum.ToString("F2")}");
            }
        }
        public void MedTemp()
        {
            var tempPerMonth = TempList();

            foreach(var entry in tempPerMonth)
            {
               Console.WriteLine($"Månad:{entry.Key}, Temperaturer: {(entry.Value.temp.Sum() / entry.Value.temp.Count).ToString("F2")}" +
                   $" Luftfuktighet: {(entry.Value.humidity.Sum() / entry.Value.humidity.Count).ToString("F2")}");

                
                var hum = (entry.Value.humidity.Sum() / entry.Value.humidity.Count);
                
            }
        }

        public void Fall()
        {
            var tempPerWeek = TempList();
            int daysInRow = 0;
           
            string day1 = "ingenDag";
            

            foreach (var entry in tempPerWeek)
            {
                var temp = (entry.Value.temp.Sum() / entry.Value.temp.Count);


                if (temp < 10)
                {
                    if (daysInRow == 0)
                    {
                        day1 = entry.Key.ToString();
                    }

                    daysInRow++;

                    if (daysInRow == 5)
                    {
                        Console.WriteLine("hösten börjar den: " + day1);
                        return;
                    }
                }
                else
                {
                    daysInRow = 0;
                }

            }
        }
        public void vinter()
        {
            var tempPerWeek = TempList();
            int daysInRow2 = 0;
            string day2 = "ingenDag";
            foreach (var entry in tempPerWeek)
            {
                var temp = (entry.Value.temp.Sum() / entry.Value.temp.Count);
                if (temp <= 2)
                {
                    if (daysInRow2 == 0)
                    {
                        day2 = entry.Key.ToString();
                    }

                    daysInRow2++;

                    if (daysInRow2 == 5)
                    {
                        Console.WriteLine("Mild vinter börjar: " + day2);
                        return;
                    }

                }
                else
                {
                    daysInRow2 = 0;
                    //Console.WriteLine("det blev mild vinter");

                }
            }
        }

    }
}
