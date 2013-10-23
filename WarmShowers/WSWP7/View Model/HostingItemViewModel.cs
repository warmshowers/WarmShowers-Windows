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
    public class HostingItemViewModel : INotifyPropertyChanged
    {
        private string _line1;
        /// <summary>
        /// Header line 1
        /// </summary>
        /// <returns></returns>
        public string Line1
        {
            get
            {
                return _line1;
            }
            set
            {
                if (value != _line1)
                {
                    _line1 = value;
                    NotifyPropertyChanged("line1");
                }
            }
        }

        private string _line2;
        /// <summary>
        /// Header line 2
        /// </summary>
        /// <returns></returns>
        public string Line2
        {
            get
            {
                return _line2;
            }
            set
            {
                if (value != _line2)
                {
                    _line2 = value;
                    NotifyPropertyChanged("line2");
                }
            }
        }

        private string _line3;
        /// <summary>
        /// Header line 3
        /// </summary>
        /// <returns></returns>
        public string Line3
        {
            get
            {
                return _line3;
            }
            set
            {
                if (value != _line3)
                {
                    _line3 = value;
                    NotifyPropertyChanged("line3");
                }
            }
        }

        private string _line4;
        /// <summary>
        /// Header line 4
        /// </summary>
        /// <returns></returns>
        public string Line4
        {
            get
            {
                return _line4;
            }
            set
            {
                if (value != _line4)
                {
                    _line4= value;
                    NotifyPropertyChanged("line4");
                }
            }
        }

        private int _userID;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public int userID
        {
            get
            {
                return _userID;
            }
            set
            {
                if (value != _userID)
                {
                    _userID = value;
                    NotifyPropertyChanged("userID");
                }
            }
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