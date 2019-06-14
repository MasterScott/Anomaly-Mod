﻿using OpenBullet.Pages.Main.Settings;
using RuriLib;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per OBSettings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        private OBSettingsPage OBSettingsPage = new OBSettingsPage();
        private RLSettingsPage RLSettingsPage = new RLSettingsPage();

        public Settings()
        {
            InitializeComponent();

            menuOptionRuriLib_MouseDown(this, null);
        }

        #region Menu Options MouseDown Events

        private void menuOptionRuriLib_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = RLSettingsPage;
            menuOptionSelected(menuOptionRuriLib);
        }

        private void menuOptionOpenBullet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = OBSettingsPage;
            menuOptionSelected(menuOptionOpenBullet);
        }

        private void menuOptionSelected(object sender)
        {
            foreach (var child in topMenu.Children)
            {
                try
                {
                    var c = (Label)child;
                    c.Foreground = Globals.GetBrush("ForegroundMain");
                }
                catch { }
            }
            ((Label)sender).Foreground = Globals.GetBrush("ForegroundCustom");
        }

        #endregion Menu Options MouseDown Events

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            IOManager.SaveSettings(Globals.rlSettingsFile, Globals.rlSettings);
        }
    }
}