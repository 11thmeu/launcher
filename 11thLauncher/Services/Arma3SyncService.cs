﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.Win32;
using _11thLauncher.Configuration;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;

namespace _11thLauncher.Services
{
    public class Arma3SyncService : IAddonSyncService
    {
        private static string _localRevision;

        private static string _remoteLogin;
        private static string _remotePassword;
        private static string _remoteUrl;
        private static string _remoteRevision;
        private static DateTime _remoteBuildDate;

        public List<Repository> ReadRepositories(string arma3SyncPath)
        {
            List<Repository> repositories = new List<Repository>();
            if (!Directory.Exists(arma3SyncPath)) return repositories;

            string[] files = Directory.GetFiles(Path.Combine(arma3SyncPath, Constants.RepositoryConfigFolder));
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (fileName != null) repositories.Add(new Repository
                {
                    Name = fileName.Substring(0, fileName.IndexOf('.')),
                    Path = file
                });
            }

            return repositories;
        }

        /// <summary>
        /// Get a list of repositories from the Arma3Sync ftp folder
        /// </summary>
        /// <returns>List of current repositories, empty list if the Arma3Sync path is not configured</returns>
        public static List<string> ListRepositories()
        {
            List<string> repositories = new List<string>();

            if (Settings.Arma3SyncPath != "")
            {
                try
                {
                    string[] files = Directory.GetFiles(Settings.Arma3SyncPath + "\\resources\\ftp\\");
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        if (fileName != null) repositories.Add(fileName.Substring(0, fileName.IndexOf('.')));
                    }
                }
                catch (Exception) { }
            }

            return repositories;
        }

        /// <summary>
        /// Check the selected local repository
        /// </summary>
        public static void CheckRepository()
        {
            //MainWindow.UpdateForm("UpdateStatusBar", new object[] { "Comprobando repositorio" });

            //Extract A3SDS
            File.WriteAllBytes(Constants.A3SdsPath, Properties.Resources.A3SDS);

            deserializeLocalRepository();
            deserializeRemoteRepository();

            //Delete A3SDS
            File.Delete(Constants.A3SdsPath);

            string revision = _localRevision;
            bool updated = false;

            if (_localRevision != null && MainWindow.Form != null)
            {
                if (_remoteRevision != null)
                {
                    if (_localRevision.Equals(_remoteRevision))
                    {
                        updated = true;
                    }
                    else
                    {
                        revision = _remoteRevision;
                    }
                }
            }

            //MainWindow.UpdateForm("UpdateRepositoryStatus", new object[] { revision, _remoteBuildDate, updated });
        }

        private static void deserializeLocalRepository()
        {
            string repositoryPath = "\"" + Settings.Arma3SyncPath + "\\resources\\ftp\\" + Settings.Arma3SyncRepository + ".a3s.repository" + "\"";

            Process p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = GetJavaPath(),
                    Arguments = " -jar " + Constants.A3SdsPath + " -deserializeRepository " + repositoryPath
                }
            };
            p.Start();
            string localRepository = p.StandardOutput.ReadToEnd();
            string[] localRepositoryInfo = localRepository.TrimEnd('\r', '\n').Split(',');
            p.WaitForExit();

            if (localRepositoryInfo.Length == 6)
            {
                _localRevision = localRepositoryInfo[1];
                _remoteLogin = localRepositoryInfo[2];
                _remotePassword = localRepositoryInfo[3];
                _remoteUrl = localRepositoryInfo[5];
            }
        }

        private static void deserializeRemoteRepository()
        {
            try
            {
                string tempPath = Path.GetTempPath() + "repoInfo";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _remoteUrl + "/.a3s/serverinfo");
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_remoteLogin, _remotePassword);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                using (Stream s = File.Create(tempPath))
                {
                    responseStream.CopyTo(s);
                }

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = @GetJavaPath();
                p.StartInfo.Arguments = " -jar " + Constants.A3SdsPath + " -deserializeServerInfo \"" + tempPath + "\"";
                p.Start();
                string remoteRepository = p.StandardOutput.ReadToEnd();
                string[] remoteRepositoryInfo = remoteRepository.TrimEnd('\r', '\n').Split(',');
                p.WaitForExit();

                //Delete temp file
                File.Delete(tempPath);

                if (remoteRepositoryInfo != null && remoteRepositoryInfo.Length == 2)
                {
                    _remoteRevision = remoteRepositoryInfo[0];
                    _remoteBuildDate = JavaDateToDatetime(remoteRepositoryInfo[1]);
                }
            }
            catch (WebException) { }
        }

        /// <summary>
        /// Check if Java is present in the system PATH variable
        /// </summary>
        public string GetJavaInSystem()
        {
            var javaVersion = "";
            try
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        FileName = "java",
                        Arguments = "-version"
                    }
                };
                p.Start();
                javaVersion = p.StandardError.ReadLine();
                p.StandardError.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception) { }

            return javaVersion;
        }

        public string GetArma3SyncPath()
        {
            string arma3SyncPath = "";

            if (Environment.Is64BitOperatingSystem)
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(Constants.Arma3SyncBaseRegistryPath64))
                {
                    arma3SyncPath = SearchRegistryInstallations(key);
                }
            }
            else
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(Constants.Arma3SyncBaseRegistryPath32))
                {
                    arma3SyncPath = SearchRegistryInstallations(key);
                }
            }

            if (string.IsNullOrWhiteSpace(arma3SyncPath) || !Directory.Exists(arma3SyncPath))
            {
                arma3SyncPath = "";
            }

            return arma3SyncPath;
        }

        private static string SearchRegistryInstallations(RegistryKey key)
        {
            string arma3SyncPath = "";
            if (key == null) return arma3SyncPath;

            foreach (string subKeyName in key.GetSubKeyNames())
            {
                using (RegistryKey subkey = key.OpenSubKey(subKeyName))
                {
                    if (subkey == null) continue;
                    var displayName = (string)subkey.GetValue(Constants.Arma3SyncRegDisplayNameEntry, "");
                    if (!displayName.StartsWith(Constants.Arma3SyncRegDisplayNameValue)) continue;
                    arma3SyncPath = (string)subkey.GetValue(Constants.Arma3SyncRegLocationEntry, "");
                    break;
                }
            }

            return arma3SyncPath;
        }

        /// <summary>
        /// Start ArmA3Sync in the configured path
        /// </summary>
        public static void StartArmA3Sync()
        {
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Settings.Arma3SyncPath,
                    FileName = Settings.Arma3SyncPath + "\\ArmA3Sync.exe"
                }
            };
            p.Start();
        }

        /// <summary>
        /// Get the Java execution path
        /// </summary>
        /// <returns>Java execution path</returns>
        private static string GetJavaPath()
        {
            var path = Settings.JavaPath != "" ? Settings.JavaPath : "java";

            return path;
        }

        /// <summary>
        /// Convert a Java long date string to C# DateTime
        /// </summary>
        /// <param name="date">Java long date string</param>
        /// <returns>Converted DateTime</returns>
        private static DateTime JavaDateToDatetime(string date)
        {
            var dateLong = long.Parse(date);
            var ss = TimeSpan.FromMilliseconds(dateLong);
            var jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var ddd = jan1St1970.Add(ss);
            var final = ddd.ToUniversalTime();

            return final;
        }
    }
}
