using VäderFilGrej;
using VäderFilGrej.Extencions;
using VäderFilGrej.ExtractInformation;

var saveData = new SaveData();
var balkong = new Balkong();



while (true)
{
    Console.Clear();
    Console.WriteLine("Vill du läsa eller spara data?");
    Console.WriteLine("Tryck 1 för att läsa");
    Console.WriteLine("Tryck 2 för att spara");
    Console.WriteLine("Tryck Esc för att avsluta");

    var key = Console.ReadKey(true);

    switch (key.Key)
    {
        case ConsoleKey.D1:
            Console.Clear();
            ReadMenu();
            break;

        case ConsoleKey.D2:
            Console.Clear();
            saveData.tempList();
            break;

        case ConsoleKey.Escape:
            Environment.Exit(0);
            break;
    }
}
void ReadMenu()
{
    Console.Clear();
    var data = new GetData();
    Console.WriteLine("Vilken data vill du se?");
    Console.WriteLine("[1]Varmt till kallt [2]Temp/Luftfuktighet värden [3]Mögel [4]Tort till blött [5]Metro höst [6]Metro vinter \n[7]Sök [8]Humidex [9]Balkongen [esc]Tillbacka");


    var key = Console.ReadKey(true);

    switch (key.Key)
    {
        case ConsoleKey.D1:
            data.WarmCold();
            Console.ReadKey();
            Console.Clear();
            break;

        case ConsoleKey.D2:
            Console.WriteLine("Tryck A för Medelvärde, B för Median och C för Maxvärde: ");
            var input = Console.ReadKey(true);
            switch (input.Key)
            {
                case ConsoleKey.A:
                    Console.WriteLine("Du har valt Medelvärde ");
                    data.MedTemp(data.CalculateAverage);
                    Console.ReadKey(true);
                    break;

                case ConsoleKey.B:
                    Console.WriteLine("Du har valt Medianvärde ");
                    data.MedTemp(data.CalculateMedian);
                    Console.ReadKey(true);
                    break;
                case ConsoleKey.C:
                    Console.WriteLine("Du har valt Maxvärde ");
                    data.MedTemp(data.CalculateMax);
                    Console.ReadKey(true);
                    break;
            }
            break;
        case ConsoleKey.D3:
            data.Mold();
            Console.ReadKey();
            Console.Clear();
            break;
        case ConsoleKey.D4:
            data.DryWet();
            Console.ReadKey();
            Console.Clear();
            break;
        case ConsoleKey.D5:
            data.Fall();
            Console.ReadKey();
            Console.Clear();
            break;
        case ConsoleKey.D6:
            data.vinter();
            Console.ReadKey();
            Console.Clear();
            break;
        case ConsoleKey.D7:
            "Ange datum enligt 0000-00-00".CW();
            var search = Console.ReadLine();
            search.Search();
            Console.ReadKey();
            Console.Clear();
            break;
        case ConsoleKey.D8:
            data.humIdex();
            Console.ReadKey();
            Console.Clear();
            break;
        case ConsoleKey.D9:
            balkong.Run();
            Console.ReadKey();
            Console.Clear();
            break;
        case ConsoleKey.Escape:
            Console.Clear();
            return;
    }
}
