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
            //Starts the program
            Stopwatch Count = new Stopwatch();
            Console.Title = "Anomaly Updater";
            Console.SetWindowSize(70, 20);
            Console.WriteLine("");
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Connecting to Github..");
            Console.WriteLine("");
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
                Console.WriteLine("");
                Console.WriteLine("ERROR, Could not Connect to GitHub!");
            }

            try
            {
                Console.WriteLine("SUCCESS, Connected to GitHub");
                Console.WriteLine("");
                //Unzips the Update
                string UpdateURL = "https://github.com/PurityWasHere/Anomaly-Mod-Hosting/raw/master/Update.zip";
                string SavePath = (AppDomain.CurrentDomain.BaseDirectory);
                WebClient WC = new WebClient();
                //Checks if there is a folder names Update.zip
                if (!File.Exists("Update.zip"))
                {
                    Console.WriteLine("Downloading Update...");
                    WC.DownloadFile(UpdateURL, SavePath + "Update.zip");
                    Console.WriteLine("");
                }
                else
                {
                    File.Delete("Update.zip");
                    Console.WriteLine("Previous Update Files Deleted.");
                    Console.WriteLine("");
                    WC.DownloadFile(UpdateURL, SavePath + "Update.zip");
                    Console.WriteLine("Downloading Update...");
                    Console.WriteLine("");
                }
                //Makes sure that the Web Client Downloaded the update
                bool updates = File.Exists("Update.zip");
                if (updates == true)
                {
                    ZipFile zip = ZipFile.Read("Update.zip");
                    {
                        Console.WriteLine("UnZipping Archive...");
                        zip.ExtractAll(SavePath, ExtractExistingFileAction.OverwriteSilently);
                        zip.Dispose();
                    }
                    //Success Messages, Also Deletes files
                    Console.WriteLine("");
                    Count.Stop();
                    string Time = Count.Elapsed.TotalSeconds.ToString();
                    Console.WriteLine("SUCCESS, Update Completed Succesfully in: " + Time + " Seconds");
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
            Console.WriteLine("");
            Console.WriteLine("Press Any key to Close.");
            Console.ReadKey();
        }
    }
}