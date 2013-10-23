using System;
using System.Windows;
using System.ComponentModel;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;


namespace WSApp
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            FacebookFollow.Text = WebResources.FacebookFollow;
            TwitterFollow.Text = WebResources.TwitterFollow;
            Description.Text = WebResources.AboutDescription;
            DevelopedBy.Text = WebResources.AboutDevelopedBy;
            Username.Text = WebResources.LoggedInAs + " " + App.settings.myUsername;
            ContactMe.Text = WebResources.AboutContactMe;
            Memory.Visibility = System.Windows.Visibility.Collapsed;    // Hide memory info for public release
            Version.Text = WebResources.Version + " " + XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value + ".";
            Privacy.Text = "\n" + WebResources.PrivacyPolicy;
            if (App.settings.isLocationEnabled)
            {
                LocationOnOff.Content = WebResources.DisableLocationService;
            }
            else
            {
                LocationOnOff.Content = WebResources.EnableLocationService;
            }
 /*           Memory.Text = WebResources.Memory + ":    " + DeviceStatus.ApplicationCurrentMemoryUsage / 1024 / 1024 + " MB " + WebResources.used + "    " +
                                                          DeviceStatus.ApplicationPeakMemoryUsage / 1024 / 1024 + " MB " + WebResources.peak + "    " +
                                                          DeviceStatus.ApplicationMemoryUsageLimit / 1024 / 1024 + " MB " + WebResources.limit;  */
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            App.nearby.AboutPageActive = false;
            e.Cancel = false;
        }

        private void Banner_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(WebResources.uriPrefix + WebResources.WarmShowersUri);
            task.Show();
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(WebResources.FacebookUri);
            task.Show();
        }

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(WebResources.TwitterUri);
            task.Show();
        }

        private void LocationOnOff_Click(object sender, RoutedEventArgs e)
        {
            if (App.settings.isLocationEnabled)
            {
                App.settings.isLocationEnabled = false;
                App.locationService.Stop();
                LocationOnOff.Content = WebResources.EnableLocationService;
            }
            else
            {
                App.settings.isLocationEnabled = true;
                App.locationService.Start();
                LocationOnOff.Content = WebResources.DisableLocationService;
            }
        }
    }
}