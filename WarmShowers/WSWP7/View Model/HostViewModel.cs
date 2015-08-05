using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WSApp.ViewModel
{
    public class HostViewModel : INotifyPropertyChanged
    {

        public HostViewModel()
        {
            this.IsDataLoaded = false;
            this.feedbackItems = new ObservableCollection<FeedbackItemViewModel>();
            this.messageItems = new ObservableCollection<MessagesItemViewModel>();
            this.aboutItems = new ObservableCollection<AboutItemViewModel>();
            this.hostingItems = new ObservableCollection<HostingItemViewModel>();
            App.networkService.RegisterAlertCallback(new NetworkServices.NetworkService.AlertCallback(networkAlertCallback));
        }


        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<FeedbackItemViewModel> feedbackItems { get; private set; }
        public ObservableCollection<MessagesItemViewModel> messageItems { get; private set; }
        public ObservableCollection<AboutItemViewModel> aboutItems { get; private set; }
        public ObservableCollection<HostingItemViewModel> hostingItems { get; private set; }

        private string _networkAlert = "";
        /// <summary>
        /// Text to indicate status of network
        /// </summary>
        /// <returns></returns>
        public string networkAlert
        {
            get
            {
                return _networkAlert;
            }
            set
            {
                if (value != _networkAlert)
                {
                    _networkAlert = value;
                    NotifyPropertyChanged("networkAlert");
                }
            }
        }

        // Receiving incoming alert from network service
        private void networkAlertCallback(string alertText)
        {
            networkAlert = alertText;
        }

        private string _loginAlert = "";
        /// <summary>
        /// Text to indicate if when user is not logged in
        /// </summary>
        /// <returns></returns>
        public string loginAlert
        {
            get
            {
                return _loginAlert;
            }
            set
            {
                if (value != _loginAlert)
                {
                    _loginAlert = value;
                    NotifyPropertyChanged("loginAlert");
                }
            }
        }

        private string _username = WebResources.loading;
        public string username
        {
            get
            {
                return _username;
            }
            set
            {
                if (value != _username)
                {
                    _username = value;
                    NotifyPropertyChanged("username");
                }
            }
        }

        private int _uId = 0;
        public int uId
        {
            get
            {
                return _uId;
            }
            set
            {
                if (value != _uId)
                {
                    _uId = value;
                    NotifyPropertyChanged("uId");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Add pinned indicator in username
        /// </summary>
        public void Pin(int uIdPin)
        {
            if (uIdPin == uId)  // Make sure we're pinning the right user
            {
                string uName = username;
                int i = uName.IndexOf(WebResources.pinned);
                if (i < 3)  // Don't pin more than once
                {
                    username = uName + " - " + WebResources.pinned;
                }
            }
        }

        /// <summary>
        /// Remove pinned indicator in username
        /// </summary>
        public void unPin(int uId)
        {
            string d = username;
 
            int i = d.IndexOf(WebResources.pinned);
            if (i > 2)
            {
                d = d.Remove(i - 3);    // strip ' - pinned' from end
            }
            username = d;
        }


        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Initial data, will be replaced with request data

            this.aboutItems.Clear();
            this.aboutItems.Add(new AboutItemViewModel() { Line1 = WebResources.loading, Type = AboutItemViewModel.AboutType.general });

            if (null != App.nearby.hosts.hosts_Result)
            {   // Display any data that came down with Hosts request or loading... while we wait for web request
                foreach (var user in App.nearby.hosts.hosts_Result.accounts)
                {
                    if (user.uid == uId)
                    {
                        App.nearby.loadProfileCommon(user.latitude, user.longitude, user.street, user.additional, user.city, user.province, user.country, user.postal_code);
                        break;
                    }
                }
            }

            this.hostingItems.Clear();
            this.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.loading });

            if (null != App.nearby.host.profile.user_Result)
            {   // We may have current profile cached, try to load it
                App.nearby.loadProfile();
            }

            // Display loading... while we wait for web request
            this.feedbackItems.Clear(); 
            this.feedbackItems.Add(new FeedbackItemViewModel() { Header1 = WebResources.loading, Header2 = "", Body = "" });

            if (null != App.nearby.host.feedback.recommendations_Result)
            {   // We may have current feedback cached, try to load it
                App.nearby.loadFeedback();
            }

            // Display loading... while we wait for web request
            this.messageItems.Clear();
            this.messageItems.Add(new MessagesItemViewModel() { Header1 = WebResources.loading, Header2 = "" });

            if (null != App.nearby.host.messages.messages_result)
            {   // We may have messages cached, try to load them 
                App.nearby.loadMessages();
            }

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}