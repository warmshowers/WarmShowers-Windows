using System;
using System.Windows;
using System.Device.Location;

namespace WSApp.LocationServices
{
    public sealed class LocationService:IDisposable
    {
        // Singleton object
        static readonly LocationService _instance = new LocationService();
        public static LocationService Instance
        {
            get
            { 
                return _instance;
            }
        }

        private GeoCoordinateWatcher watcher;
        private static GeoCoordinate _currentLocation;
        private static string _locationStatus;
        public delegate void AlertCallback(string alert);
        private AlertCallback alertCallback;
        public delegate void UpdateCallback(GeoCoordinate loc);
        private UpdateCallback updateCallback;
        private bool disposed = false; // to detect redundant calls      
    
        // Constructor
        public LocationService()    
        {
            _locationStatus = WebResources.invalid;
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (watcher != null) {
                        watcher.Dispose();
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RegisterAlertCallback(AlertCallback func)
        {
            alertCallback += func;
        }

        public void RegisterUpdateCallback(UpdateCallback func)
        {
            updateCallback += func;
        }

        public void Start()
        {
            if (null == watcher)
            {
                try
                {
                    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);

                    watcher.MovementThreshold = 20; // use MovementThreshold to ignore noise in the signal
                    watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

                    watcher.Start();
                }
                catch { };  // some of app can work without location service
            }
            else
            {
                watcher.Start();
            }
        }

        public void Stop()
        {
            watcher.Stop();
            Deployment.Current.Dispatcher.BeginInvoke(() => { alertCallback(WebResources.NoLocation); });
        }

        // Event handler for the GeoCoordinateWatcher.StatusChanged event.
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.

                    // Check to see whether the user has disabled the Location Service.
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        _locationStatus = WebResources.NoLocation; 
                    }
                    else
                    {
                        _locationStatus = WebResources.NoLocation;
                    }
                    break;

                case GeoPositionStatus.Initializing:
                    // The Location Service is initializing.
                    _locationStatus = WebResources.NoLocation;
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    _locationStatus = WebResources.NoLocation;
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    _locationStatus = "";
                    break;

                default:
                    _locationStatus = WebResources.NoLocation;
                    break;
            }

            // Update registered view models with alert
            Deployment.Current.Dispatcher.BeginInvoke(() => { alertCallback(_locationStatus); });
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            _currentLocation = e.Position.Location;

            updateCallback(_currentLocation);
        }
    }
}

