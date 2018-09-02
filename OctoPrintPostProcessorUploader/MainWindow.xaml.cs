using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Windows;

namespace OctoPrintPostProcessorUploader
{
    public partial class MainWindow : Window
    {
        static OctoPrintArgs opArgs;

        public MainWindow(OctoPrintArgs args)
        {
            opArgs = args;

            InitializeComponent();            

            var state = GetState();

            if (state == "Operational")
            {
                upload.IsEnabled = true;
                print.IsEnabled = true;
                remove.IsEnabled = true;
                status.Content = "OctoPrint is ready to start printing.";
            }
            else if (state == "Printing")
            {
                upload.IsEnabled = true;
                print.IsEnabled = false;
                remove.IsEnabled = true;
                status.Content = "OctoPrint is currently printing.";
            }
            else
            {
                status.Content = "OctoPrint is offline or there is a invalid parameter.";
            }
        }

        string GetState()
        {
            var client = new RestClient(opArgs.Server);
            var request = new RestRequest("/api/job", Method.GET);
            request.AddHeader("X-Api-Key", opArgs.ApiKey);

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {               
                return JObject.Parse(response.Content)["state"].ToString();
            }
            
            return null;
        }

        void PushFile(bool startPrint)
        {
            upload.IsEnabled = false;
            print.IsEnabled = false;
            remove.IsEnabled = false;

            var client2 = new RestClient(opArgs.Server);
            var request = new RestRequest("/api/files/local", Method.POST);
            request.AlwaysMultipartFormData = true;
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("X-Api-Key", opArgs.ApiKey);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("file", opArgs.Path + "\\"+ opArgs.File, "application/octet-stream");

            if (startPrint)
            {
                request.AddParameter("print", "true");
            }
            
            IRestResponse response = client2.Execute(request);
            switch (response.StatusCode)
            { 
                case HttpStatusCode.Created:
                    Exit(true);
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.UnsupportedMediaType:
                case HttpStatusCode.InternalServerError:
                default:
                    status.Content = "Upload to OctoPrint failed.";
                    break;
            }
        }

        void Exit(bool OpenOp)
        {
            if (OpenOp)
            {
                System.Diagnostics.Process.Start(opArgs.Server);
            }
            Environment.Exit(0);
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