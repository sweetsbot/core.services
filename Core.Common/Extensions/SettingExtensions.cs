using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;

namespace Core.Extensions
{
    public static class SettingExtensions
    {
        public static IEnumerable<Setting> AsSettings(this IEnumerable<IConfigEntry> entries)
        {
            if (entries == null) throw new NullReferenceException(nameof(entries));

            return entries.Select(Setting.FromEntry);
        }
    }
}