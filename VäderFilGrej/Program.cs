using VäderFilGrej.ExtractInformation;

namespace VäderFilGrej
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ReadFiles.ReadFiles.ReadFile();
            //ExtractInformation.MedelTemp.ExtractMedelTemp();
            //ExtractInformation.WarmestColdest.WarmCold();
            var data = new GetData();

            //data.WarmCold();
            //data.MedTemp();
            //data.Mold();
            //data.DryWet();
            data.Fall();
        }
    }
}
