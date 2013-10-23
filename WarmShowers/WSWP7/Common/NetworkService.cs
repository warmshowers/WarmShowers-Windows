using System;
using System.Windows;
using System.ComponentModel;
using Microsoft.Phone.Net.NetworkInformation;

namespace WSApp.NetworkServices
{
    public sealed class NetworkService
    {
        // Singleton object
        static readonly NetworkService _instance = new NetworkService();
        public static NetworkService Instance
        {
            get
            {
                return _instance;
            }
        }

        BackgroundWorker interfaceTypeWorker;
        NetworkInterfaceType netType;
        bool isNetworkAvailable = false;

        public delegate void AlertCallback(string alert);
        AlertCallback alertCallback;

        // Constructor
        public NetworkService()
        {
        }

        public void Start()
        {
            interfaceTypeWorker = new BackgroundWorker();
            interfaceTypeWorker.DoWork += new DoWorkEventHandler(interfaceTypeWorker_DoWork);
            interfaceTypeWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(interfaceTypeWorker_RunWorkerCompleted);  
        }

        public void RegisterAlertCallback(AlertCallback func)
        {
            alertCallback += func;
        }

        void interfaceTypeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            if (isNetworkAvailable)
            {
                netType = NetworkInterface.NetworkInterfaceType;
                if (netType == NetworkInterfaceType.None)
                {
                    isNetworkAvailable = false;
                }
            }

        }

        void interfaceTypeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string alertText = "";

            if (isNetworkAvailable)
            {
                alertText = "";
            }
            else
            {
                alertText = WebResources.NoNetwork;
            }

            // Update registered view models with alert   
            Deployment.Current.Dispatcher.BeginInvoke(() => { alertCallback(alertText); });
        }  
 
        public bool IsNetworkAvailable()
        {
            // Begin inquiry about real state of the network, but this can take some time...
            if (!interfaceTypeWorker.IsBusy)
            {
                interfaceTypeWorker.RunWorkerAsync();
            }

            // ...so if the network isn't explicitly disabled, allow requests to fire
            return NetworkInterface.GetIsNetworkAvailable();
        }
     }
}
