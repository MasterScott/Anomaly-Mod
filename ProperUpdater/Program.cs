using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace ProperUpdater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            bool updates = File.Exists("Update.zip");
            //Starts the program
            Stopwatch Count = new Stopwatch();
            Console.Title = "Anomaly Updater";
            Console.SetWindowSize(70, 20);
            Console.WriteLine("");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Connecting to Github.." + Environment.NewLine);
            Count.Start();

            //Sets the proper connection so people in other countries can connect. MUST BE CALLED BEFORE CONNECTIONS ARE MADE
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //
            try
            {
                //Checks Clients connection to GitHub
                WebClient Meme = new WebClient();
                Meme.DownloadData("https://github.com/");
            }
            catch (Exception)
            {
                //Checks Git Connection
                Console.WriteLine("ERROR, Could not Connect to GitHub!" + Environment.NewLine);
            }

            try
            {
                Console.WriteLine("SUCCESS, Connected to GitHub" + Environment.NewLine);
                //Unzips the Update
                string UpdateURL = "https://github.com/PurityWasHere/Anomaly-Mod-Hosting/raw/master/Update.zip";
                string SavePath = (AppDomain.CurrentDomain.BaseDirectory);
                WebClient WC = new WebClient();
                //Checks if there is a folder names Update.zip
                if (!updates)
                {
                    Console.WriteLine("Downloading Update..." + Environment.NewLine);
                    WC.DownloadFile(UpdateURL, SavePath + "Update.zip");
                }
                else
                {
                    File.Delete("Update.zip");
                    Console.WriteLine("Previous Update Files Deleted." + Environment.NewLine);
                    WC.DownloadFile(UpdateURL, SavePath + "Update.zip");
                    Console.WriteLine("Downloading Update..." + Environment.NewLine);
                }
                updates = File.Exists("Update.zip");
                //Makes sure that the Web Client Downloaded the update
                if (updates)
                {
                    ZipFile zip = ZipFile.Read("Update.zip");
                    {
                        Console.WriteLine("UnZipping Archive..." + Environment.NewLine);
                        zip.ExtractAll(SavePath, ExtractExistingFileAction.OverwriteSilently);
                        zip.Dispose();
                    }
                    //Success Messages, Also Deletes files
                    Count.Stop();
                    string Time = Count.Elapsed.TotalSeconds.ToString();
                    Console.WriteLine("SUCCESS, Update Completed Succesfully in: " + Time + " Seconds" + Environment.NewLine);
                    File.Delete("Update.zip");
                }
                else
                {
                    //Zip Error
                    Console.WriteLine("Update Files not found or Corrupted! Does Update.zip exist?");
                }
            }
            catch (Exception)
            {
                //Zip Error
                Console.WriteLine("ERROR");
                Console.WriteLine("Error Extracting Archive");
            }
            //ends the application
            Console.WriteLine("Press Any key to Close.");
            Console.ReadKey();
        }
    }
}