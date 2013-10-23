using System;
using System.ComponentModel;
using System.Device.Location;

namespace WSApp.ViewModel
{
    public enum PushpinType
    {
        selected,
        pinned,
        unpinned
    }

    public class MapItemViewModel : INotifyPropertyChanged
    {
        private GeoCoordinate _location;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public GeoCoordinate location
        {
            get
            {
                return _location;
            }
            set
            {
                if (value != _location)
                {
                    _location = value;
                    NotifyPropertyChanged("location");
                }
            }
        }

        private string _username;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
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


        private PushpinType _type;
        /// <summary>
        /// Identify the style for this pushpin
        /// </summary>
        /// <returns></returns>
        public PushpinType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    _type = value;
                    NotifyPropertyChanged("type");
                    switch (_type)
                    {
                        case PushpinType.selected:
                            imagePath = "/Images/Pushpins/pinned.png";
                            break;
                        case PushpinType.pinned:
                            imagePath =  "/Images/Pushpins/pinned.png";
                            break;
                        case PushpinType.unpinned:
                        default: 
                            imagePath = "/Images/Pushpins/unpinned.png";
                            break;
                    }
                }
            }
        }

        private string _imagePath;
        /// <summary>
        /// Identify the style for this pushpin
        /// </summary>
        /// <returns></returns>
        public string imagePath
        {
            get
            {
                 return _imagePath;
            }
            set
            {
                if (value != _imagePath)
                {
                    _imagePath = value;
                    NotifyPropertyChanged("imagePath");
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