using System.Reflection;
using System.Windows;

namespace Simplify3DOctoPrintUploader
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void Repo_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/jcphlux/OctoPrintPostProcessorUploader");
        }

        private void Site_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://phluxapps.com");
        }

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://twitter.com/jcphlux");
        }
    }
}
