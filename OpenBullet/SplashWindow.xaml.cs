using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenBullet
{
    /// <summary>
    ///     Logica di interazione per SplashWindow.xaml
    /// </summary>
    /// 
    [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
    public partial class SplashWindow : Window
    {
        // Local Version Number
        public string CurrentVersion = "1.2.8".Trim();
        // URL for the Changelog
        public static string ChangelogURL = "https://raw.githubusercontent.com/PurityWasHere/OpenBullet-Pures-Mod/master/Changelog";
        private readonly WebClient ChangelogGet = new WebClient();

        public SplashWindow()
        {
            InitializeComponent();
         
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
        public void login(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HandleInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            }
            catch
            {
            }
        }

        private void quitImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        private void Update(object sender, RoutedEventArgs e)
        {
            //Update Check. Checks the Version Number from a URL and compares it to the Local Version
            var wc = new WebClient();
            string NewestVersion = wc.DownloadString("https://raw.githubusercontent.com/PurityWasHere/OpenBullet-Pures-Mod/master/VersionNumber");
            string NewVersionTrimmed = NewestVersion.Trim();
            if (CurrentVersion.Equals(NewVersionTrimmed))
            {
                System.Windows.MessageBox.Show("Up to Date!", "OpenBullet Updater");
            }
            else
            {
                var dialogResult = System.Windows.MessageBox.Show("Update Found! Want to Update?", "OpenBullet Updater", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    System.Windows.MessageBox.Show("The Launcher will now close and Updater will open.", "OpenBullet Updater");
                    Process.Start("OB_Updater");
                    Environment.Exit(0);
                }
            }
        }

        // Loads the text from a URL.
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangeLogBox.Text = ChangelogGet.DownloadString(ChangelogURL);
        }
    }
}