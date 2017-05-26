﻿using Caliburn.Micro;
using _11thLauncher.Model.Addons;

namespace _11thLauncher.Messages
{
    /// <summary>
    /// A message sent when the game addons are loaded. To be handled by an event aggregator.
    /// </summary>
    public class AddonsLoaded
    {
        /// <summary>
        /// Collection of addons added.
        /// </summary>
        public BindableCollection<Addon> Addons;

        /// <summary>
        /// Creates a new instance of the <see cref="AddonsLoaded"/> class, with the loaded addons.
        /// </summary>
        public AddonsLoaded(BindableCollection<Addon> addons)
        {
            Addons = addons;
        }
    }
}