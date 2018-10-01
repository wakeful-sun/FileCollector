using FileCollector.Common;
using FileCollector.Common.Config;
using System;
using System.Collections.Generic;

namespace FileCollector.Worker
{
    class FileProviderRepository
    {
        readonly Dictionary<ProviderType, Func<IFileProvider>> providerFactories = new Dictionary<ProviderType, Func<IFileProvider>>();

        public void Register(ProviderType providerType, Func<IFileProvider> providerFactory)
        {
            providerFactories.Add(providerType, providerFactory);
        }

        public Func<IFileProvider> Get(ProviderType providerType)
        {
            return providerFactories.GetValueOrDefault(providerType);
        }
    }
}
