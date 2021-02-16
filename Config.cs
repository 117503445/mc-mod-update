using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace mc_mod_update
{
    public class Config
    {
        private const string path = "config.json";
        public static Config MainConfig { get; set; }
        public static void Load()
        {
            if (Directory.Exists(path))
            {
                var js = File.ReadAllText(path);
                MainConfig = JsonConvert.DeserializeObject<Config>(js);
            }
            else
            {
                MainConfig = new Config();
                var js = JsonConvert.SerializeObject(MainConfig);
                File.WriteAllText(path, js);
            }
        }
        public string Host { get; set; } = "http://goldhome.117503445.top:8890/public_data/mods";

    }
}
