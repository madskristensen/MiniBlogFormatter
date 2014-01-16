using System.IO;

namespace MiniBlogFormatter
{
    class Program
    {
        static void Main(string[] args)
        {
            // For BlogEngine.NET only
            string categories = @"C:\dev\MiniBlogFormatter\myblogposts";

            // For both BlogEngine.NET and DasBlog
            string origin = @"C:\dev\MiniBlogFormatter\myblogposts";
            string destination = @"C:\dev\MiniBlogFormatter\formated";


            //BlogEngine(categories, folder, destination);
            Wordpress(origin, destination);
        }

        private static void DasBlog(string folder, string destination)
        {
            DasBlogFormatter formatter = new DasBlogFormatter();
            formatter.Format(folder, destination);
        }

        private static void Wordpress(string folder, string destination)
        {
            WordpressFormatter formatter = new WordpressFormatter();
            formatter.Format(folder, destination);
        }


        private static void BlogEngine(string categories, string folder, string destination)
        {
            BlogEngineFormatter formatter = new BlogEngineFormatter();
            foreach (string file in Directory.GetFiles(folder, "*.xml"))
            {
                formatter.Format(file, destination, categories);
            }
        }
    }
}
