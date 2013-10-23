using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.Runtime.Serialization;
using WSApp.ViewModel;
using WSApp.Utility;

using System.Windows.Media;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;

namespace WSApp.DataModel
{
    public class MapCoord
    {
        public GeoCoordinate geoCoord;
        public string userName;
        public int uId;
        public PushpinType type;
    }

    [DataContract]
    public class HostProfile
    {   // Unit of persistence for pinned hosts

        // For UI, because we don't always have the complete profile stored
        [DataMember]
        public long lastUpdateTime;
        [DataMember]
        public int uId;
        [DataMember]
        public string name;

        [DataMember]
        public Profile profile;
        [DataMember]
        public Feedback feedback;
        [DataMember]
        public Messages messages;

        // Constructor
        public HostProfile()
        {
            lastUpdateTime = 0;
            uId = 0;
            name = "";

            profile = new Profile();
            feedback = new Feedback();
            messages = new Messages();
        }

        public HostProfile(HostProfile newHost)
        {
            lastUpdateTime = newHost.lastUpdateTime;
            uId = newHost.uId;
            name = newHost.name;

            profile = new Profile(newHost.profile);
            feedback = new Feedback(newHost.feedback);
            messages = new Messages(newHost.messages);
        }
    }
}