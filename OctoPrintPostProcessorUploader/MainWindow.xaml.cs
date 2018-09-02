using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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

            if(state == "Operational")
            {

            }
            else if (state == "Printing")
            {

            }
            else
            {

            }

        }

        string GetState()
        {
            var client = new RestClient(opArgs.Server);
            var request = new RestRequest("/api/job", Method.GET);
            request.AddHeader("X-Api-Key", opArgs.ApiKey);

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JObject.Parse(response.Content)["state"].ToString() ;

            }

            return null;

        }
    }
}