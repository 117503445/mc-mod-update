using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using WebDav;

namespace mc_mod_update
{
    class Program
    {
        //public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
        //{
        //    DirectoryInfo info = new DirectoryInfo(filePath);
        //    if (!info.Exists)
        //    {
        //        info.Create();
        //    }

        //    string path = Path.Combine(filePath, fileName);
        //    using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
        //    {
        //        inputStream.CopyTo(outputFileStream);
        //    }
        //}
        public static IWebDavClient _client = new WebDavClient();
        static void Main(string[] args)
        {
            var mcPath = new DirectoryInfo("./.minecraft/mods");
            if (!mcPath.Exists)
            {
                Console.WriteLine("./.minecraft/mods Not Found");
                Console.Read();
                return;
            }

            Config.Load();
            Console.WriteLine(Config.MainConfig.Host);

            var listLocalFile = from file in mcPath.GetFiles() select file.Name;

            var result = _client.Propfind(Config.MainConfig.Host).Result;

            if (!result.IsSuccessful)
            {
                Console.WriteLine("can't connect to server :(");
                Console.WriteLine(result.Description);
                Console.ReadLine();
                return;
            }

            var listServerFile = new List<string>();
            foreach (var res in result.Resources)
            {
                if (!res.IsCollection)
                {
                    listServerFile.Add(res.DisplayName);
                    //Console.WriteLine(res.DisplayName);
                    //var file = _client.GetRawFile($"{Config.MainConfig.Host}/{res.DisplayName}").Result;
                    //using (FileStream fileStream = new FileStream($"{mcPath}/{ res.DisplayName}", FileMode.Create))
                    //{
                    //    file.Stream.CopyTo(fileStream);
                    //}
                }
            }
            if (listServerFile.Except(listLocalFile).ToList().Count != 0)
            {
                Console.WriteLine("add");
                foreach (var item in listServerFile.Except(listLocalFile))
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine();
            }

            if (listLocalFile.Except(listServerFile).ToList().Count != 0)
            {
                Console.WriteLine("remove");
                foreach (var item in listLocalFile.Except(listServerFile))
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine();
            }

            if (listServerFile.Except(listLocalFile).ToList().Count == 0 && listLocalFile.Except(listServerFile).ToList().Count == 0)
            {
                Console.WriteLine("No Update");
                Console.Read();
                return;
            }

            Console.WriteLine("Press Enter to update");
            Console.Read();

            foreach (var file in listServerFile.Except(listLocalFile))
            {
                var res = _client.GetRawFile($"{Config.MainConfig.Host}/{file}").Result;
                using (FileStream fileStream = new FileStream($"{mcPath}/{file}", FileMode.Create))
                {
                    res.Stream.CopyTo(fileStream);
                }
            }

            foreach (var file in listLocalFile.Except(listServerFile))
            {
                File.Delete($"{mcPath}/{file}");
            }

            Console.Read();
        }
    }
}
