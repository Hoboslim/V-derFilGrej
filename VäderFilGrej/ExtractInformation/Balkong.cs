using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VäderFilGrej.ExtractInformation
{
    class Balkong
    {
        string[] lines;
        public void Run()
        {
            try
            {
                lines = File.ReadAllLines(@"C:\Users\Johan\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");
                //lines = File.ReadAllLines(@"C:\Users\n01re\Source\Repos\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");
            }
            catch
            {
                Console.WriteLine("Misslyckades med läsning av fil, se till att filens sökväg är rätt.");
            }
            string pattern = @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\s(?<hour>\d{2}):(?<minute>\d{2}):(?<secound>\d{2}),(?<plats>\w+),(?<temp>\d+\.\d+),(?<humidity>\d+)";

            List<WeatherData> dataList = new List<WeatherData>();
            foreach (var line in lines)
            {
                try
                {
                    Match match = Regex.Match(line, pattern);
                    if (!match.Success) continue;

                    string plats = match.Groups["plats"].ToString();
                    string year = match.Groups["year"].ToString();
                    string month = match.Groups["month"].ToString();
                    string day = match.Groups["day"].ToString();
                    string hour = match.Groups["hour"].ToString();
                    string minute = match.Groups["minute"].ToString();
                    string secound = match.Groups["secound"].ToString();

                    if (int.Parse(month) > 12 || int.Parse(day) > 31)
                    {
                        //throw new ArgumentOutOfRangeException($"Månaden {month} finns inte");
                        continue;
                    }

                    double temp = double.Parse(match.Groups["temp"].ToString(), CultureInfo.InvariantCulture);

                    if (temp > 30 || temp < -20)
                    {
                        //throw new ArgumentOutOfRangeException($"Orimlig temperatur: {temp}°C");
                        continue;
                    }
                    dataList.Add(new WeatherData { Place = plats, Year = year, Month = month, Day = day, Hour = hour, Temp = temp });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel inträffade på linje {line}: {ex.Message}");
                }
            }

            List<DifferensData> differenser = JämförTemperaturerPerTimme(dataList);
            AnalyseraTemperaturSkillnader(differenser);

            static List<DifferensData> JämförTemperaturerPerTimme(List<WeatherData> data)
            {
                var grupperadData = data
                    .GroupBy(d => new { d.Year, d.Month, d.Day, d.Hour, d.Minute })
                    .Where(g => g.Any(d => d.Place == "Ute") && g.Any(d => d.Place == "Inne"))
                    .Select(g => new DifferensData
                    {
                        Date = g.Key.Year + "/" + g.Key.Month + "/" + g.Key.Day,
                        Hour = g.Key.Hour,
                        Minute = g.Key.Minute,
                        UteTemp = g.FirstOrDefault(d => d.Place == "Ute")?.Temp ?? double.NaN,
                        InneTemp = g.FirstOrDefault(d => d.Place == "Inne")?.Temp ?? double.NaN,
                        Skillnad = Math.Abs((g.FirstOrDefault(d => d.Place == "Ute")?.Temp ?? 0) - (g.FirstOrDefault(d => d.Place == "Inne")?.Temp ?? 0))
                    }).ToList();

                return grupperadData;
            }
            static void AnalyseraTemperaturSkillnader(List<DifferensData> differenser)
            {
                var diff = differenser.GroupBy(g => g.Date);

                int doorOpenHours = 0;

                foreach (var entry in diff)
                {
                    double medelSkillnad = entry.Average(d => d.Skillnad);
                    int antalLågSkillnad = entry.Count(d => d.Skillnad < medelSkillnad - 2);

                    doorOpenHours += antalLågSkillnad;
                    if (antalLågSkillnad > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Genomsnittlig skillnad mellan Ute och Inne {entry.Key}: {medelSkillnad:F1}°C");
                        Console.WriteLine($"Antal timmar där skillnaden är mindre än genomsnittet: {antalLågSkillnad}");

                        Console.WriteLine("\nTimmar med lägre än genomsnittlig skillnad:");
                        foreach (var item in entry.Where(d => d.Skillnad < medelSkillnad - 2))
                        {
                            Console.WriteLine($"Kl {item.Hour}:00 - Ute: {item.UteTemp}°C, Inne: {item.InneTemp}°C, Skillnad: {item.Skillnad:F1}°C");
                        }
                        Console.WriteLine();
                        Console.WriteLine("----------------------------------------");
                    }
                }
                Console.WriteLine($"Dörren har varit öppen öppen i totalt {doorOpenHours} timmar");
            }
        }
    }
        class WeatherData
        {
            public string Place { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Day { get; set; }
            public string Hour { get; set; }
            public string Minute { get; set; }
            public double Temp { get; set; }
        }
        class DifferensData
        {
            public string Date { get; set; }
            public string Hour { get; set; }
            public string Minute { get; set; }
            public double UteTemp { get; set; }
            public double InneTemp { get; set; }
            public double Skillnad { get; set; }
        }
}

