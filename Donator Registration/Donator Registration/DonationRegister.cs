using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Management;

namespace Donator_Registration
{
    public partial class DonationRegister : Form
    {
        public DonationRegister()
        {
            InitializeComponent();
        }

        private const string Format = "{\"content\":\"Donation method: {0}\nTransaction ID: {1}\nDiscord Name: {2}\"}";
        public static Dictionary<string, string> values;
        bool check = false;

        public static string Hooker = "https://discordapp.com/api/webhooks/593565031963164763/A6Xp8nClhjtU8ZCCk-R5I_il1nVr9xs4rHtPSKuFlPwdAaMohdQoKOI9MY2t8k7MUnYa";

        WebClient wc = new WebClient();
        HttpClient Client = new HttpClient();

        private void TextBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "" || textBox1.Text != null)
            {

                textBox1.Text = "";

            }
        }

        private void TextBox3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() != "" || textBox3.Text != null)
            {

                textBox3.Text = "";

            }
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var btc = false;
            var PPal = false;
            string method = String.Empty;

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



                if (check == false)
                {
                    if (radioButton1.Checked)
                    {
                        btc = false;
                        PPal = true;
                        method = "PAYPAL";
                    }
                    else
                    {
                        btc = true;
                        PPal = false;
                        method = "BTC";
                    }
                    try { wc.DownloadData(Hooker); }

                    catch (Exception)
                    {
                        MessageBox.Show("Error Connecting to Servers. Please try again or contact Developers.");
                    }
                    string data = "{\"content\":\"Payment Method: " + method + " Transaction ID: [" + textBox1.Text + "] Discord Name: [" + textBox3.Text + "] HDD HWID: [" + HDDID + "]" + "\"}";

                    byte[] byteArray = Encoding.UTF8.GetBytes(data);

                    try
                    {
                        HttpWebRequest hook = (HttpWebRequest)WebRequest.Create(Hooker);
                        //WebProxy proxyPOST = new WebProxy(Files.proxy[x], int.Parse(Files.port[x]));
                        hook.ContentType = "application/json";
                        hook.ContentLength = byteArray.Length;
                        hook.Method = "POST";
                        hook.Timeout = 20000;

                        using (Stream webpageStream = hook.GetRequestStream())
                        {
                            webpageStream.Write(byteArray, 0, byteArray.Length);
                        }

                        using (HttpWebResponse webResponse = (HttpWebResponse)hook.GetResponse())
                        {
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                string response = reader.ReadToEnd();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }


                    MessageBox.Show("Your Registration has been sent. Please wait to be notified.", "Proccess Completed.");
                    Environment.Exit(0);
                }
                else
                {
                    MessageBox.Show("Please only use this once!", "Notice");
                }
                check = true;
            }
        }

    }
}
