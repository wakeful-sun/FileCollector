using FileCollector.Common.Config;
using Newtonsoft.Json;
using System.IO;

namespace FileCollector.Common
{
    public class ProviderConfigurations
    {
        public static ProviderConfiguration[] ReadFromJson(string path)
        {
            string jsonContent = File.ReadAllText(path);
            ProviderConfiguration[] configurations = JsonConvert.DeserializeObject<ProviderConfiguration[]>(jsonContent);

            return configurations;
        }
    }
}
