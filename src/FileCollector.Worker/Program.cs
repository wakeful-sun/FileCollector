using FileCollector.Common;
using FileCollector.Common.Config;
using FileCollector.FileSystem;
using FileCollector.Gmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileCollector.Worker
{
    class Program
    {
        static Dictionary<ProviderType, Func<IFileProvider>> providerFactories = RegisterProviders();
        static ProviderConfigurationValidator configurationValidator = new ProviderConfigurationValidator();
        static string separatorLine = new string('-', 70);

        static void Main(string[] args)
        {
            ProviderConfiguration[] configurations = ProviderConfigurations.ReadFromJson(@"config\providers.json");

            for (int i = 0; i < configurations.Length; i++)
            {
                separatorLine.PrintLineInGreen();
                $"{i + 1}/{configurations.Length} ".PrintInColor(ConsoleColor.Green);
                Process(configurations[i]);
            }

            separatorLine.PrintLineInGreen();
            Console.WriteLine();
            "Complited. Press any key to close.".PrintLine();
            Console.ReadKey();
        }

        static Dictionary<ProviderType, Func<IFileProvider>> RegisterProviders()
        {
            return new Dictionary<ProviderType, Func<IFileProvider>>
            {
                { ProviderType.File, () => new FileSystemProvider() },
                { ProviderType.Gmail, () => new GmailAttachmentProvider() }
            };
        }

        static void Process(ProviderConfiguration config)
        {
            $"Processing file for [{config.Name}] ...".Print();

            if (!config.Active)
            {
                $"\t [INFO]".PrintInColor(ConsoleColor.Magenta);
                " Configuration is not active.".PrintLine();
                return;
            }

            var validationResult = configurationValidator.Validate(config);
            if (!validationResult.IsValid)
            {
                $"\t [WARNING]".PrintLineInColor(ConsoleColor.Yellow);
                $" Provider {config.Name} configuration is not valid".PrintLineInYellow();
                $"\t- {string.Join(Environment.NewLine + "\t- ", validationResult.Errors.Select(x => x.ErrorMessage))}".PrintLineInYellow();
                return;
            }

            Func<IFileProvider> fileProviderFactory = providerFactories.GetValueOrDefault(config.Type);
            if (fileProviderFactory != null)
            {
                try
                {
                    Task<Result> resultTask = fileProviderFactory().Process(config);
                    Result result = resultTask.GetAwaiter().GetResult();

                    result.Print();
                }
                catch (Exception e)
                {
                    "\t [FAILED]".PrintLineInColor(ConsoleColor.Red);
                    $"\t Error message: {e.Message}".PrintLine();
                }
            }
            else
            {
                $"Invalid configuration entry - not supported provider type [{config.Type}] for [{config.Name}] provider.".PrintLine();
            }
        }
    }
}
