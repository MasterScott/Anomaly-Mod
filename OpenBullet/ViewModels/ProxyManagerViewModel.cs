﻿using Extreme.Net;
using LiteDB;
using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace OpenBullet.ViewModels
{
    public class ProxyManagerViewModel : ViewModelBase
    {
        private ObservableCollection<CProxy> proxyList;
        public ObservableCollection<CProxy> ProxyList { get { return proxyList; } set { proxyList = value; OnPropertyChanged("TotalProxies"); OnPropertyChanged("TestedProxiesCount"); OnPropertyChanged("HttpProxiesCount"); OnPropertyChanged("Socks4ProxiesCount"); OnPropertyChanged("Socks4aProxiesCount"); OnPropertyChanged("Socks5ProxiesCount"); OnPropertyChanged("WorkingProxiesCount"); OnPropertyChanged("NotWorkingProxiesCount"); } }
        public int TotalProxies { get { return proxyList.Count; } }
        public int TestedProxiesCount { get { return proxyList.Where(x => x.Working != ProxyWorking.UNTESTED).Count(); } }
        public int HttpProxiesCount { get { return proxyList.Where(x => x.Type == ProxyType.Http).Count(); } }
        public int Socks4ProxiesCount { get { return proxyList.Where(x => x.Type == ProxyType.Socks4).Count(); } }
        public int Socks4aProxiesCount { get { return proxyList.Where(x => x.Type == ProxyType.Socks4a).Count(); } }
        public int Socks5ProxiesCount { get { return proxyList.Where(x => x.Type == ProxyType.Socks5).Count(); } }
        public int ChainProxiesCount { get { return proxyList.Where(x => x.Type == ProxyType.Chain).Count(); } }
        public int WorkingProxiesCount { get { return proxyList.Where(x => x.Working == ProxyWorking.YES).Count(); } }
        public int NotWorkingProxiesCount { get { return proxyList.Where(x => x.Working == ProxyWorking.NO).Count(); } }

        private int botsNumber = 1;
        public int BotsNumber { get { return botsNumber; } set { botsNumber = value; OnPropertyChanged("BotsNumber"); } }

        public static string testURL = GetProxySettings("1");
        public string TestURL { get { return testURL; } set { testURL = value; OnPropertyChanged("TestURL"); } }

        public string successKey = GetProxySettings("2");
        public string SuccessKey { get { return successKey; } set { successKey = value; OnPropertyChanged("SuccessKey"); } }

        private bool onlyUntested = true;
        public bool OnlyUntested { get { return onlyUntested; } set { onlyUntested = value; OnPropertyChanged("OnlyUntested"); } }

        private Decimal timeout = 2;
        public Decimal Timeout { get { return timeout; } set { timeout = value; OnPropertyChanged("Timeout"); } }

        public static string GetProxySettings(string type)
        {
        Retry:
            try
            {
                if (type == "1")
                {
                    type = File.ReadAllText(Globals.ProxySettings).Trim();
                }
                else
                {
                    type = File.ReadAllText(Globals.ProxySettings2).Trim();
                }
            }
            catch (Exception ex)
            {
                FileStream myFile;
                if (ex.ToString().Contains("ProxySite"))
                {
                    MessageBox.Show("ProxySite Settings file not found, generating a default one");
                    myFile = File.Create(@"Settings/ProxySite.txt");
                    myFile.Close();
                    File.WriteAllText(@"Settings/ProxySite.txt", "Https://Google.com");
                }
                if (ex.ToString().Contains("ProxyKey"))
                {
                    MessageBox.Show("ProxyKey Settings file not found, generating a default one");
                    myFile = File.Create(@"Settings/ProxyKey.txt");
                    myFile.Close();
                    File.WriteAllText(@"Settings/ProxyKey.txt", "</title>");
                }
                goto Retry;
            }

            return type;
        }

        public void UpdateProperties()
        {
            OnPropertyChanged("TotalProxies");
            OnPropertyChanged("TestedProxiesCount");
            OnPropertyChanged("HttpProxiesCount");
            OnPropertyChanged("Socks4ProxiesCount");
            OnPropertyChanged("Socks4aProxiesCount");
            OnPropertyChanged("Socks5ProxiesCount");
            OnPropertyChanged("ChainProxiesCount");
            OnPropertyChanged("WorkingProxiesCount");
            OnPropertyChanged("NotWorkingProxiesCount");
            OnPropertyChanged("Progress");
        }

        public int Progress
        {
            get
            {
                var ret = 0;
                try { ret = (TestedProxiesCount * 100) / TotalProxies; } catch { } // If Size is 0 this will throw an Exception
                return ret;
            }
        }

        public ProxyManagerViewModel()
        {
            proxyList = new ObservableCollection<CProxy>();
        }

        public void RefreshList()
        {
            using (var db = new LiteDatabase(Globals.dataBaseFile))
            {
                ProxyList = new ObservableCollection<CProxy>(db.GetCollection<CProxy>("proxies").FindAll());
            }
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp) // CARE!!! Extremely slow method!!!
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}