using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Business
{
    public class CoreLoggingEvent : IReadOnlyList<KeyValuePair<string, object>>
    {
        readonly List<KeyValuePair<string, object>> _properties;

        public CoreLoggingEvent(string message)
        {
            _properties = new List<KeyValuePair<string, object>>();
            Message = message;
        }

        public string Message { get; set; }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _properties.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => _properties.Count;
        public KeyValuePair<string, object> this[int index] => _properties[index];

        public CoreLoggingEvent AddProperty(string name, object value)
        {
            _properties.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        public static Func<CoreLoggingEvent, Exception, string> Formatter => (l, e) => l.Message;
    }
}