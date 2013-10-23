using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using WSApp.Utility;
using WSApp.ViewModel;

namespace WSApp.DataModel
{
    [DataContract]
    public class ContactData
    {
        [DataMember]
        public string to { get; set; }
        [DataMember]
        public string subject { get; set; }
        [DataMember]
        public string body { get; set; }

        // Constructor
        public ContactData()
        {
            to = "";
            subject = "";
            body = "";
        }
    }

    [DataContract]
    public class FeedbackData
    {
        [DataMember]
        public string to { get; set; }
        [DataMember]
        public string body { get; set; }
        [DataMember]
        public string experience { get; set; }
        [DataMember]
        public string guestOrHost { get; set; }
        [DataMember]
        public int monthMet { get; set; }
        [DataMember]
        public string yearMet { get; set; }

        // Constructor
        public FeedbackData()
        {
            to = "";
            body = "";
            experience = "";
            guestOrHost = "";
            monthMet = -1;
            yearMet = "";
        }
    }

    [DataContract]
    public class MessageData
    {
        [DataMember]
        public int recipientUid { get; set; }
        [DataMember]
        public ContactData contactData { get; set; }
        [DataMember]
        public FeedbackData feedbackData { get; set; }

        // Constructor
        public MessageData()
        {
            recipientUid = -1;
            contactData = null;     // null means no data to save
            feedbackData = null;    // null means no data to save
        }
    }

    public class ApplicationSettings
    {
        const string storeFilename = "WSSettings";
        Dictionary<int, MessageData> settings;
        public MessageData messageData = null;

        // Constructor
        public ApplicationSettings()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings; 
            bool value;
            string strValue;

            if (settings.TryGetValue("isLocationEnabled", out value))
            {
                _isLocationEnabled = value;
            }
            else
            {
                _isLocationEnabled = true;
            }

            if (settings.TryGetValue("isMetric", out value))
            {
                _isMetric = value;
            }
            else
            {
                _isMetric = false;
            }

            if (settings.TryGetValue("myUsername", out strValue))
            {
                _myUsername = strValue;
            }
            else
            {
                _myUsername = "";
            }

            if (settings.TryGetValue("myPassword", out strValue))
            {
                _myPassword = strValue;
            }
            else
            {
                _myPassword = "";
            }

            _isFirstStartup = false;
            if (("" == myUsername) && ("" == myPassword))
            {
                _isFirstStartup = true;
            }
        }

        public void load()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                IsolatedStorageFileStream stream = store.OpenFile(storeFilename, FileMode.OpenOrCreate);
                if (null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Dictionary<int, MessageData>));
                    settings = (Dictionary<int, MessageData>) ser.ReadObject(stream);

                    stream.Close();
                }

            }
            catch (System.Exception) {};

            if (null == settings)
            {
                settings = new Dictionary<int, MessageData>();
            }
        }

        public void save()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            saveMessage();

            try
            {
                IsolatedStorageFileStream stream = store.OpenFile(storeFilename, FileMode.Truncate);
                if (null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Dictionary<int, MessageData>));
                    ser.WriteObject(stream, settings);
                    stream.Close();
                }
            }
            catch (System.Exception) {};
        }

        public void saveMessage()
        {
            if (null != messageData)
            {
                settings.Remove(messageData.recipientUid);
                if ((null != messageData.contactData) || (null != messageData.feedbackData))
                {   // If we have anything useful in memory, save it
                    settings.Add(messageData.recipientUid, messageData);
                }
                messageData = null;
            }
        }

        public void removeContactMessage(int uId)
        {
            if (null != messageData)
            {
                if (null == messageData.feedbackData)
                {   // Nothing left to save, remove from dictionary
                    settings.Remove(uId);
                    messageData = null;
                }
                else
                {   // Still have feedbackData, put back in dictionary
                    messageData.contactData = null;
                    settings.Remove(uId);
                    settings.Add(uId, messageData);
                }
            }
        }

        public void removeFeedbackMessage(int uId)
        {
            if (null != messageData)
            {
                if (null == messageData.contactData)
                {   // Nothing left to save, remove from dictionary
                    settings.Remove(uId);
                    messageData = null;
                }
                else
                {   // Still have contactData, put back in dictionary
                    messageData.feedbackData = null;
                    settings.Remove(uId);
                    settings.Add(uId, messageData);
                }
            }
        }

        public void loadContactMessage(int uId)
        {
            MessageData retreivedMessageData = null;

            if (!settings.TryGetValue(uId, out retreivedMessageData))
            {
                if (null == messageData)
                {
                    messageData = new MessageData();
                }
            }
            else
            {
                messageData = retreivedMessageData;
            }


            if (null == messageData.contactData)
            {
                messageData.contactData = new ContactData();
            }

            messageData.recipientUid = uId;
        }

        public void loadFeedbackMessage(int uId)
        {
            MessageData retreivedMessageData = null;

            if (!settings.TryGetValue(uId, out retreivedMessageData))
            {
                if (null == messageData)
                {
                    messageData = new MessageData();
                }
            }
            else
            {
                messageData = retreivedMessageData;
            }

            if (null == messageData.feedbackData)
            {
                messageData.feedbackData = new FeedbackData();
            }

            messageData.recipientUid = uId;
        }

        /// <summary>
        /// Is first startup
        /// </summary>
        private bool _isFirstStartup;
        public bool isFirstStartup
        {
            get
            {
                return _isFirstStartup;
            }
            set
            {
                if (value != _isFirstStartup)
                {
                    _isFirstStartup = value;
                }
            }
        }

        /// <summary>
        /// Is location service enabled
        /// </summary>
        private bool _isLocationEnabled = true;
        public bool isLocationEnabled
        {
            get
            {
                return _isLocationEnabled;
            }
            set
            {
                if (value != _isLocationEnabled)
                {
                    _isLocationEnabled = value;
                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                    settings.Remove("isLocationEnabled");
                    settings.Add("isLocationEnabled", _isLocationEnabled);
                }
            }
        }

        /// <summary>
        /// Metric or English settings
        /// </summary>
        private bool _isMetric;
        public bool isMetric
        {
            get
            {
                return _isMetric;
            }
            set
            {
                if (value != _isMetric)
                {
                    _isMetric = value;
                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                    settings.Remove("isMetric");
                    settings.Add("isMetric", _isMetric);
                }
            }
        }

        /// <summary>
        /// Has user successfully logged in
        /// </summary>
        private bool _isAuthenticated;
        public bool isAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
            set
            {
                if (value != _isAuthenticated)
                {
                    _isAuthenticated = value;
                }
            }
        }

        /// <summary>
        /// Username of logged-in user
        /// </summary>
        private string _myUsername;
        public string myUsername
        {
            get
            {
                return _myUsername;
            }
            set
            {
                if (value != _myUsername)
                {
                    _myUsername = value;
                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                    settings.Remove("myUsername");
                    settings.Add("myUsername", _myUsername);
                }
            }
        }

        /// <summary>
        /// Password of logged-in user
        /// </summary>
        private string _myPassword;
        public string myPassword
        {
            get
            {
                return _myPassword;
            }
            set
            {
                if (value != _myPassword)
                {
                    _myPassword = value;
                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
                    settings.Remove("myPassword");
                    settings.Add("myPassword", _myPassword);
                }
            }
        }
    }
}