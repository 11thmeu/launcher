﻿using System.Windows.Controls;
using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class GameViewModel : PropertyChangedBase, IHandle<LoadProfileMessage>, IHandle<FillServerInfoMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILauncherService _launcherService;
        private readonly IAddonService _addonService;
        private readonly ParameterManager _parameterManager;
        private readonly SettingsService _settingsService;

        private LaunchSettings _launchSettings = new LaunchSettings();

        public GameViewModel(IEventAggregator eventAggregator, ILauncherService launcherService, 
            IAddonService addonService, ParameterManager parameterManager, SettingsService settingsService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _launcherService = launcherService;
            _addonService = addonService;
            _parameterManager = parameterManager;
            _settingsService = settingsService;
        }

        #region Message handling

        public void Handle(LoadProfileMessage message)
        {
            //TODO copy gameconfig
        }

        public void Handle(FillServerInfoMessage message)
        {
            if (message.Server == null) return;
            Server = message.Server.Address;
            Port = message.Server.Port.ToString();
        }

        #endregion

        public LaunchOption LaunchOption
        {
            get => _launchSettings.LaunchOption;
            set
            {
                _launchSettings.LaunchOption = value;
                NotifyOfPropertyChange();
            }
        }

        public LaunchPlatform Platform
        {
            get => _launchSettings.Platform;
            set
            {
                _launchSettings.Platform = value;
                NotifyOfPropertyChange();
            }
        }

        public string Server
        {
            get => _launchSettings.Server;
            set
            {
                _launchSettings.Server = value;
                NotifyOfPropertyChange();
            }
        }

        public string Port
        {
            get => _launchSettings.Port;
            set
            {
                _launchSettings.Port = value;
                NotifyOfPropertyChange();
            }
        }

        #region UI Actions

        public void ButtonLaunch(PasswordBox passwordBox)
        {
            _launcherService.StartGame(_addonService.GetAddons(), _parameterManager.Parameters,
                LaunchOption, Platform, Server, Port, passwordBox.Password);
        }

        #endregion
    }
}
