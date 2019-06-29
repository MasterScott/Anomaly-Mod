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

namespace Donator_Registration
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool check = false;

        string Hooker = "https://discordapp.com/api/webhooks/593565031963164763/A6Xp8nClhjtU8ZCCk-R5I_il1nVr9xs4rHtPSKuFlPwdAaMohdQoKOI9MY2t8k7MUnYa";

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
            if (check == false)
            {
                if (radioButton1.Checked)
                {
                    btc = false;
                    PPal = true;
                }
                else
                {
                    btc = true;
                    PPal = false;
                }
                try { wc.DownloadData(Hooker); }

                catch (Exception)
                {
                    MessageBox.Show("Error Connecting to Servers. Please try again or contact Developers.");
                }
                var values = new Dictionary<string, string>

                {
                {"Donation Method",btc.ToString() + "" + PPal.ToString()},
                {"Transaction ID",textBox1.Text},
                {"Discord Name",textBox3.Text}
                };

                


                MessageBox.Show("btc = " + btc.ToString() + " paypal = " + PPal.ToString());
            }
            else
            {
                MessageBox.Show("Please only use this once!", "Notice");
            }
            check = true;
        }
    }
}
