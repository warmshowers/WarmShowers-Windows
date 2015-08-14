using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using WSApp.Utility;
using WSApp.ViewModel;

namespace WSApp.DataModel
{
    public static class PinnedStore
    {
        const string storeFilename = "WSPinned";

        public static void load()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                IsolatedStorageFileStream stream = store.OpenFile(storeFilename, FileMode.OpenOrCreate);
                if (null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PinnedData));
                    App.pinned = (PinnedData)ser.ReadObject(stream);
                    stream.Close();

                    if (null == App.pinned)
                    {   // First time app was run since installation
                        App.pinned = new PinnedData();
                    }
                }
                loadUI();

            }
            catch (System.Exception) { };

 //           App.nearby.loadHosts();  Todo:  Remove?
        }

        public static void save()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                IsolatedStorageFileStream stream = store.OpenFile(storeFilename, FileMode.Truncate);
                if (null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PinnedData));
                    ser.WriteObject(stream, App.pinned);
                    stream.Close();
                }
            }
            catch (System.Exception) { }
        }

        private static void loadUI()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModelMain.pinnedItems.Clear();
            });

            if (App.pinned.pinnedProfiles.Count == 0)
            {   // Load 'no hosts pinned' help text
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModelMain.pinnedItems.Add(new PinnedItemViewModel() { Name = WebResources.PinnedListEmptyHeader, line2 = WebResources.PinnedListEmptyBody + "\n\n" + WebResources.PinnedListEmptyBody2});
                });

            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var p in App.pinned.pinnedProfiles)
                    {
                        App.ViewModelMain.pinnedItems.Add(new PinnedItemViewModel() { Name = p.Value.name, Time = p.Value.lastUpdateTime, userID = p.Value.uId });
                    }
                });
            }

 /*           //  Todo:  Fake names for screen shots
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModelMain.pinnedItems.Clear();
                App.ViewModelMain.pinnedItems.Add(new PinnedItemViewModel() { Name = "Jill and Anthony Lake", line2 = "Mar 8 2013 1 new message", Time = 1 });
                App.ViewModelMain.pinnedItems.Add(new PinnedItemViewModel() { Name = "Freddie Lambert", line2 = "Feb 23 2013", Time = 0 });
            });
*/

        }
    }

    public static class FoundStore
    {
        const string storeFilename = "WSFound";

        public static void load()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                IsolatedStorageFileStream stream = store.OpenFile(storeFilename, FileMode.OpenOrCreate);
                if (null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FoundData));
                    App.found = (FoundData)ser.ReadObject(stream);
                    stream.Close();

                    if (null == App.found)
                    {   // First time app was run since installation
                        App.found = new FoundData();
                    }
                }
                loadUI();

            }
            catch (System.Exception) { };

            //           App.nearby.loadHosts();  Todo:  Remove?
        }

        public static void save()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                IsolatedStorageFileStream stream = store.OpenFile(storeFilename, FileMode.Truncate);
                if (null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FoundData));
                    ser.WriteObject(stream, App.found);
                    stream.Close();
                }
            }
            catch (System.Exception) { }
        }

        private static void loadUI()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModelMain.foundItems.Clear();
            });

            if (App.found.foundProfiles.Count == 0)
            {   // Load 'no hosts found' help text
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModelMain.foundItems.Add(new FoundItemViewModel() { Name = WebResources.FoundListEmptyHeader, line2 = WebResources.FoundListEmptyBody});
                });

            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var p in App.found.foundProfiles)
                    {
                        App.ViewModelMain.foundItems.Add(new FoundItemViewModel() { Name = p.Value.name, Time = p.Value.lastUpdateTime, userID = p.Value.uId });
                    }
                });
            }
        }
    }


    public static class NearbyStore
    {
        const string storeFilename = "WSNearby";

        public static void load()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                IsolatedStorageFileStream stream = store.OpenFile(storeFilename, FileMode.OpenOrCreate);
                if (null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Hosts));
                    App.nearby.hosts = (Hosts)ser.ReadObject(stream);

                    stream.Close();
                }

            }
            catch (System.Exception) {};

            if (null == App.nearby.hosts)
            {
                App.nearby.hosts = new Hosts();
            }
        }

        public static void save()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                IsolatedStorageFileStream stream = store.OpenFile(storeFilename, FileMode.Truncate);
                if (null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Hosts));
                    ser.WriteObject(stream, App.nearby.hosts);
                    stream.Close();
                }
            }
            catch (System.Exception) {};
        }
    }
}