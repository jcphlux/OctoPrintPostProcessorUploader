using System;
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

                    switch (pair[0])
                    {
                        case "-f":
                            var index = pair[1].LastIndexOf('\\');
                            opArgs.Path = pair[1].Substring(0, index);
                            opArgs.File = pair[1].Substring(index + 1);
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
            About about = new About();

            about.Show();
            MessageBox.Show("Please pass in valid Arguments.  \n\n Commands: \n -f=[output_filepath] \n -s=[OctoPrintServer] \n -a=[APIKey]", "Simplify3D OctoPrint Uploader", MessageBoxButton.OK, MessageBoxImage.Error);
            //Environment.Exit(0);
        }

    }
}
