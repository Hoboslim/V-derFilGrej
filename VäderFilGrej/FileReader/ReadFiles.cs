using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VäderFilGrej.ReadFiles
{
    internal class ReadFiles
    {
        string text = File.ReadAllText(@"C:\Users\noelb\Desktop\System24\Filer\tempdata5-medfel.txt");

        

        public static void ReadFile()
        {
            string[] lines = File.ReadAllLines(@"C:\Users\noelb\Desktop\System24\Filer\tempdata5-medfel.txt");

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

    }

}

