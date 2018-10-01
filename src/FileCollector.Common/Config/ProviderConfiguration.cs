namespace FileCollector.Common.Config
{
    public class ProviderSettings
    {
        public ProviderConfiguration[] Providers { get; set; }
    }

    public class ProviderConfiguration
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public ProviderType Type { get; set; }
        public string Email { get; set; }
        public FileConfiguration FileConfig { get; set; }
    }

    public enum ProviderType
    {
        Undefined = 0,
        File = 1,
        Gmail = 2
    }

    public class FileConfiguration
    {
        public string SourceFileName { get; set; }
        public string SourceDirectory { get; set; }

        public string TargetFileName { get; set; }
        public string TargetDirectory { get; set; }
    }
}
