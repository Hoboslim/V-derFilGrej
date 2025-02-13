using VäderFilGrej.Extencions;
using VäderFilGrej.ExtractInformation;

namespace VäderFilGrej
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var saveData = new SaveData();


            while (true)
            {

                Console.WriteLine("Vill du läsa eller spara data?");
                Console.WriteLine("Tryck 1 för att läsa");
                Console.WriteLine("Tryck 2 för att spara");
                Console.WriteLine("Tryck Esc för att avsluta");

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        ReadMenu();
                        break;

                    case ConsoleKey.D2:
                        saveData.tempList();
                        break;

                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                }
            }
            void ReadMenu()
            {
                var data = new GetData();
                Console.WriteLine("Vilken data vill du se?");
                Console.WriteLine("[1]Varmt till kallt [2]medel temp [3]Mögel [4]Tort till blött [5]Metro höst [6]Metro vinter [7]Sök [8]Humidex [esc]Tillbacka");


                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        data.WarmCold();
                        break;

                    case ConsoleKey.D2:
                        data.MedTemp();
                        break;
                    case ConsoleKey.D3:
                        data.Mold();
                        break;
                    case ConsoleKey.D4:
                        data.DryWet();
                        break;
                    case ConsoleKey.D5:
                        data.Fall();
                        break;
                    case ConsoleKey.D6:
                        data.vinter();
                        break;
                    case ConsoleKey.D7:
                        "Ange datum enligt 0000-00-00".CW();
                        var search = Console.ReadLine();
                        search.Search();
                        break;
                    case ConsoleKey.D8:
                        data.humIdex();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }

        }


        
    }
}
