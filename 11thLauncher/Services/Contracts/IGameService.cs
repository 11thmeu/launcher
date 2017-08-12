﻿using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface IGameService
    {
        LaunchSettings LaunchSettings { get; set; }

        void StartGame();

        void CopyLaunchShortcut();
    }
}
