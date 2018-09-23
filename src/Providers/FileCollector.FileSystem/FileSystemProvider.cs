using FileCollector.Common;
using FileCollector.Common.Config;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileCollector.FileSystem
{
    public class FileSystemProvider : IFileProvider
    {
        public Task<Result> Process(ProviderConfiguration configuration)
        {
            FileConfiguration fileConfig = configuration.FileConfig;

            string sourceFilePath;
            string[] sourceFilePaths = Directory.GetFiles(fileConfig.SourceDirectory, fileConfig.SourceFileName).ToArray();

            switch (sourceFilePaths.Length)
            {
                case 0:
                    string warning = $"0 files were found by {fileConfig.SourceFileName} file name template in {fileConfig.SourceDirectory} directory";
                    return Task.FromResult(Result.Warning(warning));
                case 1:
                    sourceFilePath = sourceFilePaths[0];
                    break;
                default:
                    string error = $"{fileConfig.SourceDirectory} directory expected to contain 1 file for {fileConfig.SourceFileName} file name template, but {sourceFilePaths.Length} found.";
                    return Task.FromResult(Result.Error(error));
            }

            string destinationFilePath = Path.Combine(fileConfig.TargetDirectory, fileConfig.TargetFileName);
            if (new FileInfo(destinationFilePath).Exists)
            {
                File.Delete(destinationFilePath);
            }

            File.Move(sourceFilePath, destinationFilePath);

            return Task.FromResult(Result.Ok());
        }
    }
}
