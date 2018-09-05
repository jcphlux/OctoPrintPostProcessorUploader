using RestSharp;
using Simplify3DOctoPrintUploader.Models;
using System;
using System.Net;
using System.Windows;

namespace Simplify3DOctoPrintUploader
{
    public partial class MainWindow : Window
    {
        static OctoPrintArgs opArgs;

        public MainWindow(OctoPrintArgs args)
        {
            opArgs = args;

            InitializeComponent();

            GetState();
        }

        void GetState()
        {
            var client = new RestClient(opArgs.Server);
            var request = new RestRequest("/api/job", Method.GET);
            request.AddHeader("X-Api-Key", opArgs.ApiKey);

            IRestResponse response = client.Execute(request);

            CurrentJob currentJob = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                currentJob = SimpleJson.SimpleJson.DeserializeObject<CurrentJob>(response.Content);
            }

            if (currentJob.state == "Operational")
            {
                upload.IsEnabled = true;
                print.IsEnabled = true;
                remove.IsEnabled = true;
                openOp.IsEnabled = true;
                refresh.Visibility = Visibility.Hidden;
                status.Content = "OctoPrint is ready to start printing.";
            }
            else if (currentJob.state == "Printing")
            {
                upload.IsEnabled = true;
                print.IsEnabled = false;
                remove.IsEnabled = true;
                openOp.IsEnabled = true;
                refresh.Visibility = Visibility.Visible;
                status.Content = "OctoPrint is currently printing.";
            }
            else if (currentJob.state == "Cancelling")
            {
                upload.IsEnabled = true;
                print.IsEnabled = false;
                remove.IsEnabled = true;
                openOp.IsEnabled = true;
                refresh.Visibility = Visibility.Visible;
                status.Content = "OctoPrint is canceling a print.";
            }
            else
            {
                status.Content = "OctoPrint is offline or there is a invalid parameter.";
            }
        }

        void PushFile(bool startPrint)
        {
            upload.IsEnabled = false;
            print.IsEnabled = false;
            remove.IsEnabled = false;
            openOp.IsEnabled = false;
            refresh.Visibility = Visibility.Hidden;

            var client = new RestClient(opArgs.Server);
            var request = new RestRequest("/api/files/local", Method.POST);
            request.AlwaysMultipartFormData = true;
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("X-Api-Key", opArgs.ApiKey);
            request.AddFile("file", opArgs.Path + "\\" + opArgs.File, "application/octet-stream");

            if (startPrint)
            {
                request.AddParameter("print", "true");
            }

            IRestResponse response = client.Execute(request);
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    Exit(true);
                    return;
                case HttpStatusCode.BadRequest:
                    status.Content = "Invalid Request.";
                    break;
                case HttpStatusCode.NotFound:
                    status.Content = "Location not found.";
                    break;
                case HttpStatusCode.Conflict:
                    status.Content = "Same file name as the file that OctoPrint is using.";
                    break;
                case HttpStatusCode.UnsupportedMediaType:
                    status.Content = "Invalid gcode in file.";
                    break;
                case HttpStatusCode.InternalServerError:
                default:
                    status.Content = "Upload to OctoPrint failed.";
                    break;
            }
            refresh.Visibility = Visibility.Visible;
        }

        void Exit(bool open)
        {
            if (open && openOp.IsChecked == true)
            {
                System.Diagnostics.Process.Start(opArgs.Server);
            }

            if (remove.IsChecked == true)
            {
                System.IO.File.Delete(opArgs.Path + "\\" + opArgs.File);
            }
            Environment.Exit(0);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetState();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();

            about.Show();
        }
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            PushFile(false);
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PushFile(true);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Exit(false);
        }
    }
}