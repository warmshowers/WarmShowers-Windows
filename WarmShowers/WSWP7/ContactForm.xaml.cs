using System;
using System.Windows;
using System.Windows.Navigation;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using WSApp.DataModel;
using WSApp.Utility;

namespace WSApp
{
    public partial class ContactForm : PhoneApplicationPage
    {      
        String origin;  // The page that invoked the contact form
        int uId = -1;

        public ContactForm()
        {
            InitializeComponent();

            Status.Text = "";

            App.networkService.RegisterAlertCallback(new NetworkServices.NetworkService.AlertCallback(networkAlertCallback));
            App.webService.RegisterSendMessageSuccessCallback(SendMessageSuccess);
            App.webService.RegisterSendMessageFailCallback(SendMessageFail);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
           
            origin = NavigationContext.QueryString["origin"];
            String id = NavigationContext.QueryString["id"];
            int.TryParse(id, out uId);

            if (null == App.nearby.host.profile.user_Result)
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
                To.Text = App.nearby.host.profile.user_Result.name;
            }

            App.networkService.IsNetworkAvailable();        // Force update of network status

            prepareForm();
        }

        private void prepareForm()
        {
            String subject;
            String body;

            App.settings.loadContactMessage(uId);
            ContactData contactData = App.settings.messageData.contactData;

            if ("HostPage" == origin)
            {   // This is a new message

                subject = WebResources.ContactSubject;
                body = createSalutation();
                if (null != contactData)
                {
                    if ("" != contactData.subject) subject = contactData.subject;
                    if ("" != contactData.body) body = contactData.body;
                }
            }
            else
            {   // This is a reply message
                subject = App.ViewModelMessage.subject;
                body = "";
                Subject.IsReadOnly = true;  // Can't change subject in reply to existing thread
                if (null != contactData)
                {
                    if ("" != contactData.body) body = contactData.body;
                }
            }

            contactData.to = To.Text;
            To.IsReadOnly = true;   // Don't allow user to change recipient because it messes up the persistence  Todo: implement user search feature
            Subject.Text = subject;
            Body.Text = body;
            App.networkService.IsNetworkAvailable();        // Force update of network status
        }

        // Receiving incoming alert from network service
        private void networkAlertCallback(string alertText)
        {
            NetworkAlert.Text = alertText;
        }

        private string createSalutation()
        {
            return WebResources.Salutation + " " + App.nearby.getFullName(uId) + ",\n";
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {   // Refresh to reflect new message
            if ("MessagePage" == origin)
            {
                if (null != App.nearby.messageThread.message_result)
                {
                    WebService.GetMessageThread(App.nearby.messageThread.message_result.pmtid);
                }
            }
            WebService.GetMessages(false);

            // Save message
            App.settings.saveMessage();

            e.Cancel = false;
        }


        private void ApplicationBarIconButton_Click_Reply(object sender, EventArgs e)
        {
            this.Focus();  // Hide keyboard

            ContactData contactData = App.settings.messageData.contactData;
            
            // Catch anything that wasn't edited
            contactData.to = To.Text;
            contactData.subject = Subject.Text;
            contactData.body = Body.Text;

            if (contactData.subject == "")
            {
                Status.Text = WebResources.SubjectBlank;
                return;
            }
            else if (contactData.body == "")
            {
                Status.Text = WebResources.BodyBlank;
                return;
            }
            else if (contactData.body == "\n")
            {
                Status.Text = WebResources.BodyBlank;
                return;
            }
            else if (contactData.body == createSalutation())
            {
                Status.Text = WebResources.BodyBlank;
                return;
            }

            bool success;
            if ("HostPage" == origin)
            {
                success = WebService.SendMessage();
            }
            else
            {
                success = WebService.ReplyMessage();
            }

            if (success)
            {
                if ("HostPage" == origin)
                {
                    Status.Text = WebResources.SendingMessage;
                }
                else
                {
                    Status.Text = WebResources.SendingReply;
                }
            }
            else
            {                
                Status.Text = WebResources.SendingMessageFailed;
            }

            App.pinned.pin();
        }

        private void Subject_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ContactData contactData = App.settings.messageData.contactData;
            if (null == contactData)
            {
                contactData = new ContactData();
            }
            App.settings.messageData.contactData.subject = Subject.Text;
        }

        private void Body_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ContactData contactData = App.settings.messageData.contactData;
            if (null == contactData)
            {
                contactData = new ContactData();
            }
            App.settings.messageData.contactData.body = Body.Text;
        }

        private void SendMessageSuccess()
        {
            if ("HostPage" == origin)
            {   // This is a new message           
                Status.Text = WebResources.SendingMessageSucceeded;
            }
            else
            {   // This is a reply message
                Status.Text = WebResources.SendingReplySucceeded;
            }
            Subject.Text = "";
            Body.Text = "";

            App.settings.removeContactMessage(uId);    // No need to keep message data around anymore
            prepareForm();  // Lands back on blank form, so set it up in unlikely event user wants to send another message
        }

        private void SendMessageFail()
        {
            Status.Text = WebResources.SendingMessageFailed;
        }
    }
}