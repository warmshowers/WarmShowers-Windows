using System;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Shell; 
using System.Device.Location;
using Microsoft.Phone.Tasks;
using WSApp.DataModel;
using WSApp.ViewModel;

enum pivotPage { nearby, pinned, found };

enum LastVisiblePage
{
    mainpivot,
    mappage,
    startup
}

namespace WSApp
{
    public partial class MainPage : PhoneApplicationPage
    {
//        private ApplicationBarIconButton filterButton = null;
        private ApplicationBarIconButton meButton = null;
        private ApplicationBarIconButton mapButton = null; 
        private ApplicationBarIconButton searchButton = null;
        private ApplicationBarIconButton updateButton = null;
        private ApplicationBarIconButton allButton = null;
        private ApplicationBarMenuItem webSiteMenu = null;
        private ApplicationBarMenuItem aboutMenu = null;
        private ApplicationBarMenuItem unitsMenu = null;
        private ApplicationBarMenuItem logoutMenu = null;
        private ApplicationBarMenuItem aerialViewMenu = null;
        private ApplicationBarMenuItem roadViewMenu = null;

        // Debug
        private ApplicationBarMenuItem debugMenu = null;
        private ApplicationBarIconButton debugButton = null;
        private Microsoft.Phone.Controls.Maps.MapPolygon rBorder = null;
        private Microsoft.Phone.Controls.Maps.MapPolygon rFill = null;

        private Pushpin selectedPin = null;     // Keep around so we can delete it when next pin is selected
        private LastVisiblePage lastVisiblePage;
        private bool firstMapClick = true;
        private bool loginCanceledByUser = false;

        // Login stuff
        App app = App.Current as App;
        Popup p;
        PasswordBox pb;
        TextBox ub;

        private static Timer t = null;

        #region General Initialization
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            /*  Todo:  Figure out why animations are causing page transitions to hang
            // Enable turnstile page transitions
            NavigationInTransition inTransition;
            NavigationOutTransition outTransition; 
            TurnstileTransition backwardTurnstileTransition;
            TurnstileTransition forwardTurnstileTransition;

            inTransition = new NavigationInTransition();
            backwardTurnstileTransition = new TurnstileTransition();
            backwardTurnstileTransition.Mode = TurnstileTransitionMode.BackwardIn;
            forwardTurnstileTransition = new TurnstileTransition();
            forwardTurnstileTransition.Mode = TurnstileTransitionMode.ForwardIn;
            inTransition.Backward = backwardTurnstileTransition;
            inTransition.Forward = forwardTurnstileTransition;
            TransitionService.SetNavigationInTransition(this, inTransition);

            outTransition = new NavigationOutTransition();
            backwardTurnstileTransition = new TurnstileTransition();
            backwardTurnstileTransition.Mode = TurnstileTransitionMode.BackwardOut;
            forwardTurnstileTransition = new TurnstileTransition();
            forwardTurnstileTransition.Mode = TurnstileTransitionMode.ForwardOut;
            outTransition.Backward = backwardTurnstileTransition;
            outTransition.Forward = forwardTurnstileTransition;
            TransitionService.SetNavigationOutTransition(this, outTransition);
            */

            App.webService.RegisterLoginFailedCallback(new WebService.LoginFailedCallback(PromptForCredentials));
            App.webService.RegisterLogoutCompleteCallback(new WebService.LogoutCompleteCallback(PromptForCredentials));
            App.webService.RegisterWP8KickstartCallback(new WebService.WP8KickstartCallback(ClickMe));
            App.webService.RegisterLoginCompleteCallback(new WebService.LoginCompleteCallback(loginCompleteCallback));
            App.locationService.RegisterUpdateCallback(new LocationServices.LocationService.UpdateCallback(newMeLocation));

            // debug
            App.webService.RegisterQueryExtentCallback(new WebService.QueryExtentCallback(PaintQueryExtent));

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModelMain;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            setAppBar();

            if (String.IsNullOrEmpty(App.settings.myUsername) && String.IsNullOrEmpty(App.settings.myPassword))
            {
                App.settings.isAuthenticated = false;
            }
            else
            {
                App.settings.isAuthenticated = true;
            }

            updateLoginLogoutMenu();

//            MainPivot.Visibility = System.Windows.Visibility.Visible;
//            MapPage.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {   // Treat clicking the map button like a normal navigation operation, back button takes you to main pivot

