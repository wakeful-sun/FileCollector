using FileCollector.Common.Config;
using FluentValidation;
using System.IO;

namespace FileCollector.Common
{
    public class ProviderConfigurationValidator : AbstractValidator<ProviderConfiguration>
    {
        public ProviderConfigurationValidator()
        {
            RuleFor(x => x.Type)
                .Must(x => x != ProviderType.Undefined)
                .WithMessage("Provider type is required.");

            RuleFor(x => x.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .When(x => x.Type == ProviderType.Gmail)
                .WithMessage("Email is required.");

            RuleFor(x => x.FileConfig)
                .NotNull()
                .WithMessage("File configuration is required.");

            When(x => x.FileConfig != null, () =>
            {
                RuleFor(x => x.FileConfig.TargetDirectory)
                    .Must(Directory.Exists)
                    .WithMessage(x => $"Target file directory {x.FileConfig.TargetDirectory} does not exist.");

                RuleFor(x => x.FileConfig.TargetFileName)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Target file name is required.");

                When(x => x.Type == ProviderType.File, () =>
                {
                    RuleFor(x => x.FileConfig.SourceDirectory)
                        .Must(Directory.Exists)
                        .WithMessage(x => $"Source file directory {x.FileConfig.SourceDirectory} does not exist.");

                    RuleFor(x => x.FileConfig.SourceFileName)
                        .Must(x => !string.IsNullOrWhiteSpace(x))
                        .WithMessage("Source file name is required.");
                });
            });
        }
    }
}
