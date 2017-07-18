﻿using System;
using Caliburn.Micro;

namespace _11thLauncher.Models
{
    public class Repository : PropertyChangedBase
    {
        private string _name;
        private string _path;
        private RepositoryStatus _status;
        private string _address;
        private string _login;
        private string _password;
        private string _localRevision;
        private string _remoteRevision;
        private DateTime _remoteBuildDate;

        public Repository()
        {
            _status = RepositoryStatus.Unknown;
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                NotifyOfPropertyChange();
            }
        }

        public RepositoryStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                NotifyOfPropertyChange();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                NotifyOfPropertyChange();
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                NotifyOfPropertyChange();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyOfPropertyChange();
            }
        }

        public string LocalRevision
        {
            get => _localRevision;
            set
            {
                _localRevision = value;
                NotifyOfPropertyChange();
            }
        }

        public string RemoteRevision
        {
            get => _remoteRevision;
            set
            {
                _remoteRevision = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime RemoteBuildDate
        {
            get => _remoteBuildDate;
            set
            {
                _remoteBuildDate = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
