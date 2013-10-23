using System;
using System.Windows;
using System.Windows.Navigation;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using WSApp.DataModel;
using WSApp.Utility;


namespace WSApp
{
    public partial class FeedbackForm : PhoneApplicationPage
    {
        int uId = -1;

        public FeedbackForm()
        {
            InitializeComponent();

            Status.Text = "";

            App.networkService.RegisterAlertCallback(new NetworkServices.NetworkService.AlertCallback(networkAlertCallback));
            App.webService.RegisterSendFeedbackSuccessCallback(SendFeedbackSuccess);
            App.webService.RegisterSendFeedbackFailCallback(SendFeedbackFail);

            GuestOrHost.Items.Add(WebResources.ItemGuest);
            GuestOrHost.Items.Add(WebResources.ItemHost);
            GuestOrHost.Items.Add(WebResources.ItemTraveling);
            GuestOrHost.Items.Add(WebResources.ItemOther);

            Experience.Items.Add(WebResources.ItemPositive);
            Experience.Items.Add(WebResources.ItemNeutral);
            Experience.Items.Add(WebResources.ItemNegative);

            MonthMet.Items.Add(WebResources.Jan); 
            MonthMet.Items.Add(WebResources.Feb); 
            MonthMet.Items.Add(WebResources.Mar);
            MonthMet.Items.Add(WebResources.Apr);
            MonthMet.Items.Add(WebResources.May);
            MonthMet.Items.Add(WebResources.Jun);
            MonthMet.Items.Add(WebResources.Jul);
            MonthMet.Items.Add(WebResources.Aug);
            MonthMet.Items.Add(WebResources.Sep);
            MonthMet.Items.Add(WebResources.Oct);
            MonthMet.Items.Add(WebResources.Nov);
            MonthMet.Items.Add(WebResources.Dec);

            int thisYear = DateTime.Now.Year;
            for (int year = thisYear - 10; year <= thisYear; year++)
            {
                YearMet.Items.Add(year.ToString());
            }
       }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String id = NavigationContext.QueryString["id"];
            int.TryParse(id, out uId);

            if (null == App.nearby.host.profile.users_Result)
            {   // If we have the hosts cached but didn't get the profile response back, all we have is the user id, which isn't good enough to send a message
                if (!String.IsNullOrEmpty(App.settings.myUsername) && !String.IsNullOrEmpty(App.settings.myPassword))
                {
                    To.Text = WebResources.ToNoNetwork;
                    Status.Text = WebResources.StatusNoNetwork;
                    MessageBox.Show(WebResources.AlertMessageFormNoNetwork, WebResources.ToNoNetwork, MessageBoxButton.OK);
                }
                else
                {
                    To.Text = WebResources.ToNotLoggedIn;
                    Status.Text = WebResources.StatusNoNetwork;
                    MessageBox.Show(WebResources.AlertMessageFormNotLoggedIn, WebResources.ToNotLoggedIn, MessageBoxButton.OK);
                }
            }
            else
            {
                To.Text = App.nearby.host.profile.users_Result.users[0].user.name;
            } 
           
            App.networkService.IsNetworkAvailable();        // Force update of network status

            prepareForm();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {   // Refresh to reflect new message
            WebService.GetFeedback(uId);

            // Save message
            App.settings.saveMessage();

            e.Cancel = false;
        }

