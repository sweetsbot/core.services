using Core.Entities;

namespace Core.Common
{
    public partial class Setting
    {
        public static Setting Null(Key key) => new Setting { Key = key.Value, Value = string.Empty, Type = SettingType.Null };

        public static Setting FromEntry(IConfigEntry entry)
        {
            return new Setting{ Key = entry.ConfigKeyName, Value = entry.ConfigValue, Type = entry.ConfigValueType.ToSettingType()};
        }
    }
}