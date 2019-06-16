﻿using OpenBullet.Models;
using RuriLib;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows;

namespace OpenBullet.ViewModels
{
    public class ConfigManagerViewModel : ViewModelBase
    {
        private List<ConfigViewModel> cachedConfigs = new List<ConfigViewModel>();

        private ObservableCollection<ConfigViewModel> configsList;

        string status = null;
        //public int counter = 0;

        public ObservableCollection<ConfigViewModel> ConfigsList
        {
            get
            {
                return
                    SearchString == "" ?
                    configsList :
                    new ObservableCollection<ConfigViewModel>(configsList.Where(c => c.Name.ToLower().Contains(SearchString.ToLower())));
            }
            set { configsList = value; OnPropertyChanged("ConfigsList"); OnPropertyChanged("Total"); }
        }

        public int Total { get { return ConfigsList.Count; } }

        public string SavedConfig { get; set; }

        public Config moreInfoConfig;
        public Config MoreInfoConfig { get { return moreInfoConfig; } set { moreInfoConfig = value; OnPropertyChanged("MoreInfoConfig"); } }

        public string CurrentConfigName { get { return Globals.mainWindow.ConfigsPage.CurrentConfig.Name; } }

        private string searchString = "";
        public string SearchString { get { return searchString; } set { searchString = value; OnPropertyChanged("SearchString"); OnPropertyChanged("ConfigsList"); OnPropertyChanged("Total"); } }

        public ConfigManagerViewModel()
        {
            configsList = new ObservableCollection<ConfigViewModel>();
            RefreshList(true);
        }

        public bool NameTaken(string name)
        {
            return ConfigsList.Any(x => x.Name == name);
        }

        public void RefreshCurrent()
        {
            OnPropertyChanged("CurrentConfigName");
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public void RefreshList(bool pullSources)
        {
            // Scan the directory and the sources for configs
            if (pullSources)
            {
                ConfigsList = new ObservableCollection<ConfigViewModel>(
                GetConfigsFromSources()
                .Concat(GetConfigsFromDisk(true))
                );
            }
            else
            {
                ConfigsList = new ObservableCollection<ConfigViewModel>(
                cachedConfigs
                .Concat(GetConfigsFromDisk(true))
                );
            }

            //System.Timers.Timer myTimer = new Timer(600000);
            //myTimer.Start();
            //myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);


            //void myTimer_Elapsed(object sender, ElapsedEventArgs e)
            //{
            //    GetConfigsFromSources();
            //    if (status == "Error") { }
            //        //foreach (RunnerManagerViewModel runner in )
            //}


            OnPropertyChanged("Total");
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public List<ConfigViewModel> GetConfigsFromDisk(bool sort = false)
        {
            List<ConfigViewModel> models = new List<ConfigViewModel>();

            // Load the configs in the root folder
            foreach (var file in Directory.EnumerateFiles(Globals.configFolder).Where(file => file.EndsWith(".loli")))
            {
                try { models.Add(new ConfigViewModel(file, "Default", IOManager.LoadConfig(file))); }
                catch { Globals.LogError(Components.ConfigManager, "Could not load file: " + file); }
            }

            foreach (var file in Directory.EnumerateFiles(Globals.configFolder).Where(file => file.EndsWith(".anom")))
            {
                try { models.Add(new ConfigViewModel(file, "Default", IOManager.LoadConfig(file))); }
                catch { Globals.LogError(Components.ConfigManager, "Could not load file: " + file); }
            }

            // Load the configs in the subfolders
            foreach (var categoryFolder in Directory.EnumerateDirectories(Globals.configFolder))
            {
                foreach (var file in Directory.EnumerateFiles(categoryFolder).Where(file => file.EndsWith(".loli")))
                {
                    try { models.Add(new ConfigViewModel(file, Path.GetFileName(categoryFolder), IOManager.LoadConfig(file))); }
                    catch { Globals.LogError(Components.ConfigManager, "Could not load file: " + file); }
                }

                foreach (var file in Directory.EnumerateFiles(categoryFolder).Where(file => file.EndsWith(".anom")))
                {
                    try { models.Add(new ConfigViewModel(file, Path.GetFileName(categoryFolder), IOManager.LoadConfig(file))); }
                    catch { Globals.LogError(Components.ConfigManager, "Could not load file: " + file); }
                }
            }

            if (sort) { models.Sort((m1, m2) => m1.Config.Settings.LastModified.CompareTo(m2.Config.Settings.LastModified)); }
            return models;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public List<ConfigViewModel> GetConfigsFromSources()
        {
            Source gitHub = new Source(6698);

            var list = new List<ConfigViewModel>();
            cachedConfigs = new List<ConfigViewModel>();

            if (Globals.obSettings.Sources.Sources.Count == 0)
            {
                try
                {
                    gitHub.ApiUrl = "https://github.com/PurityWasHere/Anomaly-Mod-Hosting/blob/master/Configs.zip?raw=true";

                    //counter++;
                    Globals.obSettings.Sources.Sources.Add(gitHub);
                }
                catch { }
            }

            foreach (var source in Globals.obSettings.Sources.Sources)
            {
                WebClient wc = new WebClient();
                switch (source.Auth)
                {
                    case Source.AuthMode.ApiKey:
                        wc.Headers.Add(HttpRequestHeader.Authorization, source.ApiKey);
                        break;

                    case Source.AuthMode.UserPass:
                        var header = BlockFunction.Base64Encode($"{source.Username}:{source.Password}");
                        wc.Headers.Add(HttpRequestHeader.Authorization, $"Basic {header}");
                        break;

                    default:
                        break;
                }

                byte[] file = new byte[] { };
                try
                {
                    file = wc.DownloadData(source.ApiUrl);
                }
                catch (Exception ex)
                {
                    if(!source.ApiUrl.Contains("Anomaly-Mod-Hosting"))
                        MessageBox.Show($"Could not contact API {source.ApiUrl}\r\nReason: {ex.Message}");
                    continue;
                }

                status = wc.ResponseHeaders["Result"];
                if (status != null && status == "Error")
                {
                    if (!source.ApiUrl.Contains("Anomaly-Mod-Hosting"))
                        MessageBox.Show($"Error from API {source.ApiUrl}\r\nThe server says: {Encoding.ASCII.GetString(file)}");
                    continue;
                }

                try
                {
                    using (var zip = new ZipArchive(new MemoryStream(file), ZipArchiveMode.Read))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            using (var stream = entry.Open())
                            {
                                using (TextReader tr = new StreamReader(stream))
                                {
                                    var text = tr.ReadToEnd();
                                    var cfg = IOManager.DeserializeConfig(text);
                                    if (entry.FullName.Contains("REPO.anom"))
                                    {
                                        list.Add(new ConfigViewModel("", "Repository", cfg, true));
                                        cachedConfigs.Add(new ConfigViewModel("", "Repository", cfg, true));
                                    }
                                    else
                                    {
                                        list.Add(new ConfigViewModel("", "Remote", cfg, true));
                                        cachedConfigs.Add(new ConfigViewModel("", "Remote", cfg, true));
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return list;
        }
    }
}
      