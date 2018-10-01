using FileCollector.Common.Config;
using System.IO;

namespace FileCollector.Common
{
    public class FileOperation
    {
        public static string PrepareDestination(FileConfiguration config)
        {
            string destinationFilePath = Path.Combine(config.TargetDirectory, config.TargetFileName);
            if (new FileInfo(destinationFilePath).Exists)
            {
                File.Delete(destinationFilePath);
            }

            return destinationFilePath;
        }
    }
}
