using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using WSApp.Utility;

namespace WSApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly CredentialsProvider _credentialsProvider = new ApplicationIdCredentialsProvider(App.Id); 
        public bool isMapInitialized { get; set; }
        public bool isMeButtonPressPending { get; set; }

        public MainViewModel()
        {              
            this.IsDataLoaded = false;
            this.isMapInitialized = false;
            this.isMeButtonPressPending = false;
            this.nearbyItems = new ObservableCollection<NearbyItemViewModel>();
            this.pinnedItems = new ObservableCollection<PinnedItemViewModel>(); 
            this.mapItems = new ObservableCollection<MapItemViewModel>();
            App.networkService.RegisterAlertCallback(new NetworkServices.NetworkService.AlertCallback(networkAlertCallback));
            App.locationService.RegisterAlertCallback(new LocationServices.LocationService.AlertCallback(locationAlertCallback));
            //            App.ViewModelFilter.RegisterAlertCallback(new FilterViewModel.AlertCallback(filteredAlertCallback));  ToDo:  Filter feature
            App.locationService.RegisterUpdateCallback(new LocationServices.LocationService.UpdateCallback(newMeLocation));

            this.nearbyItemsSource = new System.Windows.Data.CollectionViewSource();
            this.nearbyItemsSource.Source = this.nearbyItems;
            this.nearbyItemsSource.SortDescriptions.Add(new SortDescription("Distance", ListSortDirection.Ascending));
            //            this.nearbyItemsSource.View.Filter = new Predicate<object>(Filter);  ToDo:  Filter feature
            this.pinnedItemsSource = new System.Windows.Data.CollectionViewSource();
            this.pinnedItemsSource.Source = this.pinnedItems;
            this.pinnedItemsSource.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
        }

        /// <summary>
        /// Collections for ItemViewModel objects
        /// </summary>
        public ObservableCollection<NearbyItemViewModel> nearbyItems { get; private set; }
        public ObservableCollection<PinnedItemViewModel> pinnedItems { get; private set; }
        public ObservableCollection<MapItemViewModel> mapItems { get; private set; }

        /// <summary>
        /// CollectionViewSource for sorting
        /// </summary>
        public CollectionViewSource nearbyItemsSource { get; set; }
        public CollectionViewSource pinnedItemsSource { get; set; }

        public CredentialsProvider CredentialsProvider
        {   
            get { return _credentialsProvider; }
        }
        
        private bool Filter(object obj)
        {   // ToDo:  Filter feature
            if (((NearbyItemViewModel)obj).Name == WebResources.loading) return true;         
  
            return false;
        }

        private void filteredAlertCallback(string alert)
        {   
            filteredAlert = alert;
        }

        private string _filteredAlert = "";
        /// <summary>
        /// Text to indicate if the nearby list is filtered
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

        private string _pannedAlert = "";
        /// <summary>
        /// Text to indicate if the nearby list is filtered
        /// </summary>
        /// <returns></returns>
        public string pannedAlert
        {
            get
            {
                return _pannedAlert;
            }
            set
            {
                if (value != _pannedAlert)
                {
                    _pannedAlert = value;
                    NotifyPropertyChanged("pannedAlert");
                }
            }
        }

        private string _locationAlert = WebResources.NoLocation;
        /// <summary>
        /// Text to indicate status of current location
        /// </summary>
        /// <returns></returns>
        public string locationAlert
        {
            get
            {
                return _locationAlert;
            }
            set
            {
                if (value != _locationAlert)
                {
                    _locationAlert = value;
                    NotifyPropertyChanged("locationAlert");
                }
            }
        }

        // Receive incoming alert from location service
        private void locationAlertCallback(string alertText)
        {
            locationAlert = alertText;
            if (string.IsNullOrEmpty(alertText))
            {   // We've just come back from GPS outage, reset map center
                mapLocation = App.nearby.meCenter;
                App.nearby.viewportCache.getHosts();
            }
        }

        private string _networkAlert = WebResources.NoNetwork;
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

        // Receive incoming alert from network service
        private void networkAlertCallback(string alertText)
        {
            networkAlert = alertText;
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
            this.nearbyItems.Clear();
            this.nearbyItems.Add(new NearbyItemViewModel() { Name = WebResources.loading });

/*           //  Todo:  Fake names for screen shots
            this.nearbyItems.Add(new NearbyItemViewModel() { Name = "Joseph Wright", Line2 = "0.3 mi", Distance=0.3});
            this.nearbyItems.Add(new NearbyItemViewModel() { Name = "Arnold Banks", Line2="1.2 mi", Distance=1.2});
            this.nearbyItems.Add(new NearbyItemViewModel() { Name = "Jill and Anthony Lake", Line2 = "1.4 mi - pinned", Distance=1.4 });
            this.nearbyItems.Add(new NearbyItemViewModel() { Name = "Zoe Brooks", Line2 = "2.3 mi", Distance=2.3});
            this.nearbyItems.Add(new NearbyItemViewModel() { Name = "Freddie Lambert", Line2 = "3.1 mi - pinned" , Distance=3.1});
            this.nearbyItems.Add(new NearbyItemViewModel() { Name = "Lisa Margaret Jones", Line2 = "4.8 mi", Distance=4.8 });
*/

            // Initialize state of filterAlert
 //           filteredAlert = App.ViewModelFilter.filteredAlert;  ToDo:  Filter feature

            this.IsDataLoaded = true;
        }

        private double _zoom = 11.0;
        /// <summary>
        /// Text to indicate if the nearby list is filtered
        /// </summary>
        /// <returns></returns>
        public double zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                if (value != _zoom)
                {
                    _zoom = value;
                    NotifyPropertyChanged("zoom");
                }
            }
        }

