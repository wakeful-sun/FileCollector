using FileCollector.Common.Config;
using System.Threading.Tasks;

namespace FileCollector.Common
{
    public interface IFileProvider
    {
        Task<Result> Process(ProviderConfiguration configuration);
    }
}
