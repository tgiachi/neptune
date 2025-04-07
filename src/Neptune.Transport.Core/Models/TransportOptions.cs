namespace Neptune.Transport.Core.Models;

   /// <summary>
    /// Options for configuring a transport
    /// </summary>
    public class TransportOptions
    {
        private readonly Dictionary<string, string> _options = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets an option value
        /// </summary>
        public string this[string key]
        {
            get => _options.TryGetValue(key, out var value) ? value : null;
            set => _options[key] = value;
        }

        /// <summary>
        /// Gets an option value with a default
        /// </summary>
        public string GetOption(string key, string defaultValue = null)
        {
            return _options.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets an option value as integer with a default
        /// </summary>
        public int GetOptionInt(string key, int defaultValue = 0)
        {
            if (_options.TryGetValue(key, out var value) && int.TryParse(value, out var intValue))
                return intValue;
            return defaultValue;
        }

        /// <summary>
        /// Gets an option value as boolean with a default
        /// </summary>
        public bool GetOptionBool(string key, bool defaultValue = false)
        {
            if (_options.TryGetValue(key, out var value) && bool.TryParse(value, out var boolValue))
                return boolValue;
            return defaultValue;
        }

        /// <summary>
        /// Sets an option value
        /// </summary>
        public void SetOption(string key, string value)
        {
            _options[key] = value;
        }

        /// <summary>
        /// Gets all option keys
        /// </summary>
        public IEnumerable<string> Keys => _options.Keys;

        /// <summary>
        /// Gets all options
        /// </summary>
        public IReadOnlyDictionary<string, string> AllOptions => _options;
    }
