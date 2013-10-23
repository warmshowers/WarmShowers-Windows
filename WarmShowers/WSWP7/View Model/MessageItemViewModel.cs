using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WSApp.ViewModel
{
    public class MessageItemViewModel : INotifyPropertyChanged
    {
        private string _header1;
        /// <summary>
        /// Header line 1
        /// </summary>
        /// <returns></returns>
        public string Header1
        {
            get
            {
                return _header1;
            }
            set
            {
                if (value != _header1)
                {
                    _header1 = value;
                    NotifyPropertyChanged("header1");
                }
            }
        }

        private string _body;
        /// <summary>
        /// Body of the review
        /// </summary>
        /// <returns></returns>
        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                if (value != _body)
                {
                    _body = value;
                    NotifyPropertyChanged("body");
                }
            }
        }

/*
        private int _threadID;
        /// <summary>
        /// Thread ID so we can load thread after user clicks on message
        /// </summary>
        /// <returns></returns>
        public int threadID
        {
            get
            {
                return _threadID;
            }
            set
            {
                if (value != _threadID)
                {
                    _threadID = value;
                    NotifyPropertyChanged("threadID");
                }
            }
        }
 */
 

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