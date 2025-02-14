using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VäderFilGrej.ExtractInformation
{
    internal class BalkongenOppen
    {
        public void balkongoppen()
        {
            string[] lines = File.ReadAllLines(@"C:\Users\n01re\Source\Repos\V-derFilGrej\VäderFilGrej\FileReader\tempdata5.txt");

            string pattern = @"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\s(?<time>\d{2}:\d{2}:\d{2}),(?<plats>\w+),(?<temp>\d+\.\d+),(?<humidity>\d+)";

            foreach (string line in lines)
            {
                Match temp = Regex.Match(line, pattern);

                //if (temp.Success && temp.Groups["plats"].ToString() == "Inne") 
                
                    if (temp.Groups["month"].ToString() == "05" || temp.Groups["month"].ToString() == "01")
                    {

                    }
                    else
                    { 
                        foreach (var ex in temp.Groups["month"].ToString())
                        {
                            double diffrence = 0;

                            foreach (var something in temp.Groups["time"].ToString())
                            {
                                double value = 0;    

                                foreach (var plats in temp.Groups["plats"].ToString())
                                {
                                    //value = double.Parse(plats.Groups["temp"].ToString());
                                }
                                
                            }
                        }
                    }
                
            }
        }

        
    }
}
