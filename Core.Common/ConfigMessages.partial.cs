using System;
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

    public static class SettingTypeExtensions
    {
        public static SettingType ToSettingType(this ConfigValueType valueType) =>
            valueType switch
            {
                ConfigValueType.Int => SettingType.Int,
                ConfigValueType.Boolean => SettingType.Boolean,
                ConfigValueType.None => SettingType.Null,
                ConfigValueType.String => SettingType.String,
                ConfigValueType.StringArray => SettingType.StringArray,
                ConfigValueType.IntArray => SettingType.IntArray,
                ConfigValueType.Uri => SettingType.Uri,
                ConfigValueType.Double => SettingType.Double,
                _ => throw new ArgumentException("Invalid enum value", nameof(valueType))
            };
        
        public static ConfigValueType ToConfigValueType(this SettingType settingType) =>
            settingType switch
            {
                SettingType.Int => ConfigValueType.Int,
                SettingType.Boolean => ConfigValueType.Boolean,
                SettingType.Null => ConfigValueType.None,
                SettingType.String => ConfigValueType.String,
                SettingType.StringArray => ConfigValueType.StringArray,
                SettingType.IntArray => ConfigValueType.IntArray,
                SettingType.Uri => ConfigValueType.Uri,
                SettingType.Double => ConfigValueType.Double,
                _ => throw new ArgumentException("Invalid enum value", nameof(settingType))
            };
    }
}