        private void prepareForm()
        {
            String body = "";
            String experience = WebResources.ItemPositive;
            String guestOrHost = WebResources.ItemHost;
            int monthMet = DateTime.Now.Month - 1;  // zero-based index
            String yearMet = DateTime.Now.Year.ToString();

            App.settings.loadFeedbackMessage(uId);
            FeedbackData feedbackData = App.settings.messageData.feedbackData;

            if (null != feedbackData)
            {
                feedbackData.to = To.Text;
                body = feedbackData.body;
                if ("" != feedbackData.experience) experience = feedbackData.experience;
                if ("" != feedbackData.guestOrHost) guestOrHost = feedbackData.guestOrHost;
                if (-1 != feedbackData.monthMet) monthMet = feedbackData.monthMet;
                if ("" != feedbackData.yearMet) yearMet = feedbackData.yearMet;
            }

            Body.Text = body;
            Experience.SelectedItem = experience;
            GuestOrHost.SelectedItem = guestOrHost;
            MonthMet.SelectedIndex = monthMet;
            YearMet.SelectedItem = yearMet;

            App.networkService.IsNetworkAvailable();        // Force update of network status
        }

        // Receiving incoming alert from network service
        private void networkAlertCallback(string alertText)
        {
            NetworkAlert.Text= alertText;
        }

        private void ApplicationBarIconButton_Click_Reply(object sender, EventArgs e)
        {
            this.Focus();  // Hide keyboard

            FeedbackData feedbackData = App.settings.messageData.feedbackData;

            // Catch anything that wasn't edited
            feedbackData.to = To.Text;
            feedbackData.body = Body.Text;
            feedbackData.experience = (String) Experience.SelectedItem;
            feedbackData.guestOrHost = (String) GuestOrHost.SelectedItem;
            feedbackData.monthMet = MonthMet.SelectedIndex;
            feedbackData.yearMet = (String) YearMet.SelectedItem;

            GreatCircle gc = new GreatCircle();
            if (gc.GetWordCount(feedbackData.body) < 10)
            {
                Status.Text = WebResources.SendingFeedbackTooShort;
                return;
            }

            if (WebService.SendFeedback())
            {
                Status.Text = WebResources.SendingFeedback;
            }
            else
            {
                Status.Text = WebResources.SendingFeedbackFailed;
            }
            
            App.pinned.pin();
        }

        private void SendFeedbackSuccess()
        {
            Status.Text = WebResources.SendingFeedbackSucceeded;
            Body.Text = "";

            App.settings.removeFeedbackMessage(uId);    // No need to keep message data around anymore
            prepareForm();  // Lands back on blank form, so set it up in unlikely event user wants to send another message
        }

        private void SendFeedbackFail()
        {
            Status.Text = WebResources.SendingFeedbackFailed;
        }

        private void GuestOrHost_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (null != App.settings.messageData)
            {
                FeedbackData feedbackData = App.settings.messageData.feedbackData;
                if (null == feedbackData)
                {
                    feedbackData = new FeedbackData();
                }
                App.settings.messageData.feedbackData.guestOrHost = (String)GuestOrHost.SelectedItem;
            }
        }

        private void MonthMet_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (null != App.settings.messageData)
            {
                FeedbackData feedbackData = App.settings.messageData.feedbackData;
                if (null == feedbackData)
                {
                    feedbackData = new FeedbackData();
                }
                App.settings.messageData.feedbackData.monthMet = MonthMet.SelectedIndex;
            }
        }

        private void YearMet_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (null != App.settings.messageData)
            {
                FeedbackData feedbackData = App.settings.messageData.feedbackData;
                if (null == feedbackData)
                {
                    feedbackData = new FeedbackData();
                }
                App.settings.messageData.feedbackData.yearMet = (String)YearMet.SelectedItem;
            }
        }

        private void Experience_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (null != App.settings.messageData)
            {
                FeedbackData feedbackData = App.settings.messageData.feedbackData;
                if (null == feedbackData)
                {
                    feedbackData = new FeedbackData();
                }
                App.settings.messageData.feedbackData.experience = (String)Experience.SelectedItem;
            }
        }

        private void Body_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (null != App.settings.messageData)
            {
                FeedbackData feedbackData = App.settings.messageData.feedbackData;
                if (null == feedbackData)
                {
                    feedbackData = new FeedbackData();
                }
                App.settings.messageData.feedbackData.body = (String)Body.Text;
            }
        }
    }
}