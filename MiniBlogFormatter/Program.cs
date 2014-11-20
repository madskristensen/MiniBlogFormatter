using MiniBlogFormatter.Formatters;
using System.IO;

namespace MiniBlogFormatter
{
    class Program
    {
        static void Main(string[] args)
        {
            // For BlogEngine.NET only
            var categories = @"C:\dev\MiniBlogFormatter\myblogposts";

            // For both BlogEngine.NET and DasBlog
            //var origin = @"C:\Temp\GhostData.json";
            //var destination = @"C:\Temp\Formatted";


            //BlogEngine(categories, folder, destination);
            //Wordpress(origin, destination);

            //Ghost(origin, destination);

            // For BlogML
            var origin = @"C:\dev\BlogMLExport.xml";
            var destination = @"C:\dev\formatted";

            BlogML(origin, destination);
        }

        static void BlogML(string file, string destination)
        {
            var formatter = new BlogMLFormatter();

            formatter.Format(file, destination);
        }

        private static void DasBlog(string folder, string destination)
        {
            var formatter = new DasBlogFormatter();
            formatter.Format(folder, destination);
        }

        private static void Wordpress(string folder, string destination)
        {
            var formatter = new WordpressFormatter();
            formatter.Format(folder, destination);
        }


        private static void BlogEngine(string categories, string folder, string destination)
        {
            var formatter = new BlogEngineFormatter();
            foreach (string file in Directory.GetFiles(folder, "*.xml"))
            {
                formatter.Format(file, destination, categories);
            }
        }

        private static void Ghost(string folder, string destination)
        {
            var formatter = new GhostFormatter();
            formatter.Format(folder, destination);
        }
    }
}
