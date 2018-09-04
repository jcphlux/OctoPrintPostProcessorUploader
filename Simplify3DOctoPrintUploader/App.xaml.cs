using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Simplify3DOctoPrintUploader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            if (e.Args.Length > 0)
            {
                var opArgs = new OctoPrintArgs();

                foreach (var arg in e.Args)
                {
                    var pair = arg.Split('=');

                    if (pair.Length != 2)
                    {
                        InvalidArgs();
                        return;
                    }

                    switch(pair[0]){
                        case "-p":
                            opArgs.Path = pair[1];
                            break;
                        case "-f":
                            opArgs.File = pair[1];
                            break;
                        case "-s":
                            opArgs.Server = pair[1];
                            break;
                        case "-a":
                            opArgs.ApiKey = pair[1];
                            break;
                        default:
                            InvalidArgs();
                            return;
                    }
                }

                MainWindow wnd = new MainWindow(opArgs);

                wnd.Show();
            }
            else
            {
                InvalidArgs();
            }           
        }

        private void InvalidArgs()
        {
            MessageBox.Show("Please pass in valid Arguments.  \n\n Commands: \n -p={path} \n -f={filename} \n -s={OctoPrintServer} \n -a={APIKey}", "OctoPrint Post Processor Uploader", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }

    }
}
