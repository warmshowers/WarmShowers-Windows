﻿#pragma checksum "C:\Code\WarmShowers\Main\WarmShowers-Windows\WarmShowers\WSWP7\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "83DDDB657560389B41951536B0C31491"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace WSApp {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ApplicationBar MainAppbar;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot MainPivot;
        
        internal System.Windows.Controls.Grid list;
        
        internal System.Windows.Controls.ListBox NearbyListBox;
        
        internal System.Windows.Controls.TextBlock LoginInfo;
        
        internal System.Windows.Controls.TextBlock PannedInfo;
        
        internal System.Windows.Controls.TextBlock LocationInfo;
        
        internal System.Windows.Controls.TextBlock NetworkInfo;
        
        internal System.Windows.Controls.Grid list2;
        
        internal System.Windows.Controls.ListBox PinnedListBox;
        
        internal System.Windows.Controls.TextBlock LoginInfo3;
        
        internal System.Windows.Controls.TextBlock PannedInfo3;
        
        internal System.Windows.Controls.TextBlock LocationInfo3;
        
        internal System.Windows.Controls.TextBlock NetworkInfo3;
        
        internal System.Windows.Controls.Image SplashScreen;
        
        internal System.Windows.Controls.Grid MapPage;
        
        internal Microsoft.Phone.Controls.Maps.Map myMap;
        
        internal System.Windows.Controls.StackPanel SearchCanvas;
        
        internal System.Windows.Controls.TextBox SearchBox;
        
        internal System.Windows.Controls.Button SearchButton;
        
        internal System.Windows.Controls.TextBlock LoginInfo2;
        
        internal System.Windows.Controls.TextBlock PannedInfo2;
        
        internal System.Windows.Controls.TextBlock LocationInfo2;
        
        internal System.Windows.Controls.TextBlock NetworkInfo2;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/WSApp;component/MainPage.xaml", System.UriKind.Relative));
            this.MainAppbar = ((Microsoft.Phone.Shell.ApplicationBar)(this.FindName("MainAppbar")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.MainPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("MainPivot")));
            this.list = ((System.Windows.Controls.Grid)(this.FindName("list")));
            this.NearbyListBox = ((System.Windows.Controls.ListBox)(this.FindName("NearbyListBox")));
            this.LoginInfo = ((System.Windows.Controls.TextBlock)(this.FindName("LoginInfo")));
            this.PannedInfo = ((System.Windows.Controls.TextBlock)(this.FindName("PannedInfo")));
            this.LocationInfo = ((System.Windows.Controls.TextBlock)(this.FindName("LocationInfo")));
            this.NetworkInfo = ((System.Windows.Controls.TextBlock)(this.FindName("NetworkInfo")));
            this.list2 = ((System.Windows.Controls.Grid)(this.FindName("list2")));
            this.PinnedListBox = ((System.Windows.Controls.ListBox)(this.FindName("PinnedListBox")));
            this.LoginInfo3 = ((System.Windows.Controls.TextBlock)(this.FindName("LoginInfo3")));
            this.PannedInfo3 = ((System.Windows.Controls.TextBlock)(this.FindName("PannedInfo3")));
            this.LocationInfo3 = ((System.Windows.Controls.TextBlock)(this.FindName("LocationInfo3")));
            this.NetworkInfo3 = ((System.Windows.Controls.TextBlock)(this.FindName("NetworkInfo3")));
            this.SplashScreen = ((System.Windows.Controls.Image)(this.FindName("SplashScreen")));
            this.MapPage = ((System.Windows.Controls.Grid)(this.FindName("MapPage")));
            this.myMap = ((Microsoft.Phone.Controls.Maps.Map)(this.FindName("myMap")));
            this.SearchCanvas = ((System.Windows.Controls.StackPanel)(this.FindName("SearchCanvas")));
            this.SearchBox = ((System.Windows.Controls.TextBox)(this.FindName("SearchBox")));
            this.SearchButton = ((System.Windows.Controls.Button)(this.FindName("SearchButton")));
            this.LoginInfo2 = ((System.Windows.Controls.TextBlock)(this.FindName("LoginInfo2")));
            this.PannedInfo2 = ((System.Windows.Controls.TextBlock)(this.FindName("PannedInfo2")));
            this.LocationInfo2 = ((System.Windows.Controls.TextBlock)(this.FindName("LocationInfo2")));
            this.NetworkInfo2 = ((System.Windows.Controls.TextBlock)(this.FindName("NetworkInfo2")));
        }
    }
}
