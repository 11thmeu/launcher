﻿using _11thLauncher.Model;

namespace _11thLauncher.Messages
{
    public class UpdateStatusBarMessage
    {
        public AsyncAction Action;
        public bool IsRunning;

        public UpdateStatusBarMessage(AsyncAction action, bool isRunning)
        {
            Action = action;
            IsRunning = isRunning;
        }
    }
}
