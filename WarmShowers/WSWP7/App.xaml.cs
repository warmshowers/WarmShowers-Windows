using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WSApp.LocationServices;
using WSApp.NetworkServices;
using WSApp.DataModel;
using WSApp.ViewModel;


namespace WSApp
{
    public partial class App : Application
    {

        internal const string Id = APIKeys.bingMapsToken;
        private static MainViewModel viewModelMain = null;
        private static HostViewModel viewModelHost = null;
        private static MessageViewModel viewModelMessage = null; 
        private static FilterViewModel viewModelFilter = null;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModelMain
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModelMain == null)
                    viewModelMain = new MainViewModel();

                return viewModelMain;
            }
        }

        public static HostViewModel ViewModelHost
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModelHost == null)
                    viewModelHost = new HostViewModel();

                return viewModelHost;
            }
        }

        public static MessageViewModel ViewModelMessage
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModelMessage == null)
                    viewModelMessage = new MessageViewModel();

                return viewModelMessage;
            }
        }

    
        public static FilterViewModel ViewModelFilter
        {
            get
            {
                // Gets created during the initial load
 //               if (viewModelFilter == null)
 //                   viewModelFilter = new FilterViewModel();

                return viewModelFilter;
            }
            set
            {
                viewModelFilter = value;
            }
        }

/*
Todo:  Implement filter
        // Instantiate filter    
        private static Filter _filter = Filter.Instance;
        public static Filter filter
        {
            get
            {
                return _filter;
            }

            set
            {
                _filter = value;
            }
        }        
*/
        // Instantiate location service    
        private static LocationService _locationService = LocationService.Instance;
        public static LocationService locationService
        {
            get
            {
                return _locationService;
            }
        }        

        // Instantiate network service
        private static NetworkService _networkService = NetworkService.Instance;
        public static NetworkService networkService
        {
            get
            {
                return _networkService;
            }
        }

        // Instantiate web service
        private static WebService _webService = WebService.Instance;
        public static WebService webService
        {
            get
            {
                return _webService;
            }
        }

        // Provide access to the data model
        public static WSApp.DataModel.NearbyData nearby { get; set; }
        public static WSApp.DataModel.PinnedData pinned { get; set; }
        public static WSApp.DataModel.FoundData found { get; set; }
        public static WSApp.DataModel.ApplicationSettings settings { get; private set; }
    
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            // Instantiate the data model
            nearby = new WSApp.DataModel.NearbyData();
            pinned = new WSApp.DataModel.PinnedData();
            found = new WSApp.DataModel.FoundData();
            settings = new WSApp.DataModel.ApplicationSettings();

            PinnedStore.load();          // PinnedStore load must precede NearbyStore load
            FoundStore.load();
            NearbyStore.load();
            settings.load();

            // Start services
            if (settings.isLocationEnabled) locationService.Start();
            networkService.Start();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            TiltEffect.SetIsTiltEnabled(RootFrame, true);

            // Begin requesting hosts
            WebService.Login();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // Ensure that application state is restored appropriately
            if (!App.ViewModelMain.IsDataLoaded)
            {
                App.ViewModelMain.LoadData();
            }

            // Todo:  restore other view models???

            TiltEffect.SetIsTiltEnabled(RootFrame, true);

            // Begin requesting hosts
            WebService.Login();

        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            settings.save();
            NearbyStore.save();
            PinnedStore.save();
            FoundStore.save();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Ensure that required application state is persisted here.
            settings.save();
            NearbyStore.save();
            PinnedStore.save();
            FoundStore.save();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}