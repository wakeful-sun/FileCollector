using FileCollector.Common;
using FileCollector.Common.Config;
using FileCollector.FileSystem;
using FileCollector.Gmail;
using Microsoft.Extensions.Configuration;
using System;

namespace FileCollector.Worker
{
    class Application : IDisposable
    {
        readonly FileProviderRepository fileProviderRepository;
        readonly IDisposable[] services;

        public ProviderConfiguration[] Providers { get; }

        private Application(ProviderConfiguration[] providers, FileProviderRepository fileProviderRepository, params IDisposable[] services)
        {
            this.fileProviderRepository = fileProviderRepository;
            this.services = services;
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

            FileSystemProvider fileSystemProvider = new FileSystemProvider();
            GmailAttachmentProvider gmailAttachmentProvider = new GmailAttachmentProvider(gmailSettings);

            repository.Register(ProviderType.File, () => fileSystemProvider);
            repository.Register(ProviderType.Gmail, () => gmailAttachmentProvider);

            return new Application(providerSettings.Providers, repository, gmailAttachmentProvider);
        }

        public void Dispose()
        {
            if (services != null)
            {
                foreach (var service in services)
                {
                    service.Dispose();
                }
            }
        }
    }
}
