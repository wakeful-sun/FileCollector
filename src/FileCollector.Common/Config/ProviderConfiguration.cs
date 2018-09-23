namespace FileCollector.Common.Config
{
    public class ProviderConfiguration
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public ProviderType Type { get; set; }
        public string Email { get; set; }
        public FileConfiguration FileConfig { get; set; }
    }
}
