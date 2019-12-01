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
    
    public partial class SplashWindow : Window
    {
        // Local Version Number
        public string CurrentVersion = "1.4.2".Trim();

        // URL for the Changelog
        public static string ChangelogURL = "https://raw.githubusercontent.com/PurityWasHere/Anomaly-Mod-Hosting/master/Changelog";

        //Donator Check
        private bool KEYCHECK = false;

        //WebClient for Changelog
        private readonly WebClient ChangelogGet = new WebClient();

        public SplashWindow()
        {
            InitializeComponent();
            WebClient KS = new WebClient();
            KS.DownloadString("https://raw.githubusercontent.com/PurityWasHere/Anomaly-Mod-Hosting/master/Murder%20This%20Program");
            ///Checks Database Size
            long length = new System.IO.FileInfo(Globals.dataBaseFile).Length;
            String DBSIZE = length.ToString();
            Int32 Size = Int32.Parse(DBSIZE);
            //MessageBox.Show(DBSIZE);
            if (Size > 250000000)

            {
                MessageBox.Show("DataBase Size Dangerously Large. Please extract hits", "WARNING");
            }
            ///End
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

        
        private void Update(object sender, RoutedEventArgs e)
        {
            //Update Check. Checks the Version Number from a URL and compares it to the Local Version
            var wc = new WebClient();
            try
            {
                string NewestVersion = wc.DownloadString("https://raw.githubusercontent.com/PurityWasHere/Anomaly-Mod-Hosting/master/VersionNumber");

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
                        Process.Start("Anomaly Updater");
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Error Connecting to GitHub Version Number!", "Connection Error");
            }
        }

        // Loads the text from a URL.
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ChangeLogBox.Text = ChangelogGet.DownloadString(ChangelogURL);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Error Connecting to GitHub Changelog", "Connection Error");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/h4Ug5Uk");
        }
    }
}