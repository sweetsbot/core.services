namespace Core.Entities
{
    public interface IConfigEntry
    {
        int ConfigEntryId { get; set; }
        string ConfigKeyName { get; set; }
        ConfigValueType ConfigValueType { get; set; }
        string ConfigValue { get; set; }
        bool IsEncrypted { get; set; }
    }
}