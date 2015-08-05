using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WSApp
{
    public partial class MessagePage : PhoneApplicationPage
    {
        private ApplicationBarIconButton replyButton = null;
        private int uId;

        public MessagePage()
        {
            InitializeComponent();

            DataContext = App.ViewModelMessage;

            // Create reply button
            replyButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.reply.people.png", UriKind.Relative));
            replyButton.Text = "reply";
            replyButton.Click += ApplicationBarIconButton_Click_Reply;
            ApplicationBar.Buttons.Add(replyButton);

            App.ViewModelMessage.LoadData();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id = NavigationContext.QueryString["id"];
            int.TryParse(id, out uId);

            // Can't get to this if network down, so OK to assume we have profile response
            App.ViewModelMessage.username = App.nearby.host.profile.user_Result.fullname + " (" +
                                            App.nearby.host.profile.user_Result.name + ")";
            App.networkService.IsNetworkAvailable();        // Force update of network status
        }

        private void ApplicationBarIconButton_Click_Reply(object sender, EventArgs e)
        {
            // Todo:  Make this a better test
            if (App.ViewModelMessage.subject == WebResources.loading && App.ViewModelMessage.messageItems.Count != 1)
            {
                ViewModel.MessageItemViewModel vm = App.ViewModelMessage.messageItems[0];
                if (vm.Header1 == WebResources.loading)
                {
                    return;
                }
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            { NavigationService.Navigate(new Uri("/ContactForm.xaml?origin=MessagePage&id=" + uId, UriKind.Relative)); });
        }
    }
}