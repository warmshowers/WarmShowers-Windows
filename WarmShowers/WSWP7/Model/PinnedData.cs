using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Windows;
using WSApp.Utility;
using WSApp.ViewModel;

namespace WSApp.DataModel
{
    [DataContract]
    public class PinnedData
    {
        [DataMember]
        public Dictionary<int, HostProfile> pinnedProfiles { get; set; }

        // Constructor
        public PinnedData()
        {
            pinnedProfiles = new Dictionary<int, HostProfile>();
        }

        /// <summary>
        /// Is specified host currently pinned
        /// </summary>
        public bool isPinned(int uId)
        {
            return (pinnedProfiles.ContainsKey(uId));
        }

        /// <summary>
        /// Is active host currently pinned
        /// </summary>
        public bool isPinned()
        {
            return (pinnedProfiles.ContainsKey(App.nearby.selectedUid));
        }

        public HostProfile getHost(int uId)
        {
            HostProfile host;
            if (pinnedProfiles.ContainsKey(uId))
            {
                if (pinnedProfiles.TryGetValue(uId, out host))
                {
                    return host;
                }
            }
            return null;
        }

        /// <summary>
        /// Got new message from host, update new message count and autopin
        /// </summary>
        public void newMessage(int uId, string name, int count)
        {
            autoPin(uId, name);

            // Update UI (must do this after autoPin)
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModelMain.SetNewMessageCount(uId, count);
            });
        }

        /// <summary>
        /// Pin a host that isn't currently selected
        /// </summary>
        public bool autoPin(int uId, string name)
        {

            HostProfile host = null;
            bool isHostValid = false;

            if (isPinned(uId))
            {   // Update with existing pinned profile
                isHostValid = pinnedProfiles.TryGetValue(uId, out host);
            }
            if (!isHostValid)
            {   // Empty profile, will be filled first time this host is selected
                host = new HostProfile();
            }

            host.uId = uId;

            // Might be able to get a better name
            string n; ;
            if ("" != (n = App.nearby.getFullName(uId)))
            {
                host.name = n;
            }
            else
            {
                host.name = name;
            }

            return pin(host);
        }

        /// <summary>
        /// Explicitly pin the currently selected host
        /// </summary>
        public bool pin()
        {
            HostProfile host = new HostProfile(App.nearby.host);
            return pin(host);
        }

        /// <summary>
        /// Common pin functionality
        /// </summary>
        private bool pin(HostProfile host)
        {
            int uId = host.uId;
            GreatCircle gc = new GreatCircle();

            if (pinnedProfiles.Count == 0)
            {   // Clear the "no pinned hosts" help message
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModelMain.pinnedItems.Clear();
                });
            }

            host.lastUpdateTime = gc.Now();

            // Deal with case where host is already pinned
            if (isPinned(uId))
            {
                pinnedProfiles.Remove(uId);
            }
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModelMain.unPin(uId);
            });

            // Add pinned items to store and pinnedList UI
            pinnedProfiles.Add(uId, host);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModelMain.Pin(uId, host.name, host.lastUpdateTime);
                App.ViewModelHost.Pin(uId);
            });

            return true;
        }

        public bool unPin(int uId)
        {
            pinnedProfiles.Remove(uId);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModelMain.unPin(uId);
                App.ViewModelHost.unPin(uId);
            });

            return true;
        }

        public void update(int uId, long time)
        {
            GreatCircle gc = new GreatCircle();

            HostProfile host = null;
            if (pinnedProfiles.ContainsKey(uId))
            {
                if (pinnedProfiles.TryGetValue(uId, out host))
                {
                    if (null != host)
                    {
                        host.lastUpdateTime = time;
                        //  pinnedProfiles.Add(uId, host);  // Todo:  I don't think we need to add it back????
                        App.ViewModelMain.UpdateTimestampPinned(uId, time);
                    }
                }
            }
        }

        /// <summary>
        /// Updtate store with freshly-downloaded host profile, if pinned
        /// </summary>
        public void refreshHost()
        {
            int uId = App.nearby.host.profile.user_Result.uid;

            if (isPinned(uId))
            {
                HostProfile host = getHost(uId);
                if (null != host)
                {   // Maintain timestamp of the pinned copy
                    App.nearby.host.lastUpdateTime = host.lastUpdateTime;
                }
                pinnedProfiles.Remove(uId);
                pinnedProfiles.Add(uId, new HostProfile(App.nearby.host));
            
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {   // We now have the fullname, update it in pinned UI in main pivot...
                    App.ViewModelMain.UpdateUsernamePinned(uId, host.name);

                    //... and the header name in host pivot
                    App.ViewModelHost.username = host.name + " - " + WebResources.pinned;
                });
            }
        }
    }
}
