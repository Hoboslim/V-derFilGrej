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
        string[] lines;

        public Dictionary<string, (List<double> temp, List<double> humidity)> TempList(bool meny, int? ineUte )
        {
            try
            { 
                lines = File.ReadAllLines(@"C:\Users\noelb\Desktop\System24\Filer\tempdata5-medfel.txt");
                //string[] lines = File.ReadAllLines(@"C:\Users\Johan\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");
                //string[] lines = File.ReadAllLines(@"C:\Users\n01re\Source\Repos\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");
            }
            catch
            {
                Console.Clear();
                Console.WriteLine("Misslyckades med läsning av fil, se till att filens sökväg är rätt.");
                Environment.Exit(1);
            }

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
                if (ineUte == 1)
                {
                    val = "Inne";
                }
                else
                {
                    val = "Ute";
                }
            }

            foreach (string line in lines)
            {
                Match temp = Regex.Match(line, pattern);

                if (temp.Success && temp.Groups["plats"].ToString() == val)
                {
                    if (temp.Groups["month"].ToString() == "05" || temp.Groups["month"].ToString() == "01")
                    {
                        continue;
                    }
                    try
                    {
                        string yearMonth = $"{temp.Groups["year"].Value}-{temp.Groups["month"].Value}-{temp.Groups["day"].Value}";

                        if (int.Parse(temp.Groups["month"].Value) > 12)
                        {
                            throw new ArgumentOutOfRangeException($"Månad {temp.Groups["month"].Value} finns inte");
                        }

                        double tempCheck = double.Parse(temp.Groups["temp"].Value, CultureInfo.InvariantCulture);

                        if (tempCheck > 30 || tempCheck < -20)
                        {
                            throw new ArgumentOutOfRangeException($"Orimlig temperatur: {tempCheck}°C på {yearMonth}");
                        }

                        double humidityCheck = double.Parse(temp.Groups["humidity"].Value);

                        if (!tempPerMonth.ContainsKey(yearMonth))
                        {
                            tempPerMonth[yearMonth] = (new List<double>(), new List<double>());
                        }
                        tempPerMonth[yearMonth].tempList.Add(tempCheck);
                        tempPerMonth[yearMonth].humidityList.Add(humidityCheck);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Fel vid bearbetning av rad: {line}. {ex.Message}");
                    }
                }
            }
            return tempPerMonth;
        }
       
        public void Mold()
        {
            meny = true;
            double risk;
            Dictionary<string, double> mold = new Dictionary<string, double>(); 

            var tempPerday = TempList(meny, null);
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
            var tempDay = TempList(meny, null);

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
            var tempPerDay = TempList(meny, null);

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

        public delegate double CalculationDelegate(List<double> values);
        public void MedTemp(CalculationDelegate calculationMethod)
        {
            meny = true;
            var tempPerMonth = TempList(meny, null);

            foreach(var entry in tempPerMonth)
            {
                double avgTemp = calculationMethod(entry.Value.temp);
                double avgHumidity = calculationMethod(entry.Value.humidity);
                Console.WriteLine($"{entry.Key}, Temperatur: {avgTemp:F2}, Luftfuktighet: {avgHumidity:F2}");

            }
        }

        public double CalculateAverage(List<double> Values)
        {
            return Values.Count > 0 ? Values.Sum() / Values.Count() : 0;
        }
        public double CalculateMedian(List<double> values)
        {
            if (values.Count == 0) return 0;
            var sorted = values.OrderBy(x => x).ToList();
            int mid = sorted.Count / 2;
            return sorted.Count % 2 == 0 ? (sorted[mid - 1] + sorted[mid]) / 2.0 : sorted[mid];
        }
        public double CalculateMax(List<double> Values)
        {
            return Values.Count > 0 ? Values.Max() : 0;
        }

        public void Fall()
        {
            meny = false;
            var tempPerWeek = TempList(meny, null);
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
            var tempPerWeek = TempList(meny, null);
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
        public void humIdex() 
        {
            var humiIndex = TempList(false, null);
            var list = new Dictionary<string, double>();
            foreach(var entry in humiIndex) 
            {
                var hum = (entry.Value.humidity.Sum() / entry.Value.humidity.Count);
                var temp = (entry.Value.temp.Sum() / entry.Value.temp.Count);
                var humidIndex = CalculateHumidex(temp, hum);
                
                list.Add(entry.Key, humidIndex);

            }

            var top10 = list.OrderByDescending(ex => ex.Value).Take(10);
            var best5 = top10.OrderBy(ex => ex.Value).Take(5);
            var bottom5 = list.OrderBy(ex => ex.Value).Take(5);

            Console.WriteLine("Bekvämaste Dagarna");
            foreach (var entry in best5)
            {
                Console.WriteLine($"{entry.Key}, Humidex {entry.Value.ToString("F2")}" );
            }
            Console.WriteLine("Obekvemäste dagarna");
            foreach (var entry in bottom5) 
            {
                Console.WriteLine($"{entry.Key}, Humidex {entry.Value.ToString("F2")}");
            }

            double CalculateHumidex(double temperature, double humidity)
            {

                double e = 6.112 * Math.Pow(10, (7.5 * temperature) / (237.7 + temperature)) * (humidity / 100);
                return temperature + (e - 10) / 5;
            }
        }
        //public void Balkong()
        //{
        //    var Inne = TempList(false, 1);
        //    var ute = TempList(false, null);
            


        //}

    }

}

