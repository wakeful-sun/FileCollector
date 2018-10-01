using FileCollector.Common;
using FileCollector.Common.Config;
using FileCollector.FileSystem;
using FileCollector.Gmail;
using Microsoft.Extensions.Configuration;

namespace FileCollector.Worker
{
    class Application
    {
        readonly FileProviderRepository fileProviderRepository;

        public ProviderConfiguration[] Providers { get; }

        private Application(ProviderConfiguration[] providers, FileProviderRepository fileProviderRepository)
        {
            this.fileProviderRepository = fileProviderRepository;
            Providers = providers;
        }

        public IFileProvider CreateFileProviderFor(ProviderType providerType)
        {
            var factory = fileProviderRepository.Get(providerType);
            if (factory != null)
            {
                return factory.Invoke();
            }
            return null;
        }

        public static Application Create()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("config/providers.json", optional: false, reloadOnChange: true)
                .Build();

            ProviderSettings providerSettings = config.GetSection("providerSettings").Get<ProviderSettings>();
            GmailSettings gmailSettings = config.GetSection("gmail").Get<GmailSettings>();

            FileProviderRepository repository = new FileProviderRepository();
            repository.Register(ProviderType.File, () => new FileSystemProvider());
            repository.Register(ProviderType.Gmail, () => new GmailAttachmentProvider(gmailSettings));

            return new Application(providerSettings.Providers, repository);
        }
    }
}