            if (MainPivot.Visibility == Visibility.Collapsed)
            {
                MainPivot.Visibility = Visibility.Visible;
                MapPage.Visibility = Visibility.Collapsed;

                updateAppbar();

                e.Cancel = true;
            }
            else if (null != p)
            {
                if (p.IsOpen)
                {   // Close login dialog
                    p.IsOpen = false;
                    ApplicationBar.IsVisible = true;
                    e.Cancel = true;
                }
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModelMain.IsDataLoaded)
            {
                App.ViewModelMain.LoadData();
            }

            // Make sure we're showing the correct pushpin image
            ViewChangeEnd();    

            // Enable selection on lists while pivot is visible
            PinnedListBox.SelectionChanged += PinnedListBox_SelectionChanged;
            NearbyListBox.SelectionChanged += NearbyListBox_SelectionChanged;
            FoundListBox.SelectionChanged += FoundListBox_SelectionChanged;

            updateAppbar();

            updateLoginLogoutMenu();

            PromptForCredentials(WebResources.LoginNew, App.settings.myUsername, App.settings.myPassword);
        }

        private void updateLoginLogoutMenu()
        {
            if (App.settings.isAuthenticated)
            {   // logged in
                logoutMenu.Text = WebResources.LogoutMenuText;
                App.ViewModelMain.loginAlert = "";
                App.ViewModelHost.loginAlert = "";
                App.ViewModelMessage.loginAlert = "";
            }
            else
            {   // logged out
                logoutMenu.Text = WebResources.LoginMenuText;
                if (!App.settings.isFirstStartup) App.ViewModelMain.loginAlert = WebResources.AlertNotLoggedIn;
                App.ViewModelHost.loginAlert = WebResources.AlertNotLoggedIn;
                App.ViewModelMessage.loginAlert = WebResources.AlertNotLoggedIn;
            }
        }

        private void MapInitializationComplete()
        {
            this.MapPage.Visibility = Visibility.Collapsed;
            this.SplashScreen.Visibility = Visibility.Collapsed;
            this.MainPivot.Visibility = Visibility.Visible;
            App.nearby.isCenteredOnMe = true;
            LoginInfo.Visibility  = PannedInfo.Visibility  = LocationInfo.Visibility  = NetworkInfo.Visibility  = Visibility.Visible;
            LoginInfo2.Visibility = PannedInfo2.Visibility = LocationInfo2.Visibility = NetworkInfo2.Visibility = Visibility.Visible;
            LoginInfo3.Visibility = PannedInfo3.Visibility = LocationInfo3.Visibility = NetworkInfo3.Visibility = Visibility.Visible;
            LoginInfo4.Visibility = PannedInfo4.Visibility = LocationInfo4.Visibility = NetworkInfo4.Visibility = Visibility.Visible;

            App.nearby.loadHosts();     // Load hosts we pulled from isolated storage
        }

        #endregion

