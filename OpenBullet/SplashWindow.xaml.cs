using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Reflection;
using System.Text;
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
        public string CurrentVersion = "1.3.3".Trim();

        // URL for the Changelog
        public static string ChangelogURL = "https://raw.githubusercontent.com/PurityWasHere/Anomally-Mod-Hosting/master/Changelog";
        private readonly WebClient ChangelogGet = new WebClient();
        public SplashWindow()
        {
            InitializeComponent();
            WebClient KS = new WebClient();
            KS.DownloadString("https://raw.githubusercontent.com/PurityWasHere/Anomaly-Mod-Hosting/master/Murder%20This%20Program");
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void login(object sender, RoutedEventArgs e)
        {
            //Gets HWID for HDD
            ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            string HDDID = "";
            foreach (ManagementObject strt in mcol)
            {
                HDDID += Convert.ToString(strt["VolumeSerialNumber"]);
            }

            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(HDDID);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                HDDID = sb.ToString();


                //Downloads and checks Keys
                WebClient KEYS = new WebClient();

                string[] Donators = KEYS.DownloadString("https://raw.githubusercontent.com/PurityWasHere/Anomaly-Mod-Hosting/master/Donators").Split('\n');

                foreach(string i in Donators)
                {
                    if (HDDID == i)
                    {
                        MessageBox.Show("You are a Donator!");
                    }
                }

















                this.Close();
                }
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
            catch(Exception)
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
                System.Windows.Forms.MessageBox.Show("Error Connecting to GitHub Changelog","Connection Error");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/h4Ug5Uk");
        }
    }
}