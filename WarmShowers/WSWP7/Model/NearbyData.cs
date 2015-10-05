using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using WSApp.Utility;
using WSApp.ViewModel;

namespace WSApp.DataModel
{
    public class NearbyData
    {
        public Hosts hosts { get; set; }
        public HostProfile host { get; private set; }
        public MessageThread messageThread { get; private set; }
        public ViewportCache viewportCache { get; set; }

        public LocationRect locationRect { get; set; }
        public int selectedPage { get; set; }       // so we can return to the same page
        public int selectedUid { get; set; }        // so we can invalidate page if Uid changes
        public int selectedTid { get; set; }        // so we can invalidate page if Tid changes
        public bool AboutPageActive { get; set; }

//        public Map myMap;

        // Constructor
        public NearbyData()
        {
            hosts = new Hosts();
            host = new HostProfile();
            messageThread = new MessageThread();
            viewportCache = new ViewportCache();

            locationRect = null;
            selectedPage = 0;
            selectedUid = 0;
            selectedTid = 0;
            AboutPageActive = false;

            _mapCenter = null;                      
            _isCenteredOnMe = true;
            _meCenter = null;    
        }

        /// <summary>
        /// Map is centered on current location
        /// </summary>
        private bool _isCenteredOnMe;
        public bool isCenteredOnMe
        {
            get
            {
                return _isCenteredOnMe;
            }
            set
            {
                if (value != _isCenteredOnMe)
                {
                    _isCenteredOnMe = value;
                    if (_isCenteredOnMe)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModelMain.pannedAlert = ""; });
                    }
                    else
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModelMain.pannedAlert = WebResources.Panned; });
                    }
                }
            }
        }

        /// <summary>
        /// Center of map
        /// </summary>
        private GeoCoordinate _mapCenter;
        public GeoCoordinate mapCenter
        {
            get
            {
                if (isCenteredOnMe)
                {
                    _mapCenter = _meCenter;
                }
                return _mapCenter;
            }
            set
            {
                if (value != _mapCenter)
                {
                    _mapCenter = value;
                    if (!App.nearby.viewportCache.isMapInitialized)
                    {
                        App.nearby.viewportCache.getHosts();
                    }
                }
            }
        }

        /// <summary>
        /// Current location
        /// </summary>
        private GeoCoordinate _meCenter;
        public GeoCoordinate meCenter
        {
            get
            {
                return _meCenter;
            }
            set
            {
                if (value != _meCenter)
                {
                    _meCenter = value;
                }
            }
        }

        /// <summary>
        /// Get user name, annotated with pinned status
        /// </summary>
        public string getDecoratedUsername(int uId)
        {
            string uName = getFullName(uId);
            if (App.pinned.isPinned()) uName += " - " + WebResources.pinned;

            return uName;
        }

        /// <summary>
        /// Extract latitude
        /// </summary>
        public double getLat(int uId)
        {
            double lat = 0.0;
            bool success = false;

            // First try to get it from results
            if (null != this.hosts.hosts_Result)
            {
                try
                {
                    var accountQuery =
                        from account in this.hosts.hosts_Result.accounts
                        where (account.uid == uId)
                        select account.latitude;

                    lat = accountQuery.First();
                    success = true;
                }
                catch (InvalidOperationException e)
                {   // No match yields exception
                    System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                    System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                }
            }

            // Next try to get it from store of pinned users
            if (!success)
            {
                HostProfile host;
                if (App.pinned.pinnedProfiles.ContainsKey(uId))
                {
                    if (App.pinned.pinnedProfiles.TryGetValue(uId, out host))
                    {   // Name should be in the stored profile, extract it
                        if (null != host.profile.user_Result)
                        {
                            lat = host.profile.user_Result.latitude;

                        }
                    }
                }
            }

            return lat;
        }


        /// <summary>
        /// Extract longitude
        /// </summary>
        public double getLon(int uId)
        {
            double lon = 0.0;
            bool success = false;

            // First try to get it from results
            if (null != this.hosts.hosts_Result)
            {
                try
                {
                    var accountQuery =
                        from account in this.hosts.hosts_Result.accounts
                        where (account.uid == uId)
                        select account.longitude;

                    lon = accountQuery.First();
                    success = true;
                }
                catch (InvalidOperationException e)
                {   // No match yields exception
                    System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                    System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                }
            }

            // Next try to get it from store of pinned users
            if (!success)
            {
                HostProfile host;
                if (App.pinned.pinnedProfiles.ContainsKey(uId))
                {
                    if (App.pinned.pinnedProfiles.TryGetValue(uId, out host))
                    {   // Name should be in the stored profile, extract it
                        if (null != host.profile.user_Result)
                        {

                            lon = host.profile.user_Result.longitude;
                        }
                    }
                }
            }

            return lon;
        }

        /// <summary>
        /// Extract full user name
        /// </summary>
        public string getFullName(int uId)
        {
            string uName = "";

            // First try to get it from results
            if (null != this.hosts.hosts_Result)
            {
                try
                {
                    var accountQuery =
                        from account in this.hosts.hosts_Result.accounts
                        where (account.uid == uId)
                        select account.name;

                    uName = accountQuery.First();
                }
                catch (InvalidOperationException e)
                {   // No match yields exception
                    System.Diagnostics.Debug.WriteLine("Exception Message " + e.Message);
                    System.Diagnostics.Debug.WriteLine("Exception Data " + e.Data);
                }
            }

            // Next try to get it from store of pinned users
            if (string.IsNullOrEmpty(uName))
            {
                HostProfile host;
                if (App.pinned.pinnedProfiles.ContainsKey(uId))
                {
                    if (App.pinned.pinnedProfiles.TryGetValue(uId, out host))
                    {   // First try name in parent class (are we storing name in one too many places?)
                        uName = host.name;

                        if (string.IsNullOrEmpty(uName))
                        {   // Name should be in the stored profile, extract it
                            if (null != host.profile.user_Result)
                            {
                                uName = host.profile.user_Result.fullname;

                                // Fix up the pinned item in the store
                                App.pinned.pinnedProfiles.Remove(uId);
                                host.name = uName;
                                App.pinned.pinnedProfiles.Add(uId, host);
                            }
                        }
                    }
                }
            }

            /*            if (string.IsNullOrEmpty(uName))
                        {   // One last try. In case where we've unpinned and then re-pinned a host that isn't in the current results
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                int uIdHost = App.ViewModelHost.uId;
                                if (uIdHost == uId)
                                {
                                    uName = App.ViewModelHost.username;
                                }
                            });
                        } */
            return uName;
        }

        /// <summary>
        /// Extract host offerings from results
        /// </summary>
        public string getHostMayOffer()
        {
            string offer = "users_Result is null";

            if (null != this.host.profile.user_Result)
            {
                Profile.User user = this.host.profile.user_Result;
                offer = "";
                bool firstLine = true;
                if (user.bed == "1")
                {
                    if (!firstLine)
                    {
                        offer += "\n";
                    }
                    firstLine = false;
                    offer += "\x2022 " + WebResources.Bed;
                }

                if (user.food == "1")
                {
                    if (!firstLine)
                    {
                        offer += "\n";
                    }
                    firstLine = false;
                    offer += "\x2022 " + WebResources.Food;
                }

                if (user.laundry == "1")
                {
                    if (!firstLine)
                    {
                        offer += "\n";
                    }
                    firstLine = false;
                    offer += "\x2022 " + WebResources.Laundry;
                }

                if (user.lawnspace == "1")
                {
                    if (!firstLine)
                    {
                        offer += "\n";
                    }
                    firstLine = false;
                    offer += "\x2022 " + WebResources.LawnSpace;
                }

                if (user.sag == "1")
                {
                    if (!firstLine)
                    {
                        offer += "\n";
                    }
                    firstLine = false;
                    offer += "\x2022 " + WebResources.SAG;
                }

                if (user.shower == "1")
                {
                    if (!firstLine)
                    {
                        offer += "\n";
                    }
                    firstLine = false;
                    offer += "\x2022 " + WebResources.Shower;
                }

                if (user.kitchenuse == "1")
                {
                    if (!firstLine)
                    {
                        offer += "\n";
                    }
                    firstLine = false;
                    offer += "\x2022 " + WebResources.Kitchen;
                }

                if (user.storage == "1")
                {
                    if (!firstLine)
                    {
                        offer += "\n";
                    }
                    firstLine = false;
                    offer += "\x2022 " + WebResources.Storage;
                }
            }

            return offer;
        }

        /// <summary>
        /// Update UI with feedback results
        /// </summary>
        public void loadFeedback()
        {
            if (null != host.feedback.recommendations_Result.recommendations)   // cache may be incomplete
            {
                GreatCircle gc = new GreatCircle();

                Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModelHost.feedbackItems.Clear(); });

                // Handle feedback list being empty
                int len = host.feedback.recommendations_Result.recommendations.Count;
                if (len < 1)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        App.ViewModelHost.feedbackItems.Add(new FeedbackItemViewModel()
                        {
                            Header1 = WebResources.none,
                        });
                    });
                }
                else
                {
                    foreach (var recommendation in host.feedback.recommendations_Result.recommendations)
                    {
                        long date = recommendation.recommendation.field_hosting_date_value;
                        string header = "";

                        header += recommendation.recommendation.field_rating_value;  
                        header += " - ";

                        header += recommendation.recommendation.field_guest_or_host_value;
                        if (date != 0)
                        {
                            date += 86400;  // Fudge to kick us into the next month.  Warm showers server bug?
                            header += " - ";
                            header += gc.date(date);
                        }

                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            App.ViewModelHost.feedbackItems.Add(new FeedbackItemViewModel()
                            {
                                Header1 = header,
                                Header2 = recommendation.recommendation.fullname,
                                Body = recommendation.recommendation.body,
                                userID = recommendation.recommendation.uid
                            });
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Update UI with results common to both Hosts and Profile
        /// </summary>
        public void loadProfileCommon(double lat, double lon, string street, string additional, string city, string province, string country, string postal_code)
        {

            string addr1 = lat + " " + lon;
            string addr2line2 = street;
            if (!string.IsNullOrEmpty(additional))
            {
                if (addr2line2 != "")
                {
                    addr2line2 += '\n';
                }
                addr2line2 += additional;
            }
            string addr2line3 = city + ", " + province.ToUpper() + ", " + country.ToUpper() + " " + postal_code;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModelHost.aboutItems.Add(new AboutItemViewModel() { Line1 = WebResources.AboutCoordinates, Line2 = addr1, Type = AboutItemViewModel.AboutType.address});
                if (!string.IsNullOrEmpty(addr2line2))
                {
                    App.ViewModelHost.aboutItems.Add(new AboutItemViewModel()
                    {
                        Line1 = WebResources.AboutAddress,
                        Line2 = addr2line2,
                        Line3 = addr2line3,
                        Type = AboutItemViewModel.AboutType.address
                    });
                }
            });
        }
        
        /// <summary>
        /// Update UI with profile results
        /// </summary>
        public void loadProfile()
        {
            if (null != host.profile.user_Result)  // cache may be incomplete
            {
                Profile.User user = host.profile.user_Result;
                GeoCoordinate loc = new GeoCoordinate();
                loc.Latitude = user.latitude;
                loc.Longitude = user.longitude;
                GreatCircle gc = new GreatCircle();

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModelHost.aboutItems.Clear();
                    App.ViewModelHost.aboutItems.Add(new AboutItemViewModel()
                    {
                        Line2 = WebResources.MemberFor + " " + gc.TimeSince(user.created),
                        Line3 = WebResources.LastAccess + " " + gc.TimeSince(user.access) + WebResources.ago,
                        Line4 = gc.Distance(loc) + " " + WebResources.FromCurrentLocation,
                        Type = AboutItemViewModel.AboutType.general
                    });
                });

                loadProfileCommon(user.latitude, user.longitude, user.street, user.additional, user.city, user.province, user.country, user.postal_code);

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (!string.IsNullOrEmpty(user.mobilephone))
                    {
                        App.ViewModelHost.aboutItems.Add(new AboutItemViewModel() { Line1 = WebResources.AboutPhone, Line2 = user.mobilephone, Type = AboutItemViewModel.AboutType.phone });
                        App.ViewModelHost.aboutItems.Add(new AboutItemViewModel() { Line1 = WebResources.AboutSMS, Line2 = user.mobilephone, Type = AboutItemViewModel.AboutType.sms });
                    }
                    if (!string.IsNullOrEmpty(user.homephone) && user.homephone != user.mobilephone)
                    {
                        App.ViewModelHost.aboutItems.Add(new AboutItemViewModel() { Line1 = WebResources.AboutHomePhone, Line2 = user.homephone, Type = AboutItemViewModel.AboutType.phone });
                    }
                    if (!string.IsNullOrEmpty(user.workphone) && user.workphone != user.mobilephone && user.workphone != user.homephone)
                    {
                        App.ViewModelHost.aboutItems.Add(new AboutItemViewModel() { Line1 = WebResources.AboutWorkPhone, Line2 = user.workphone, Type = AboutItemViewModel.AboutType.phone });
                    }
                    if (!string.IsNullOrEmpty(user.URL))
                    {
                        App.ViewModelHost.aboutItems.Add(new AboutItemViewModel() { Line1 = WebResources.AboutURL, Line2 = user.URL, Type = AboutItemViewModel.AboutType.web });
                    }
                    if (!string.IsNullOrEmpty(user.picture))
                    {
                        App.ViewModelHost.aboutItems.Add(new AboutItemViewModel() { Picture = user.profile_image_mobile_photo_456, Type = AboutItemViewModel.AboutType.picture });

                    }
                    App.ViewModelHost.aboutItems.Add(new AboutItemViewModel() { Line1 = WebResources.AboutComments, Line2 = user.comments, Type = AboutItemViewModel.AboutType.comments });

                    App.ViewModelHost.hostingItems.Clear();
                    if (!string.IsNullOrEmpty(user.languagesspoken))
                    {
                        App.ViewModelHost.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.HostingHeaderLanguagesSpoken, Line2 = user.languagesspoken });
                    }
                    if (!string.IsNullOrEmpty(user.preferred_notice))
                    {
                        App.ViewModelHost.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.HostingHeaderPreferred_notice, Line2 = user.preferred_notice });
                    }
                    if (!string.IsNullOrEmpty(user.maxcyclists))
                    {
                        App.ViewModelHost.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.HostingHeaderMaxCyclists, Line2 = user.maxcyclists });
                    }
                    if (!string.IsNullOrEmpty(user.bikeshop))
                    {
                        App.ViewModelHost.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.HostingHeaderBikeShop, Line2 = user.bikeshop });
                    }
                    if (!string.IsNullOrEmpty(user.campground))
                    {
                        App.ViewModelHost.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.HostingHeaderCampground, Line2 = user.campground });
                    }
                    if (!string.IsNullOrEmpty(user.motel))
                    {
                        App.ViewModelHost.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.HostingHeaderMotel, Line2 = user.motel });
                    }
                    string offer = getHostMayOffer();
                    if (!string.IsNullOrEmpty(offer))
                    {
                        App.ViewModelHost.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.HostingHeaderHostMayOffer, Line2 = offer });
                    }
                    // Handle hosting list being empty
                    if (App.ViewModelHost.hostingItems.Count < 1)
                    {
                        App.ViewModelHost.hostingItems.Add(new HostingItemViewModel() { Line1 = WebResources.none });
                    }
                });
            }
        }

        /// <summary>
        /// Update UI with hosts results
        /// </summary>
        public void loadHosts()
        {
            if (null == App.nearby.hosts) return;
            else if (null == App.nearby.hosts.hosts_Result) return;
            Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModelMain.nearbyItems.Clear(); });

            if (!App.settings.isLocationEnabled)
            {   // User deliberately disabled location service
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModelMain.nearbyItems.Add(new NearbyItemViewModel()
                    {
                        Name = WebResources.NearbyListLocationDisabledHeader,
                        Line2 = WebResources.NearbyListLocationDisabledBody
                    });
                });
                return;
            }

            int len = hosts.hosts_Result.accounts.Count;
            if (len < 1)
            {   // Display empty list message to user and return
                // Todo:  Filter feature
                //                if (App.ViewModelFilter.isFiltered())
                //                {   // With filter active message
                //                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                //                    {
                //                        App.ViewModelMain.nearbyItems.Add(new NearbyItemViewModel()
                //                        {
                //                            Name = WebResources.NearbyListEmptyHeader,
                //                            Line2 = WebResources.NearbyListEmptyBody1 + "\n\n" +
                //                                WebResources.NearbyListEmptyBody2 + "\n\n" + WebResources.NearbyListEmptyBody3
                //                        });
                //                    });
                //                }
                //                else
                {   // Without filter active message
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        App.ViewModelMain.nearbyItems.Add(new NearbyItemViewModel()
                        {
                            Name = WebResources.NearbyListEmptyHeader,
                            Line2 = WebResources.NearbyListEmptyBody1 + "\n\n" + WebResources.NearbyListEmptyBody3
                        });
                    });
                }
                return;
            }

            // Add hosts to main list
            List<MapCoord> mapCoords = new List<MapCoord>();
            GreatCircle gc = new GreatCircle();
            foreach (var account in hosts.hosts_Result.accounts)
            {
                MapCoord coord = new MapCoord();
                coord.geoCoord = new GeoCoordinate(account.latitude, account.longitude);
                coord.userName = account.name;
                coord.uId = account.uid;
                if (App.pinned.isPinned(coord.uId))
                {
                    coord.type = PushpinType.pinned;
                }
                else
                {
                    coord.type = PushpinType.unpinned;
                }
                mapCoords.Add(coord);

                App.nearby.viewportCache.hostLocation(coord.geoCoord);

                string distance = gc.Distance(coord.geoCoord);
                double d = gc.dDistance(coord.geoCoord);
                if (App.pinned.isPinned(account.uid))
                {
                    distance += " - " + WebResources.pinned;
                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                { App.ViewModelMain.nearbyItems.Add(new NearbyItemViewModel() { Name = account.name, Line2 = distance, userID = account.uid, Distance = d }); });
            }

            Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModelMain.mapItems.Clear(); });

            // Add hosts to map
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (MapCoord coord in mapCoords)
                {
                    App.ViewModelMain.mapItems.Add(new MapItemViewModel() { location = coord.geoCoord, username = coord.userName, Type = coord.type, userID = coord.uId });
                }
            });
        }

        /// <summary>
        /// Update UI with messages 
        /// </summary>
        public void loadMessages()
        {
            if (null != host.messages.messages_result)
            {
                if (null != host.messages.messages_result.messages)  // cache may be incomplete
                {
                    GreatCircle gc = new GreatCircle();

                    Deployment.Current.Dispatcher.BeginInvoke(() => { App.ViewModelHost.messageItems.Clear(); });

                    // Handle messages list being empty
                    int len = host.messages.messages_result.messages.Count;
                    if (len < 1)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            App.ViewModelHost.messageItems.Add(new MessagesItemViewModel()
                            {
                                Header1 = WebResources.none,
                            });
                        });
                    }
                    else
                    {
                        foreach (var message in host.messages.messages_result.messages)
                        {
                            long date = message.last_updated;
                            string header = gc.date_mmmddyyyy(date);

                            if (1 == message.is_new)    
                            {
                                header += " " + message.is_new.ToString() + " " + WebResources.NewMessageReceived;
                            }
                            else if (1 < message.is_new)    // is_new represents the number of new messages, it's not a new message boolean!
                            {
                                header += " " + message.is_new.ToString() + " " + WebResources.NewMessagesReceived;
                            }

                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                App.ViewModelHost.messageItems.Add(new MessagesItemViewModel()
                                {
                                    Header1 = message.subject,
                                    Header2 = header,
                                    threadID = message.thread_id
                                });
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update UI with message 
        /// </summary>
        public void loadMessage()
        {
            GreatCircle gc = new GreatCircle();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {            
                App.ViewModelMessage.messageItems.Clear();

                // Handle message list being empty
                int len = messageThread.message_result.messages.Count();
                if (len < 1)
                {
                    App.ViewModelMessage.messageItems.Add(new MessageItemViewModel()
                    {
                        Header1 = WebResources.none,
                    });
                }
                else
                {
                    App.ViewModelMessage.subject = messageThread.message_result.subject;
                    for (int i = len - 1; i >= 0; i--)
                    {   // Display most recent message first
                        MessageThread.Message message = messageThread.message_result.messages[i];

                        long date = message.timestamp;

                        //Lookup author fullname
                        int uid;
                        int.TryParse(message.author, out uid);
                        string author = getFullName(uid);

                        string header = WebResources.SentBy + " " + author + " " + WebResources.On + " " + gc.date_mmmddyyyy(date);
                        if (1 == message.is_new) header += " - " + WebResources.New;

                        App.ViewModelMessage.messageItems.Add(new MessageItemViewModel()
                        {
                            Header1 = header,
                            Body = message.body
                        });
                    }
                }
            });           
        }

        public bool loadHostFromStorage(int uId)
        {
            HostProfile host2 = App.pinned.getHost(uId);
            if (null != host2)
            {
                host = host2;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