        #region Appbar
        private void setAppBar()
        {
            ApplicationBar.IsVisible = false;

            SearchCanvas.Visibility = System.Windows.Visibility.Collapsed;
            SearchBox.Visibility = System.Windows.Visibility.Collapsed;

            int len = ApplicationBar.Buttons.Count;
            if (len <= 0)
            {   // No buttons assigned to application bar in XAML
                //                if (null == filterButton)
                //                {   // Create filter button
                //                    filterButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.filter.png", UriKind.Relative));
                //                    filterButton.Text = WebResources.filterButtonText;
                //                    filterButton.Click += ApplicationBarIconButton_Click_Filter;
                //                }
                if (null == meButton)
                {   // Create me button
                    meButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.location.circle.png", UriKind.Relative));
                    meButton.Text = WebResources.meButtonText;
                    meButton.Click += ApplicationBarIconButton_Click_Me;
                }
                if (null == mapButton)
                {   // Create me button
                    mapButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.map.png", UriKind.Relative));
                    mapButton.Text = WebResources.mapButtonText;
                    mapButton.Click += ApplicationBarIconButton_Click_Map;
                }
                if (null == updateButton)
                {   // Create update button
                    updateButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.getnew.png", UriKind.Relative));
                    updateButton.Text = WebResources.updateButtonText;
                    updateButton.Click += ApplicationBarIconButton_Click_Update;
                }
                if (null == allButton)
                {   // Create update button
                    allButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.getall.png", UriKind.Relative));
                    allButton.Text = WebResources.allButtonText;
                    allButton.Click += ApplicationBarIconButton_Click_All;
                }

                if (null == searchButton)
                {   // Create search button
                    searchButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.magnify.png", UriKind.Relative));
                    searchButton.Text = WebResources.searchButtonText;
                    searchButton.Click += ApplicationBarIconButton_Click_Search;
                }

                // Debug
                if (null == debugButton)
                {   // Create me button
                    debugButton = new ApplicationBarIconButton(new Uri("/Images/Appbar/appbar.attach.rest.png", UriKind.Relative));
                    debugButton.Text = WebResources.debugButtonText;
                    debugButton.Click += ApplicationBarIconButton_Click_Debug;
                }
            }

            len = ApplicationBar.MenuItems.Count;
            if (len <= 0)
            {   // No menus assigned to application bar in XAML
                if (null == webSiteMenu)
                {   // Create menu item to launch web site
                    webSiteMenu = new ApplicationBarMenuItem();
                    webSiteMenu.Text = WebResources.webSiteMenuText;
                    webSiteMenu.Click += webSiteMenu_Click;
                }
                if (null == unitsMenu)
                {   // Create menu item to change distance units
                    unitsMenu = new ApplicationBarMenuItem();
                    if (App.settings.isMetric)
                    {
                        unitsMenu.Text = WebResources.unitsMenuTextMi;
                    }
                    else
                    {
                        unitsMenu.Text = WebResources.unitsMenuTextKm;
                    }
                    unitsMenu.Click += unitsMenu_Click;
                }
                if (null == logoutMenu)
                {   // Create menu item to log out
                    logoutMenu = new ApplicationBarMenuItem();
                    logoutMenu.Text = WebResources.LogoutMenuText;
                    logoutMenu.Click += loginLogoutMenu_Click;
                }
                if (null == aerialViewMenu)
                {   // Create menu item to change to aerial map mode
                    aerialViewMenu = new ApplicationBarMenuItem();
                    aerialViewMenu.Text = WebResources.aerialViewMenuText;
                    aerialViewMenu.Click += aerialViewMenu_Click;
                }
                if (null == roadViewMenu)
                {   // Create menu item to change to road map mode
                    roadViewMenu = new ApplicationBarMenuItem();
                    roadViewMenu.Text = WebResources.roadViewMenuText;
                    roadViewMenu.Click += roadViewMenu_Click;
                }
                if (null == aboutMenu)
                {   // Create menu item to show traffic on map
                    aboutMenu = new ApplicationBarMenuItem();
                    aboutMenu.Text = WebResources.aboutMenuText;
                    aboutMenu.Click += aboutMenu_Click;
                }

                if (null == debugMenu)
                {   // Create menu item to toggle debug mode   
                    debugMenu = new ApplicationBarMenuItem();
                    debugMenu.Text = WebResources.debugMenuText;
                    debugMenu.Click += debugMenu_Click;
                }
            }

            // Assuming we start up on pivot Todo: is this always true?
            lastVisiblePage = LastVisiblePage.startup;
            updateAppbar();
        }

        void updateAppbar()
        {   // Handle appbar switching between map and main pivot           
            if (lastVisiblePage == LastVisiblePage.startup || (MainPivot.Visibility == Visibility.Visible && lastVisiblePage != LastVisiblePage.mainpivot))
            {   // Main pivot visible
                lastVisiblePage = LastVisiblePage.mainpivot;
                // Add map button
                ApplicationBar.Buttons.Clear();
                //                ApplicationBar.Buttons.Add(filterButton);
                ApplicationBar.Buttons.Add(meButton);
                ApplicationBar.Buttons.Add(mapButton);

                // Add appropriate menu items
                ApplicationBar.MenuItems.Clear();
                ApplicationBar.MenuItems.Add(webSiteMenu);
                ApplicationBar.MenuItems.Add(unitsMenu);
                ApplicationBar.MenuItems.Add(logoutMenu);
                ApplicationBar.MenuItems.Add(aboutMenu);

                // Debug.  Uncomment this to enable debug mode.
                ApplicationBar.MenuItems.Add(debugMenu);   
            }
            else if (MainPivot.Visibility != Visibility.Visible && lastVisiblePage != LastVisiblePage.mappage)
            {   // Map visible
                lastVisiblePage = LastVisiblePage.mappage;
                ApplicationBar.Buttons.Clear();
                //            ApplicationBar.Buttons.Add(filterButton);
                ApplicationBar.Buttons.Add(meButton);
                ApplicationBar.MenuItems.Clear();
                ApplicationBar.MenuItems.Add(aerialViewMenu);
                ApplicationBar.MenuItems.Add(roadViewMenu);

                // Debug
                if (App.ViewModelMain.debug)
                {
                    ApplicationBar.Buttons.Add(debugButton);    // Todo: comment out for public release
                }
            }
        }

