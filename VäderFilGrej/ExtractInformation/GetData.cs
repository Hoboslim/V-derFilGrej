using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VäderFilGrej.ExtractInformation
{
    internal class GetData
    {
        bool meny = false;

        public Dictionary<string, (List<double> temp, List<double> humidity)> TempList(bool meny)
        {
            string[] lines = File.ReadAllLines(@"C:\Users\noelb\Desktop\System24\Filer\tempdata5-medfel.txt");
            //string[] lines = File.ReadAllLines(@"C:\Users\Johan\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");

            string pattern = @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\s(?<time>\d{2}:\d{2}:\d{2}),(?<plats>\w+),(?<temp>\d+\.\d+),(?<humidity>\d+)";

            Dictionary<string, (List<double> tempList, List<double> humidityList)> tempPerMonth = new Dictionary<string, (List<double>, List<double>)>();

            string val = null;

            if (meny == true)
            {
                Console.WriteLine("Tryck X för att visa info inomhus ");
                Console.WriteLine("Tryck valfri knapp för att se info utomhus");

                var key = Console.ReadKey(true);

                

                if (key.Key == ConsoleKey.X)
                {
                    val = "Inne";
                }
                else
                {
                    val = "Ute";
                }

            }
            else 
            {
                val = "Ute";
            }

            foreach (string line in lines)
            {
                Match temp = Regex.Match(line, pattern);

                if (temp.Success && temp.Groups["plats"].ToString() == val)
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
        public void Search()
        {
            Console.WriteLine("Ange datum enligt 0000-00-00");
            var search = Console.ReadLine();

            var list = TempList(false);
            
            foreach (var entry in list)
            {
                if (entry.Key == search)
                {
                    Console.WriteLine($"Månad:{entry.Key}, Temperaturer: {(entry.Value.temp.Sum() / entry.Value.temp.Count).ToString("F2")}" +
                   $" Luftfuktighet: {(entry.Value.humidity.Sum() / entry.Value.humidity.Count).ToString("F2")}");
                    Console.ReadKey();

                }
            }

        }
        public void Mold()
        {
            meny = true;
            double risk;
            Dictionary<string, double> mold = new Dictionary<string, double>(); 

            var tempPerday = TempList(meny);
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

                if (risk > 0)
                {
                  mold.Add(entry.Key, risk);
                }
            }

            if (!mold.Any())
            {
                Console.WriteLine("Det finns ingen risk för mögel ");
            }

            foreach (var entry in mold.OrderBy(entry => entry.Value))
            { 
               Console.WriteLine($"Dag {entry.Key} risk: {entry.Value.ToString("F2")}");
            }
        }
        public void DryWet()
        {
            meny = true;
            var tempDay = TempList(meny);

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
            meny = true;
            var tempPerDay = TempList(meny);

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
            meny = true;
            var tempPerMonth = TempList(meny);

            foreach(var entry in tempPerMonth)
            {
               Console.WriteLine($"Månad:{entry.Key}, Temperaturer: {(entry.Value.temp.Sum() / entry.Value.temp.Count).ToString("F2")}" +
                   $" Luftfuktighet: {(entry.Value.humidity.Sum() / entry.Value.humidity.Count).ToString("F2")}");

                
                var hum = (entry.Value.humidity.Sum() / entry.Value.humidity.Count);
                
            }
        }

        public void Fall()
        {
            meny = false;
            var tempPerWeek = TempList(meny);
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
            meny = false;
            var tempPerWeek = TempList(meny);
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
