using System;
using System.ComponentModel;
using WSApp.Utility;

namespace WSApp.ViewModel
{
    public class FoundItemViewModel : INotifyPropertyChanged
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
        public string line2
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

        private long _time;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public long Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (value != _time)
                {
                    _time = value;
                    NotifyPropertyChanged("time");

                    UpdateLine2();
               }
            }
        }

        private int _newMessageCount;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public int newMessageCount
        {
            get
            {
                return _newMessageCount;
            }
            set
            {
                if (value != _newMessageCount)
                {
                    _newMessageCount = value;
                    NotifyPropertyChanged("newMessageCount");

                    UpdateLine2();
                }
            }
        }

        private void UpdateLine2()
        {
            GreatCircle gc = new GreatCircle();

//  return;  // Todo:  Fake names for screen shots
            
            string line = gc.date_mmmddyyyy(_time);
            if (_newMessageCount > 0)
            {
                line += " " + _newMessageCount.ToString() + " ";

                if (_newMessageCount == 1)
                {
                     line += WebResources.NewMessage;
                }
                else
                {
                    line += WebResources.NewMessages;
                }
            }
            line2 = line;
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
        /// Label for center on host context menu
        /// </summary>
        /// <returns></returns>
        public string CenterHostContextMenuLabel
        {
            get
            {
                return WebResources.centerHostContextMenuLabel;
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