//        private GeoCoordinate _mapLocation = null;
        /// <summary>
        /// Map center
        /// </summary>
        /// <returns></returns>
        public GeoCoordinate mapLocation
        {
            get
            {
                return App.nearby.mapCenter;
            }
            set
            {
                GreatCircle gc = new GreatCircle();
                if (!gc.isEqual(value,  App.nearby.mapCenter))
                {   // User changed map location so no longer centered on me
                    if (!isMeButtonPressPending) App.nearby.isCenteredOnMe = false;
                    App.nearby.mapCenter = value;
                    NotifyPropertyChanged("mapLocation");
                }

            }
        }

        public void newMeLocation()
        {   // This is invoked by location service through UpdateCallback
            if (App.nearby.isCenteredOnMe)
            {   // GPS changed map location so remains centered on me
                App.nearby.mapCenter = App.nearby.meCenter;
                NotifyPropertyChanged("mapLocation");
            }
        }

        public void ClearNewMessageCount()
        {
            foreach (var item in pinnedItems)
	        {
                item.newMessageCount = 0;  
	        }
        }

        /// <summary>
        /// Remove specified user from pinned list
        /// </summary>
        private void RemoveFromPinned(int uId)
        {
            PinnedItemViewModel found = FindItemInPinnedList(uId);
            if (null != found)
            {
                pinnedItems.Remove(found);
            }
        }

        /// <summary>
        /// Add pinned indicator to existing user in nearby list
        /// </summary>
        private void ShowPinnedNearby(int uId)
        {
            NearbyItemViewModel found = FindItemInNearbyList(uId); 
            if (null != found)
            {
                string d = found.Line2;
                if (!d.Contains(WebResources.pinned))    // Don't do it twice
                {
                    d += " - " + WebResources.pinned; 
                    string name = found.Name;
                    int userID = found.userID;
                    double distance = found.Distance;

//                    found.Line2 = d;
                    nearbyItems.Remove(found); 
                    nearbyItems.Add(new NearbyItemViewModel() { Name = name, Line2 = d, userID = userID, Distance = distance });
                }
            }
        }

        /// <summary>
        /// Remove pinned indicator to existing user in nearby list
        /// </summary>
        private void ShowUnpinnedNearby(int uId)
        {
            NearbyItemViewModel found = null;

            foreach (var item in nearbyItems)
            {
                if (item.userID == uId)
                {
                    found = item;
                    break;
                }
            }
            if (null != found)
            {
                string d = found.Line2;
                string name = found.Name;
                int userID = found.userID;
                double distance = found.Distance;

                if (null != found.Line2)    // not sure why we need this...
                {
                    int i = found.Line2.IndexOf(WebResources.pinned);
                    if (i > 2)
                    {
                        d = d.Remove(i - 3);    // strip ' - pinned' from end
                    }
                }

                nearbyItems.Remove(found);
                nearbyItems.Add(new NearbyItemViewModel() { Name = name, Line2 = d, userID = userID, Distance = distance });
            }
        }

        /// <summary>
        /// Change timestamp in pinned list
        /// </summary>
        public void UpdateTimestampPinned(int uId, long time)
        {
            GreatCircle gc = new GreatCircle();

            PinnedItemViewModel found = FindItemInPinnedList(uId);
            if (null != found)
            {
                string name = found.Name;
                int userID = found.userID;

                App.ViewModelMain.pinnedItems.Remove(found);
                App.ViewModelMain.pinnedItems.Add(new PinnedItemViewModel() { Name = name, Time = time, userID = userID });
            }
        }

        /// <summary>
        /// Update UI to reflect new message received
        /// </summary>
        public void SetNewMessageCount(int uId, int count)
        {
            PinnedItemViewModel found = FindItemInPinnedList(uId);
            if (null != found)
            {
                string name = found.Name;
                long time = found.Time;

                App.ViewModelMain.pinnedItems.Remove(found);
                App.ViewModelMain.pinnedItems.Add(new PinnedItemViewModel() { Name = name, Time = time, userID = uId, newMessageCount = count });
            }
         }

        /// <summary>
        /// Update UI to reflect pinned user
        /// </summary>
        public void Pin(int uId, string name, long time)
        {
            // If the only item is the 'no hosts' message, clear it
            if (pinnedItems.Count == 1)
            {
                PinnedItemViewModel vm = pinnedItems[0];
                if (vm.Name == WebResources.PinnedListEmptyHeader)
                {
                    pinnedItems.Clear();
                }
            }

            pinnedItems.Add(new PinnedItemViewModel() { Name = name, Time = time, userID = uId });
            // Update UI to reflect this user is pinned in nearbyList
            ShowPinnedNearby(uId);
        }

        /// <summary>
        /// Update UI to reflect unpinned user
        /// </summary>
        public void unPin(int uId)
        {
            // Remove pinned indicator from nearby list
            ShowUnpinnedNearby(uId);

            // Remove entry altogether from pinned list
            RemoveFromPinned(uId);

            // If we're down to zero pinned hosts, put up the 'no hosts' info text
            if (pinnedItems.Count == 0)
            {
                pinnedItems.Add(new PinnedItemViewModel() { Name = WebResources.PinnedListEmptyHeader, line2 = WebResources.PinnedListEmptyBody + "\n\n" + WebResources.PinnedListEmptyBody2 });
            }
        }

        public NearbyItemViewModel FindItemInNearbyList(int uId)
        {
            NearbyItemViewModel found = null;

            foreach (var item in nearbyItems)
            {
                if (item.userID == uId)
                {
                    found = item;
                    break;
                }
            }

            return found;
        }

        public PinnedItemViewModel FindItemInPinnedList(int uId)
        {
            PinnedItemViewModel found = null;

            foreach (var item in pinnedItems)
            {
                if (item.userID == uId)
                {
                    found = item;
                    break;
                }
            }
            return found;
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