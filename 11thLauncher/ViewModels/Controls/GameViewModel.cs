﻿using Caliburn.Micro;
using _11thLauncher.Messages;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.ViewModels.Controls
{
    public class GameViewModel : PropertyChangedBase, IHandle<ProfileLoadedMessage>, IHandle<FillServerInfoMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameService _gameService;
        private readonly ISecurityService _securityService;

        private bool _loadingProfile;

        public GameViewModel(IEventAggregator eventAggregator, IGameService gameService, ISecurityService securityService)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _gameService = gameService;
            _securityService = securityService;
        }

        #region Message handling

        public void Handle(ProfileLoadedMessage message)
        {
            _loadingProfile = true;

            _gameService.LaunchSettings.LaunchOption = message.LaunchSettings.LaunchOption;
            NotifyOfPropertyChange(() => LaunchOption);

            _gameService.LaunchSettings.Platform = message.LaunchSettings.Platform;
            NotifyOfPropertyChange(() => Platform);

            _gameService.LaunchSettings.Server = message.LaunchSettings.Server;
            NotifyOfPropertyChange(() => Server);

            _gameService.LaunchSettings.Port = message.LaunchSettings.Port;
            NotifyOfPropertyChange(() => Port);

            _gameService.LaunchSettings.Password = message.LaunchSettings.Password;
            NotifyOfPropertyChange(() => Password);
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
            get => _gameService.LaunchSettings.LaunchOption;
            set
            {
                _gameService.LaunchSettings.LaunchOption = value;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
                NotifyOfPropertyChange();
            }
        }

        public LaunchPlatform Platform
        {
            get => _gameService.LaunchSettings.Platform;
            set
            {
                _gameService.LaunchSettings.Platform = value;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
                NotifyOfPropertyChange();
            }
        }

        public string Server
        {
            get => _gameService.LaunchSettings.Server;
            set
            {
                _gameService.LaunchSettings.Server = value;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
                NotifyOfPropertyChange();
            }
        }

        public string Port
        {
            get => _gameService.LaunchSettings.Port;
            set
            {
                _gameService.LaunchSettings.Port = value;
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
                NotifyOfPropertyChange();
            }
        }

        public string Password
        {
            get => _securityService.DecryptPassword(_gameService.LaunchSettings.Password);
            set
            {
                _gameService.LaunchSettings.Password = _securityService.EncryptPassword(value);
                NotifyOfPropertyChange();
                if (_loadingProfile) { _loadingProfile = false; return; } //Avoid profile write when loading profile
                _eventAggregator.PublishOnCurrentThread(new SaveProfileMessage());
            }
        }

        #region UI Actions

        public void ButtonLaunch()
        {
            _gameService.StartGame();
        }

        public void ButtonCopyToClipboard()
        {
            _gameService.CopyLaunchShortcut();
        }

        #endregion
    }
}
