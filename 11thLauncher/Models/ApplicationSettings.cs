﻿using Newtonsoft.Json;

namespace _11thLauncher.Models
{
    public class ApplicationSettings
    {
        private LogLevel? _logLevel;

        #region Properties

        public bool CheckUpdates = true;
        public bool CheckServers = true;
        public bool CheckRepository = false;
        public string Language = "en-US";
        public string Arma3Path = null;
        public StartAction StartAction = StartAction.Nothing;
        public bool MinimizeNotification = false;
        public ThemeStyle ThemeStyle = 0;
        public AccentColor AccentColor = 0;
        public string JavaPath = string.Empty;
        public string Arma3SyncPath = string.Empty;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LogLevel? LogLevel
        {
            get => _logLevel;
            set
            {
                _logLevel = value;
                if (value != null)
                    ApplicationConfig.MaxLogLevel = (LogLevel)value;
            }
        }

        #endregion
    }
}
