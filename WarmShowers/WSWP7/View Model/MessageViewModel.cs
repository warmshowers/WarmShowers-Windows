using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WSApp.ViewModel
{
    public class MessageViewModel : INotifyPropertyChanged
    {

        public MessageViewModel()
        {
            this.IsDataLoaded = false;
            this.messageItems = new ObservableCollection<MessageItemViewModel>(); 
            App.networkService.RegisterAlertCallback(new NetworkServices.NetworkService.AlertCallback(networkAlertCallback));
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<MessageItemViewModel> messageItems { get; private set; }

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

        // Receiving incoming alert from network service
        private void networkAlertCallback(string alertText)
        {
            networkAlert = alertText;
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

        private string _subject = WebResources.loading;
        public string subject
        {
            get
            {
                return _subject;
            }
            set
            {
                if (value != _subject)
                {
                    _subject = value;
                    NotifyPropertyChanged("subject");
                }
            }
        }
 
        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Initial data, will be replaced with request data

            this.messageItems.Clear();
            // Not currently caching individual messages
            //if (null != App.currentData.messageThread.message_result)
            //{   // We may have messages, load them 
            //    App.loadMessage();
            //}
            //else
            {   // Display loading... while we wait for web request
                this.messageItems.Add(new MessageItemViewModel() { Header1 = WebResources.loading });
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