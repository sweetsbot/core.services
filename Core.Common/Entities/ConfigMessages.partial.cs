using Core.Extensions;

namespace Core.Entities
{
    public partial class Setting
    {
        public static Setting Null(string keyName) => new Setting { Key = keyName, Value = string.Empty, Type = SettingType.Null };

        public static Setting FromEntry(IConfigEntry entry)
        {
            return new Setting{ Key = entry.ConfigKeyName, Value = entry.ConfigValue, Type = entry.ConfigValueType.ToSettingType()};
        }
    }

    public partial class GetSettingRes
    {
        
    }
}