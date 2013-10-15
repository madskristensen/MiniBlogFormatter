using System.IO;

namespace MiniBlogFormatter
{
    class Program
    {
        static void Main(string[] args)
        {
            // For BlogEngine.NET only
            string categories = @"C:\Users\madsk\Desktop\BlogPosts\categories.xml";

            // For both BlogEngine.NET and DasBlog
            string origin = @"C:\Users\madsk\Desktop\SayedBlog\content"; 
            string destination = @"C:\Users\madsk\Desktop\SayedBlog\posts";


            //BlogEngine(categories, folder, destination);
            DasBlog(origin, destination);
        }

        private static void DasBlog(string folder, string destination)
        {
            DasBlogFormatter formatter = new DasBlogFormatter();
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
