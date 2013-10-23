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
    public class NearbyItemViewModel : INotifyPropertyChanged
    {
        private string _name;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("name");
                }
            }
        }

        private string _line2;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
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

        private double _distance;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                if (value != _distance)
                {
                    _distance = value;
                    NotifyPropertyChanged("distance");
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


        /// <summary>
        /// Label for context menu
        /// </summary>
        /// <returns></returns>
        public string ContextMenuLabel
        {
            get
            {
                if (App.pinned.isPinned(userID))
                {
                    return WebResources.unpin;
                }
                else
                {
                    return WebResources.pin;
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