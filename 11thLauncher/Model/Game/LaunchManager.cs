﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using _11thLauncher.Model.Parameter;

namespace _11thLauncher.Model.Game
{
    public class LaunchManager
    {
        public LaunchError StartGame(string gamePath, Collection<Addon.Addon> addons, Collection<LaunchParameter> parameters, 
            LaunchOption launchOption, Platform platform, Priority priority,
            string server, string port, string password)
        {
            if (!SteamRunning())
            {
                return LaunchError.NoSteam;
            }

            string gameParams = "";
            foreach (var param in parameters)
            {
                if (!param.IsEnabled) continue;
                if (param.Platform != Platform.Any && param.Platform != platform) continue;

                gameParams += " " + param.Name;
                if (param.Type == ParameterType.Selection || param.Type == ParameterType.Text)
                {
                    gameParams += param.SelectedValue.Value;
                }
            }

            //Parameters
            //if (Profiles.GetParameter("emptyWorld", false))
            //    launchParams += " -world=empty";

            //if (Profiles.GetParameter("maxMemory", false))
            //{
            //    switch (Profiles.GetParameter("maxMemoryValue", "0"))
            //    {
            //        case "0":
            //            launchParams += " -maxMem=768";
            //            break;
            //        case "1":
            //            launchParams += " -maxMem=1024";
            //            break;
            //        case "2":
            //            launchParams += " -maxMem=2048";
            //            break;
            //        case "3":
            //            launchParams += " -maxMem=4096";
            //            break;
            //        case "4":
            //            launchParams += " -maxMem=8192";
            //            break;
            //        default:
            //            launchParams += " -maxMem=768";
            //            break;
            //    }
            //}
            //if (Profiles.GetParameter("maxVMemory", false))
            //{
            //    switch (Profiles.GetParameter("maxVMemoryValue", "0"))
            //    {
            //        case "0":
            //            launchParams += " -maxVRAM=128";
            //            break;
            //        case "1":
            //            launchParams += " -maxVRAM=256";
            //            break;
            //        case "2":
            //            launchParams += " -maxVRAM=512";
            //            break;
            //        case "3":
            //            launchParams += " -maxVRAM=1024";
            //            break;
            //        case "4":
            //            launchParams += " -maxVRAM=2048";
            //            break;
            //        case "5":
            //            launchParams += " -maxVRAM=4096";
            //            break;
            //        case "6":
            //            launchParams += " -maxVRAM=8192";
            //            break;
            //        default:
            //            launchParams += " -maxVRAM=1024";
            //            break;
            //    }
            //}
            //if (Profiles.GetParameter("cpuCount", false))
            //{
            //    int value = Profiles.GetParameter("cpuCountValue", 0) + 1;
            //    launchParams += " -cpuCount=" + value;
            //}
            //if (Profiles.GetParameter("extraThreads", false))
            //{
            //    switch (Profiles.GetParameter("extraThreadsValue", "0"))
            //    {
            //        case "0":
            //            launchParams += " -exThreads=0";
            //            break;
            //        case "1":
            //            launchParams += " -exThreads=1";
            //            break;
            //        case "2":
            //            launchParams += " -exThreads=3";
            //            break;
            //        case "3":
            //            launchParams += " -exThreads=5";
            //            break;
            //        case "4":
            //            launchParams += " -exThreads=7";
            //            break;
            //        default:
            //            launchParams += " -exThreads=0";
            //            break;
            //    }
            //}

            //Addons
            string addonParams = "";
            foreach (var addon in addons)
            {
                if (addon.IsEnabled)
                    addonParams += addon.Name + ";";
            }
            if (addonParams.Length > 0)
            {
                gameParams += " -mod=" + addonParams;
            }

            //Server connection
            if (launchOption == LaunchOption.JoinServer)
            {
                if (server.Length > 0)
                    gameParams += " -connect=" + server;
                if (port.Length > 0)
                    gameParams += " -port=" + port;
                if (password.Length > 0)
                    gameParams += " -password=" + password;
            }

            string fileName;
            switch (platform)
            {
                case Platform.X86:
                    fileName = Constants.Arma3Filename32;
                    break;

                case Platform.X64:
                    fileName = Constants.Arma3Filename64;
                    break;

                case Platform.Any:
                    fileName = Constants.Arma3Filename32;
                    break;

                default:
                    fileName = Constants.Arma3Filename32;
                    break;
            }

            if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)))
            {
                return LaunchError.NoElevation;
            }

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = gamePath + "\\" + fileName,
                    Verb = "runas"
                }
            };

            if (gameParams.Length > 0)
            {
                process.StartInfo.Arguments = gameParams;
            }

            //Start process
            if (!string.IsNullOrEmpty(gamePath))
            {
                process.Start();

                //Set priority
                switch (priority)
                {
                    case Priority.Normal:
                        process.PriorityClass = ProcessPriorityClass.Normal;
                        break;
                    case Priority.AboveNormal:
                        process.PriorityClass = ProcessPriorityClass.AboveNormal;
                        break;
                    case Priority.High:
                        process.PriorityClass = ProcessPriorityClass.High;
                        break;
                    default:
                        process.PriorityClass = ProcessPriorityClass.Normal;
                        break;
                }
            }
            else
            {
                return LaunchError.NoGamePath;
            }

            return LaunchError.None;
        }

        private static bool SteamRunning()
        {
            var steamProcess = Process.GetProcessesByName("Steam").FirstOrDefault();
            return steamProcess != null;
        }
    }
}