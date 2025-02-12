using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace VäderFilGrej.ExtractInformation
{
    internal class SaveData
    {
        public void tempList()
        {

            string[] lines = File.ReadAllLines(@"C:\Users\noelb\Desktop\System24\Filer\tempdata5-medfel.txt");

            string pattern = @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\s(?<time>\d{2}:\d{2}:\d{2}),(?<plats>\w+),(?<temp>\d+\.\d+),(?<humidity>\d+)";

            Dictionary<string, (List<double> tempList, List<double> humidityList)> tempPerMonth = new Dictionary<string, (List<double>, List<double>)>();

            Console.WriteLine("Tryck X för att visa info inomhus ");
            Console.WriteLine("Tryck valfri knapp för att se info utomhus");

            var key = Console.ReadKey(true);

            string val = null;

            if (key.Key == ConsoleKey.X)
            {
                val = "Inne";
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
                        string yearMonth = $"{temp.Groups["year"].Value}-{temp.Groups["month"].Value}";

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
            Console.WriteLine("Tryck A för Medeltemp");
            Console.WriteLine("Tryck B för MedelFuktighet");
            Console.WriteLine("Tryck C för medelmögelrisk");
            Console.WriteLine("Tryck D för Metrologisk Höst/Vinter");
            var key1 = Console.ReadKey(true);
            switch (key1.Key)
            {
                case ConsoleKey.A:
                    SaveMedelTemp(tempPerMonth, key);
                    break;
                case ConsoleKey.B:
                    SaveAvgHumidity(tempPerMonth, key);
                    break;
                case ConsoleKey.C:
                    avgMold(tempPerMonth, key);
                    break;
                    case ConsoleKey.D:
                    FallWinterDays();
                    break;
            }




        }

        public void SaveMedelTemp(Dictionary<string, (List<double> temp, List<double> humidity)> temp, ConsoleKeyInfo key)
        {
            string filePath = "C:\\Users\\noelb\\Desktop\\System24\\Filer\\medelTempInne.txt";
            string filePath1 = "C:\\Users\\noelb\\Desktop\\System24\\Filer\\medelTempUte.txt";


            foreach (var entry in temp)
            {
                if (entry.Key == "2016-13")
                {

                }
                else if (key.Key == ConsoleKey.X)
                {
                    var text = $"Månad:{entry.Key}, Temperaturer: {(entry.Value.temp.Sum() / entry.Value.temp.Count).ToString("F2")}";
                    File.AppendAllText(filePath, text);

                }
                else
                {
                    var text = $"Månad:{entry.Key}, Temperaturer: {(entry.Value.temp.Sum() / entry.Value.temp.Count).ToString("F2")}";
                    File.AppendAllText(filePath1, text);
                }
            }
            Console.WriteLine("Filen har sparats! ");
        }

        public void SaveAvgHumidity(Dictionary<string, (List<double> temp, List<double> humidity)> temp, ConsoleKeyInfo key)
        {
            string filePath = "C:\\Users\\noelb\\Desktop\\System24\\Filer\\avgHumidityInne.txt";
            string filePath1 = "C:\\Users\\noelb\\Desktop\\System24\\Filer\\avgHumidityUte.txt";


            foreach (var entry in temp)
            {
                var text = $"Månad:{entry.Key}, Luftfuktighet: {(entry.Value.humidity.Sum() / entry.Value.humidity.Count).ToString("F2")}";

                if (entry.Key == "2016-13")
                {

                }
                else if (key.Key == ConsoleKey.X)
                {
                    File.AppendAllText(filePath, text);

                }
                else
                {

                    File.AppendAllText(filePath1, text);
                }
            }
            Console.WriteLine("Filen har sparats! ");
        }

        public void avgMold(Dictionary<string, (List<double> temp, List<double> humidity)> temp, ConsoleKeyInfo key)
        {
            string filePath = "C:\\Users\\noelb\\Desktop\\System24\\Filer\\avgMoldInne.txt";
            string filePath1 = "C:\\Users\\noelb\\Desktop\\System24\\Filer\\avgMoldUte.txt";

            double risk;
            Dictionary<string, double> mold = new Dictionary<string, double>();

            foreach (var entry in temp)
            {
                var hum = (entry.Value.humidity.Sum() / entry.Value.humidity.Count);
                var temp1 = (entry.Value.temp.Sum() / entry.Value.temp.Count);

                if (hum >= 78 && temp1 >= 0)
                {
                    risk = (hum - 78) * (temp1 / 15) / 0.22;
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
                return;
            }
            else
            {
                foreach (var entry in mold.OrderBy(entry => entry.Value))
                {
                    string text = ($"Dag {entry.Key} risk: {entry.Value.ToString("F2")}");

                    if (entry.Key == "2016-13")
                    {

                    }
                    else if (key.Key == ConsoleKey.X)
                    {

                        File.AppendAllText(filePath, text);
                    }
                    else
                    {
                        File.AppendAllText(filePath1, text);
                    }
                }
                Console.WriteLine("Filen har sparats! ");

            }

        }

        public void FallWinterDays()
        {
            var data = new GetData();
            bool meny = false;
            var temp = data.TempList(meny, null);

            string filePath = "C:\\Users\\noelb\\Desktop\\System24\\Filer\\AutumnDay.txt";
            string filePath1 = "C:\\Users\\noelb\\Desktop\\System24\\Filer\\VinterDay.txt";

            Console.WriteLine("Tryck H för Höst");
            Console.WriteLine("Tryck V för Vinter");
            var key = Console.ReadKey(true);
            switch(key.Key)
            {
                case ConsoleKey.H:
                    Fall();
                    break;
                case ConsoleKey.V:
                    Vinter();
                    break;

            }

            

            void Fall()
            {
                
                int daysInRow = 0;

                string day1 = "ingenDag";


                foreach (var entry in temp)
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
                            var text = "hösten börjar den: " + day1;
                            File.AppendAllText(filePath, text);
                            Console.WriteLine("Filen har sparats! ");
                            return;
                        }
                    }
                    else
                    {
                        daysInRow = 0;
                    }

                }
            }
            
            void Vinter()
            {
                
                int daysInRow2 = 0;
                string day2 = "ingenDag";
                foreach (var entry in temp)
                {
                    var temp1 = (entry.Value.temp.Sum() / entry.Value.temp.Count);
                    if (temp1 <= 2)
                    {
                        if (daysInRow2 == 0)
                        {
                            day2 = entry.Key.ToString();
                        }

                        daysInRow2++;

                        if (daysInRow2 == 5)
                        {
                            var text = "Mild vinter börjar: " + day2;
                            File.AppendAllText(filePath1, text);
                            Console.WriteLine("Filen har sparats! ");
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
}







