using System;
using System.ComponentModel;

namespace WSApp.ViewModel
{
    public class FilterViewModel : INotifyPropertyChanged
    {
        public delegate void AlertCallback(string alert);
        private AlertCallback alertCallback;

        public FilterViewModel()
        {
            this.IsDataLoaded = false;
        }

        public void RegisterAlertCallback(AlertCallback func)
        {
            alertCallback += func;
        }

        private int _numberInParty = 1;
        /// <summary>
        /// Show hosts supporting at least this number of guests
        /// </summary>
        /// <returns></returns>
        public int numberInParty
        {
            get
            {
                return _numberInParty;
            }
            set
            {
                if (value != _numberInParty)
                {
                    _numberInParty = value;
 //                   App.filter.numberInParty = value;
                    filteredAlert = filterStatus(); 
                    NotifyPropertyChanged("numberInParty");
                }
            }
        }

        private bool _laundry = false;
        /// <summary>
        /// Show hosts offering laundry
        /// </summary>
        /// <returns></returns>
        public bool laundry
        {
            get
            {
                return _laundry;
            }
            set
            {
                if (value != _laundry)
                {
                    _laundry = value;
  //                  App.filter.laundry = value; 
                    filteredAlert = filterStatus();
                    NotifyPropertyChanged("laundry");
                }
            }
        }

        private bool _bed = false;
        /// <summary>
        /// Show hosts offering bed
        /// </summary>
        /// <returns></returns>
        public bool bed
        {
            get
            {
                return _bed;
            }
            set
            {
                if (value != _bed)
                {
                    _bed = value;
  //                  App.filter.bed = value; 
                    filteredAlert = filterStatus();
                    NotifyPropertyChanged("bed");
                }
            }
        }

        private bool _food = false;
        /// <summary>
        /// Show hosts offering food
        /// </summary>
        /// <returns></returns>
        public bool food
        {
            get
            {
                return _food;
            }
            set
            {
                if (value != _food)
                {
                    _food = value;
  //                  App.filter.food = value;
                    filteredAlert = filterStatus();
                    NotifyPropertyChanged("food");
                }
            }
        }

        private bool _lawn = false;
        /// <summary>
        /// Show hosts offering lawn space
        /// </summary>
        /// <returns></returns>
        public bool lawn
        {
            get
            {
                return _lawn;
            }
            set
            {
                if (value != _lawn)
                {
                    _lawn = value;
  //                  App.filter.lawn = value;
                    filteredAlert = filterStatus();
                    NotifyPropertyChanged("lawn");
                }
            }
        }

        private bool _sag = false;
        /// <summary>
        /// Show hosts offering SAG services
        /// </summary>
        /// <returns></returns>
        public bool sag
        {
            get
            {
                return _sag;
            }
            set
            {
                if (value != _sag)
                {
                    _sag = value;
 //                   App.filter.sag = value;
                    filteredAlert = filterStatus();
                    NotifyPropertyChanged("sag");
                }
            }
        }

        private bool _shower = false;
        /// <summary>
        /// Show hosts offering shower
        /// </summary>
        /// <returns></returns>
        public bool shower
        {
            get
            {
                return _shower;
            }
            set
            {
                if (value != _shower)
                {
                    _shower = value;
  //                  App.filter.shower = value;
                    filteredAlert = filterStatus();
                    NotifyPropertyChanged("shower");
                }
            }
        }

        private bool _kitchen = false;
        /// <summary>
        /// Show hosts offering kitchen
        /// </summary>
        /// <returns></returns>
        public bool kitchen
        {
            get
            {
                return _kitchen;
            }
            set
            {
                if (value != _kitchen)
                {
                    _kitchen = value;
  //                  App.filter.kitchen = value;
                    filteredAlert = filterStatus();
                    NotifyPropertyChanged("kitchen");
                }
            }
        }

        private bool _storage = false;
        /// <summary>
        /// Show hosts offering storage
        /// </summary>
        /// <returns></returns>
        public bool storage
        {
            get
            {
                return _storage;
            }
            set
            {
                if (value != _storage)
                {
                    _storage = value;
  //                  App.filter.storage = value;
                    filteredAlert = filterStatus();
                    NotifyPropertyChanged("storage");
                }
            }
        }

        private string _filteredAlert = "";
        /// <summary>
        /// Show filtered alert
        /// </summary>
        /// <returns></returns>
        public string filteredAlert
        {
            get
            {
                return _filteredAlert;
            }
            set
            {
                if (value != _filteredAlert)
                {
                    _filteredAlert = value;
                    NotifyPropertyChanged("filteredAlert");

                    // Update registered view models with alert
                    if (null != alertCallback)
                    {
                        alertCallback(value);
                    }
                }
            }
        }

        private string filterStatus()
        {
            if (isFiltered())
            {
                return WebResources.Filtered;
            }
            else
            {
                return "";
            }
        }

        public bool isFiltered()
        {
            if (numberInParty > 1) return true;
            if (laundry) return true;
            if (bed) return true;
            if (food) return true;
            if (lawn) return true;
            if (sag) return true;
            if (shower) return true;
            if (kitchen) return true;
            if (storage) return true;

            return false;
        }


        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {

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