        private void ApplicationBarIconButton_Click_Filter(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            { NavigationService.Navigate(new Uri("/FilterPage.xaml", UriKind.Relative)); });
        }

        private void ApplicationBarIconButton_Click_Map(object sender, EventArgs e)
        {
            if (firstMapClick)
            {
                firstMapClick = false;
//                App.nearby.viewportCache.mapInitialized();
                mePushpin.Visibility = Visibility.Visible;
            }
            this.MainPivot.Visibility = System.Windows.Visibility.Collapsed;
            this.MapPage.Visibility = System.Windows.Visibility.Visible;
            updateAppbar();
        }

        private void ClickMe()
        {
            double oldZoomLevel = myMap.ZoomLevel;

            App.ViewModelMain.isMeButtonPressPending = true;
            if (App.nearby.isCenteredOnMe)
            {   // Pressing me button when map not panned forces getHosts request, bypassing viewPort cache
                GeoCoordinate loc = App.nearby.meCenter;
                if (null != loc)
                {   // There are a couple reasons why LocationRect could be out of sync with meCenter:
                    //  Reason 1: User hit me button on map and then quickly switched to nearby view.  
                    //  Reason 2: User physically moved and the new meCenter is outside locationRect
                    myMap.SetView(loc, oldZoomLevel);
                    App.ViewModelMain.mapLocation = loc;
                    ViewChangeEnd();
                    App.nearby.viewportCache.getHostsForce(loc);
                }
            }
            else
            {   // Map is panned
                App.nearby.isCenteredOnMe = true;
                //            meButton.IconUri = meButtonImage;
                GeoCoordinate loc = App.nearby.meCenter;
                if (null != loc)
                {
                    App.nearby.mapCenter = loc;
                    myMap.SetView(loc, oldZoomLevel);

                    if (this.MapPage.Visibility == Visibility.Collapsed)
                    {   // Map processing disabled when page not visible, do it manually
                        const int startTime = 500;     // Milliseconds until first timer callback 
                        const int periodTime = 500;    // Milliseconds between timer callback 
                        t = new Timer(timer_ViewChangeEnd, t, startTime, periodTime);
                    }
                }
            }
        }

        private void ApplicationBarIconButton_Click_Me(object sender, EventArgs e)
        {
            ClickMe();
        }

        // Debug
        private void ApplicationBarIconButton_Click_Debug(object sender, EventArgs e)
        {   // Send debug info back to creator
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = WebResources.debugEmailAddress;
            emailComposeTask.Subject = WebResources.debugSubject;
            emailComposeTask.Body = WebService.debugPayload;
            emailComposeTask.Show();
        }

        private void ApplicationBarIconButton_Click_Search(object sender, EventArgs e)
        {   // Todo:  implement host search
            if (SearchCanvas.Visibility == System.Windows.Visibility.Visible)
            {
                SearchCanvas.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                SearchCanvas.Visibility = System.Windows.Visibility.Visible;
                SearchBox.Visibility = System.Windows.Visibility.Visible;
                if (SearchBox.Text != "")
                {
                    SearchBoxHint.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    SearchBoxHint.Text = "username, full name, town, or email";
                    SearchBox.Visibility = System.Windows.Visibility.Visible;
                    SearchBox.SelectAll();
                }
                SearchBox.Focus();
            }
        }

        private void ApplicationBarIconButton_Click_Update(object sender, EventArgs e)
        {
            WebService.GetMessages(false);
        }

        private void ApplicationBarIconButton_Click_All(object sender, EventArgs e)
        {
            WebService.GetMessages(true);
        }

        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Search(SearchBox.Text);
            }
            else if (SearchBox.Text == "")
            {
                SearchBoxHint.Visibility = Visibility.Visible;
            }
            else
            {
                SearchBoxHint.Visibility = Visibility.Collapsed;
            }
        }

        private void Search(string searchString)
        {
            SearchCanvas.Visibility = System.Windows.Visibility.Collapsed;
            SearchBox.Visibility = System.Windows.Visibility.Collapsed;
            WebService.GetUsers(searchString);
        }

        #endregion

        #region Login Dialog
        private void PromptForCredentials(string msg, string un, string pw)
        {
            updateLoginLogoutMenu();
            if (App.nearby.AboutPageActive) return;
            if (loginCanceledByUser) return;

            if (null != p)
            {
                if (p.IsOpen)
                {
                    return;
                }
            }

            if (!App.settings.isAuthenticated)
            {
                p = new Popup();
                LayoutRoot.Children.Add(p);
                ApplicationBar.IsVisible = false;

                // Set where the popup will show up on the screen.    
                p.VerticalOffset = 50;
                p.HorizontalOffset = 25;

                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Colors.Gray);
                border.BorderThickness = new Thickness(5.0);

                StackPanel panel1 = new StackPanel();
                panel1.Background = new SolidColorBrush(Colors.Gray);
                if (LayoutRoot.ActualWidth > 2)
                {
                    panel1.Width = LayoutRoot.ActualWidth - 2 * p.HorizontalOffset;
                }

                Button button1 = new Button();
                button1.Content = WebResources.OKButtonText;
                button1.Margin = new Thickness(5.0);
                button1.Click += new RoutedEventHandler(loginbutton_Click);

                Button button2 = new Button();
                button2.Content = WebResources.CancelButtonText;
                button2.Margin = new Thickness(5.0);
                button2.Click += new RoutedEventHandler(cancelbutton_Click);

                Button button3 = new Button();
                button3.Content = WebResources.RegisterButtonText;
                button3.Margin = new Thickness(5.0);
                button3.Click += new RoutedEventHandler(registerbutton_Click);

                TextBlock textblock1 = new TextBlock();
                textblock1.TextWrapping = TextWrapping.Wrap;
                textblock1.Text = " " + WebResources.Username;
                textblock1.Margin = new Thickness(5.0);
                textblock1.FontSize = 22;
                textblock1.Foreground = new SolidColorBrush(Colors.White);

                TextBlock textblock2 = new TextBlock();
                textblock2.TextWrapping = TextWrapping.Wrap;
                textblock2.Text = " " + WebResources.Password;
                textblock2.Margin = new Thickness(5.0);
                textblock2.FontSize = 22;
                textblock2.Foreground = new SolidColorBrush(Colors.White);

                TextBlock textblock3 = new TextBlock();
                textblock3.TextWrapping = TextWrapping.Wrap;
                textblock3.Text = " " + msg;
                textblock3.Margin = new Thickness(5.0);
                textblock3.FontSize = 25;
                if (msg == WebResources.LoginFailed)
                {
                    textblock3.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    textblock3.Foreground = new SolidColorBrush(Colors.White);
                }

                ub = new TextBox();

                // Use sensible keyboard layout for entering an email-style username
                InputScope Keyboard = new InputScope();
                InputScopeName ScopeName = new InputScopeName();
                ScopeName.NameValue = InputScopeNameValue.EmailUserName;
                Keyboard.Names.Add(ScopeName);
                ub.InputScope = Keyboard;  
            
                ub.Text = un;

                pb = new PasswordBox();

                pb.Password = pw;
                pb.KeyDown += new KeyEventHandler(pb_KeyDown);
                panel1.Children.Add(textblock3);
                panel1.Children.Add(textblock1);
                panel1.Children.Add(ub);
                panel1.Children.Add(textblock2);
                panel1.Children.Add(pb);

                StackPanel panel2 = new StackPanel();
                panel2.Orientation = System.Windows.Controls.Orientation.Horizontal;
                panel2.Children.Add(button1);
                panel2.Children.Add(button3); 
                panel2.Children.Add(button2);

                panel1.Children.Add(panel2);
                border.Child = panel1;

                // Set the Child property of Popup to the border     
                // which contains a stackpanel, textblock and button.    
                p.Child = border;

                // Open the popup.    
                p.IsOpen = true;
                ub.Focus();
            }
        }

        private void loginCompleteCallback()
        {
            App.settings.isAuthenticated = true;
            App.settings.isFirstStartup = false;
            updateLoginLogoutMenu();
        }

        private void loginbutton_Click(object sender, RoutedEventArgs e)
        {
            // Close the popup.
            p.IsOpen = false;

            App.settings.myUsername = ub.Text;
            App.settings.myPassword = pb.Password;

            App.ViewModelMain.LoadData();

            WebService.Login();

            ApplicationBar.IsVisible = true;
        }

        private void registerbutton_Click(object sender, RoutedEventArgs e)
        {
            App.settings.isFirstStartup = false;
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(WebResources.uriPrefix + WebResources.WarmShowersUri + "/user/register", UriKind.Absolute);
            task.Show();

            ApplicationBar.IsVisible = true;
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            // Close the popup.
            p.IsOpen = false;

            App.settings.isFirstStartup = false;
            ApplicationBar.IsVisible = true;
            loginCanceledByUser = true;
            updateLoginLogoutMenu();
        }

        void pb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                loginbutton_Click(this, new RoutedEventArgs());
        }

        #endregion

        #region Menu Handlers

        private void centerNearbyItem_Click(object sender, RoutedEventArgs e)
        {
            // Get host location
            NearbyItemViewModel vm = (NearbyItemViewModel)(sender as MenuItem).DataContext;
            int uId = vm.userID;
            centerItem_Click(uId);
        }

        private void centerFoundItem_Click(object sender, RoutedEventArgs e)
        {
            // Get host location
            FoundItemViewModel vm = (FoundItemViewModel)(sender as MenuItem).DataContext;
            int uId = vm.userID;
            centerItem_Click(uId);
        }

        private void centerPinnedItem_Click(object sender, RoutedEventArgs e)
        {
            // Get host location
            PinnedItemViewModel vm = (PinnedItemViewModel)(sender as MenuItem).DataContext;
            int uId = vm.userID;
            centerItem_Click(uId);
        }

        private void centerItem_Click(int uId)
        {

            double lat = App.nearby.getLat(uId);
            double lon = App.nearby.getLon(uId);

            // Center map on host location
            double oldZoomLevel = myMap.ZoomLevel;

            App.ViewModelMain.isMeButtonPressPending = true;        

            App.nearby.isCenteredOnMe = false;
            GeoCoordinate loc = new GeoCoordinate();
            loc.Latitude = lat;
            loc.Longitude = lon;
            App.nearby.mapCenter = loc;
            myMap.SetView(loc, oldZoomLevel);

            if (this.MapPage.Visibility == Visibility.Collapsed)
            {   // Map processing disabled when page not visible, do it manually
                const int startTime = 1000;     // Milliseconds until first timer callback 
                const int periodTime = 500;    // Milliseconds between timer callback 
                t = new Timer(timer_ViewChangeEnd, t, startTime, periodTime);
            }
        }

        private void pinNearbyItem_Click(object sender, RoutedEventArgs e)
        {
            NearbyItemViewModel vm = (NearbyItemViewModel)(sender as MenuItem).DataContext;

            if (App.pinned.isPinned(vm.userID))
            {
                App.pinned.unPin(vm.userID);
            }
            else
            {
                App.pinned.autoPin(vm.userID, vm.Name); // This will pin the minimal information currently available
                WebService.GetHost(vm.userID);          // Try to get complete set of information to update pin
            }
        }

        // Todo: refactor
        private void pinFoundItem_Click(object sender, RoutedEventArgs e)
        {
            FoundItemViewModel vm = (FoundItemViewModel)(sender as MenuItem).DataContext;

            if (App.pinned.isPinned(vm.userID))
            {
                App.pinned.unPin(vm.userID);
            }
            else
            {
                App.pinned.autoPin(vm.userID, vm.Name); // This will pin the minimal information currently available
                WebService.GetHost(vm.userID);          // Try to get complete set of information to update pin
            }
        }

        private void UnpinPinnedItem_Click(object sender, RoutedEventArgs e)
        {
            PinnedItemViewModel vm = (PinnedItemViewModel)(sender as MenuItem).DataContext;
            App.pinned.unPin(vm.userID);
        }

        private void UnpinFoundItem_Click(object sender, RoutedEventArgs e)
        {
            FoundItemViewModel vm = (FoundItemViewModel)(sender as MenuItem).DataContext;
            App.pinned.unPin(vm.userID);
        }

        private void webSiteMenu_Click(object sender, EventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(WebResources.uriPrefix + WebResources.WarmShowersUri, UriKind.Absolute);
            task.Show();

        }

        private void unitsMenu_Click(object sender, EventArgs e)
        {   // Toggle between metric and english
            if (App.settings.isMetric)
            {
                unitsMenu.Text = WebResources.unitsMenuTextKm;
                App.settings.isMetric = false;
            }
            else
            {
                unitsMenu.Text = WebResources.unitsMenuTextMi;
                App.settings.isMetric = true;
            }
            App.nearby.loadHosts();
        }

        private void loginLogoutMenu_Click(object sender, EventArgs e)
        {
            loginCanceledByUser = false; 
            
            if (App.settings.isAuthenticated)
            {   // log out
                App.settings.myUsername = "";
                App.settings.myPassword = "";
                App.settings.isAuthenticated = false;
                updateLoginLogoutMenu();
                WebService.Logout();
            }
            else
            {   // log in
                PromptForCredentials(WebResources.LoginNew, App.settings.myUsername, App.settings.myPassword);
            }
        }

        private void aerialViewMenu_Click(object sender, EventArgs e)
        {
            myMap.Mode = new AerialMode();
        }

        private void roadViewMenu_Click(object sender, EventArgs e)
        {
            myMap.Mode = new RoadMode();
        }

        private void debugMenu_Click(object sender, EventArgs e)
        {
            App.ViewModelMain.debug = true;
            debugMenu.Text = WebResources.debugEnabled; 
        }

        private void aboutMenu_Click(object sender, EventArgs e)
        {
            App.nearby.AboutPageActive = true;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            { NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative)); });
        }

        #endregion 

        #region List Management

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            switch (((Pivot)sender).SelectedIndex)
            {
                case (int) pivotPage.nearby:
//                    ApplicationBar.Buttons.Add(filterButton);
                    ApplicationBar.Buttons.Add(meButton);                    
                    ApplicationBar.Buttons.Add(mapButton);
                    SearchCanvas.Visibility = Visibility.Collapsed;
                    break;

                case (int) pivotPage.pinned:
                    ApplicationBar.Buttons.Add(updateButton);
                    ApplicationBar.Buttons.Add(allButton);
                    SearchCanvas.Visibility = Visibility.Collapsed;
                    break;

                case (int)pivotPage.found:
                    ApplicationBar.Buttons.Add(searchButton);
                    break;
            }
            if (null == p)
            {
                ApplicationBar.IsVisible = true;
            }
        }

        private void NearbyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != e && NearbyListBox.SelectedIndex > -1)
            {
                NearbyItemViewModel vm = (NearbyItemViewModel)e.AddedItems[0];
                if (null != vm)
                {
                    if ((vm.Name != WebResources.NearbyListEmptyHeader) && (vm.Name != WebResources.loading) && (vm.Name != WebResources.NearbyListLocationDisabledHeader))   // Disable click
                    {   
                        int uId = vm.userID;
                        DisplayHost(uId);
                    }
                }
            }
        }

        private void PinnedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != e && PinnedListBox.SelectedIndex > -1)
            {
                PinnedItemViewModel vm = (PinnedItemViewModel)e.AddedItems[0];
                if (null != vm)
                {
                    if ((vm.Name != WebResources.PinnedListEmptyHeader) && (vm.Name != WebResources.loading))   // Disable click  Todo:  make this a better test
                    {
                        int uId = vm.userID;
                        DisplayHost(uId);
                    }
                }
            }
        }

        private void FoundListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != e && FoundListBox.SelectedIndex > -1)
            {
                FoundItemViewModel vm = (FoundItemViewModel)e.AddedItems[0];
                if (null != vm)
                {
                    if ((vm.Name != WebResources.FoundListEmptyHeader) && (vm.Name != WebResources.loading))   // Disable click  Todo:  make this a better test
                    {
                        int uId = vm.userID;
                        DisplayHost(uId);
                    }
                }
            }
        }

        private void DisplayHost(int uId)
        {
            if (!App.nearby.loadHostFromStorage(uId))     // Try to load cached copy of host profile
            {
                if (uId != App.nearby.selectedUid)
                {   // User changed, saved data is invalid
                    App.nearby.host.profile.user_Result = null;
                    App.nearby.host.feedback.recommendations_Result = null;
                    App.nearby.host.messages.messages_result = null;
                    App.nearby.messageThread.message_result = null;
                }
            }
            App.nearby.selectedUid = uId;

            WebService.GetHost(uId);   // Begin user query for latest profile

            // Disable selection on lists while main pivot is invisible
            PinnedListBox.SelectionChanged -= PinnedListBox_SelectionChanged;
            NearbyListBox.SelectionChanged -= NearbyListBox_SelectionChanged;
            FoundListBox.SelectionChanged -= FoundListBox_SelectionChanged;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            { NavigationService.Navigate(new Uri("/HostPivotPage.xaml?id=" + uId, UriKind.Relative)); });
            PinnedListBox.SelectedIndex = -1; // Enable re-selection.  
            NearbyListBox.SelectedIndex = -1; // Enable re-selection. 
            FoundListBox.SelectedIndex = -1; // Enable re-selection.  
        }

        private void timer_ViewChangeEnd(Object state)
        {   // Do this on timer to give map time to process new location
            t.Dispose(); 
            Deployment.Current.Dispatcher.BeginInvoke(() => { ViewChangeEnd(); }); 
        }

        private void myMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            ViewChangeEnd();
        }

        private void ViewChangeEnd()
        {   // Set the query extent to match
            if (App.ViewModelMain.isMapInitialized)
            {
                if (this.MapPage.Visibility == Visibility.Visible)
                {   
                    App.ViewModelMain.isMeButtonPressPending = false;

                    // Binding doesn't always seem to work to set new map center, causes panned alert not to fire
                    // Adding this line with TargetCenter instead of Center seemed to solve the problem
                    App.ViewModelMain.mapLocation = myMap.TargetCenter;                        
                }
            }
            else
            {   // Map is flaky if collapsed, must wait until initialization is complete
                App.ViewModelMain.isMapInitialized = true;
                MapInitializationComplete();
            }


            // Show me position on map
            if (null != App.nearby.meCenter)
            {
                mePushpin.Location = App.nearby.meCenter;

                // Hack to ensure current location is shown on top of other pushpins
                myMap.Children.Remove(mePushpin);
                myMap.Children.Add(mePushpin);

                if (App.nearby.isCenteredOnMe)      
                {   // Dealing with some initialization nonsense on WP8
                    if (!App.nearby.viewportCache.isInside(App.nearby.meCenter, myMap.TargetBoundingRectangle))
                    {
                        myMap.SetView(App.nearby.meCenter, App.ViewModelMain.zoom);
                        App.ViewModelMain.mapLocation = App.nearby.meCenter;
                    }
                }
                App.nearby.locationRect = myMap.TargetBoundingRectangle;
            }

            App.nearby.viewportCache.getHosts();

        }

        private void PaintQueryExtent(double lat, double lon, double north, double south, double east, double west, int limit)
        {   // Debug.  Paint query extent on the map.
            if (null != rBorder)
            {
                myMap.Children.Remove(rBorder);
            }

            if (null != rFill)
            {
                myMap.Children.Remove(rFill);
            }

            rBorder = new Microsoft.Phone.Controls.Maps.MapPolygon();
            rBorder.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            rBorder.StrokeThickness = 10;
            rBorder.Opacity = 0.8;

            rFill = new Microsoft.Phone.Controls.Maps.MapPolygon();
            rFill.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            rFill.StrokeThickness = 10;
            rFill.Opacity = 0.1;

            var collection = new Microsoft.Phone.Controls.Maps.LocationCollection();
            collection.Add(new GeoCoordinate(north, east));
            collection.Add(new GeoCoordinate(south, east));
            collection.Add(new GeoCoordinate(south, west));
            collection.Add(new GeoCoordinate(north, west));

            rBorder.Locations = collection;
            rFill.Locations = collection;

            // Add the polyline to the map
            myMap.Children.Add(rBorder);
            myMap.Children.Add(rFill);
        }

        #endregion

        #region Map Page
        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pin = sender as Pushpin;

            if (null != selectedPin)
            {   // only one pin selected at a time
                myMap.Children.Remove(selectedPin);
            }

            selectedPin = new Pushpin();
            selectedPin.Location = pin.Location;
            selectedPin.Content = pin.Content;
            selectedPin.Tag = pin.Tag;
            selectedPin.Tap += selectedPin_Tap;
            myMap.Children.Add(selectedPin);
            
            e.Handled = true;
        }

        void selectedPin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (null != selectedPin)
            {   // only one pin selected at a time
                myMap.Children.Remove(selectedPin);
            }

            Pushpin pin = sender as Pushpin;
            DisplayHost((int)pin.Tag);
        }

        void newMeLocation(GeoCoordinate loc)
        {
            // This callback function is a hack because I could never get the binding to work in MainViewModel-- the map never calls meLocation.  
            // May be a bug in the map control, see http://social.msdn.microsoft.com/Forums/en-US/683db702-6d57-4fdd-8ba5-25e37fd362eb/wpf-bing-maps-control-maplayerposition-not-updated?forum=bingmapssilverlightwpfcontrols

            mePushpin.Location = loc;

            // Hack to ensure current location is shown on top of other pushpins
            myMap.Children.Remove(mePushpin);
            myMap.Children.Add(mePushpin);
        }

        #endregion

    }
}