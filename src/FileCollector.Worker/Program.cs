using FileCollector.Common;
using FileCollector.Common.Config;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FileCollector.Worker
{
    class Program
    {
        static ProviderConfigurationValidator configurationValidator = new ProviderConfigurationValidator();
        static string separatorLine = new string('-', 70);

        static void Main(string[] args)
        {
            Application app = Application.Create();

            for (int i = 0; i < app.Providers.Length; i++)
            {
                separatorLine.PrintLineInGreen();
                $"{i + 1}/{app.Providers.Length} ".PrintLineInGreen();

                Process(app.Providers[i], x => app.CreateFileProviderFor(x));
            }

            separatorLine.PrintLineInGreen();
            Console.WriteLine();
            "Complited. Press any key to close.".PrintLine();
            //Console.ReadKey();
        }

        static void Process(ProviderConfiguration config, Func<ProviderType, IFileProvider> fileProviderFactory)
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
                $"\t [WARNING]".PrintLineInYellow();
                $" Provider {config.Name} configuration is not valid".PrintLineInYellow();
                $"\t- {string.Join(Environment.NewLine + "\t- ", validationResult.Errors.Select(x => x.ErrorMessage))}".PrintLineInYellow();
                return;
            }

            IFileProvider fileProvider = fileProviderFactory(config.Type);
            if (fileProvider != null)
            {
                try
                {
                    Task<Result> resultTask = fileProvider.Process(config);
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
