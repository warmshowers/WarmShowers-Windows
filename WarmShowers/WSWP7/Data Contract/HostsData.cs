using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WSApp.DataModel
{

// {"status":{"delivered":73,"totalresults":"73","status":"complete"},"query_data":{"sql":"SELECT u.uid, u.created, u.access, u.login, u.name AS name, w.fullname AS fullname, street, city, province, postal_code, country, latitude, longitude,\n    source, picture, notcurrentlyavailable,\n    DEGREES(ACOS(SIN(RADIANS(:centerlat)) * SIN(RADIANS(latitude))+COS(RADIANS(:centerlat)) * COS(RADIANS(latitude)) * COS(RADIANS(:centerlon - longitude)))) * 60 AS distance,\n    CONCAT(latitude, ',', longitude) AS position\n    FROM {users} u, {user_location} l, {wsuser} w\n    WHERE latitude > :minlat AND latitude < :maxlat AND longitude > :minlon AND longitude < :maxlon \n    AND u.uid = l.oid AND u.uid = w.uid AND u.status > 0\n    AND !notcurrentlyavailable\n    AND u.uid NOT IN (SELECT ur.uid FROM users_roles ur WHERE ur.rid = 9)\n\t\tORDER BY distance ASC","centerlat":"47.669444","centerlon":"-122.123889","minlat":"47.4915834571513","maxlat":"47.8467004753047","minlon":"-122.288683921875","maxlon":"-121.959094078125","limit":"100"},
    // "accounts":[
// {"uid":"88175","created":"1430065228","access":"1442161872","login":"1442161871","name":"chk1","fullname":"chong kim ","street":"","city":"Redmond","province":"wa",
//  "postal_code":"98052","country":"us","latitude":"47.670119","longitude":"-122.118237","source":"1","picture":"163735","notcurrentlyavailable":"0","distance":"0.2319277567224286",
//  "position":"47.670119,-122.118237","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/styles/mobile_profile_photo_std/public/pictures/picture-88175.jpg?itok=laH2dmRW",
//  "profile_image_profile_picture":"https://www.warmshowers.org/files/styles/profile_picture/public/pictures/picture-88175.jpg?itok=DIGedPG1",
//  "profile_image_mobile_photo_456":"https://www.warmshowers.org/files/styles/mobile_photo_456/public/pictures/picture-88175.jpg?itok=Vw3HZaZw",
//  "profile_image_map_infoWindow":"https://www.warmshowers.org/files/styles/map_infoWindow/public/pictures/picture-88175.jpg?itok=cOD3zdhd"},
// {"uid":"2492","created":"1139592790","access":"1436046750","login":"1436046300","name":"Carry and Nick","fullname":"Carry Porter / Nick Brown","street":"13024 NE 112th St","city":"Kirkland",
//  "province":"wa","postal_code":"98033","country":"us","latitude":"47.701281","longitude":"-122.166518","source":"2","picture":"130986","notcurrentlyavailable":"0","distance":"2.5717291885540554",
//  "position":"47.701281,-122.166518","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/styles/mobile_profile_photo_std/public/pictures/picture-2492.jpg?itok=WVxAQAgj",
//  "profile_image_profile_picture":"https://www.warmshowers.org/files/styles/profile_picture/public/pictures/picture-2492.jpg?itok=P2XIqoLd",
//  "profile_image_mobile_photo_456":"https://www.warmshowers.org/files/styles/mobile_photo_456/public/pictures/picture-2492.jpg?itok=QIiutOpy",
//  "profile_image_map_infoWindow":"https://www.warmshowers.org/files/styles/map_infoWindow/public/pictures/picture-2492.jpg?itok=tNhPhtp4"},
// {"uid":"62026","created":"1395264512","access":"1440522744","login":"1440522558","name":"RandyOakley","fullname":"Randy Oakley","street":"1435 170TH PL NE","city":"BELLEVUE","province":"wa",
//  "postal_code":"98008","country":"us","latitude":"47.623516","longitude":"-122.113089","source":"2","picture":"0","notcurrentlyavailable":"0","distance":"2.7900459692525805",
//  "position":"47.623516,-122.113089","profile_image_mobile_profile_photo_std":"","profile_image_profile_picture":"","profile_image_mobile_photo_456":"","profile_image_map_infoWindow":""},

    [DataContract]
    public class Hosts
    {
        [DataMember]
        public Hosts_Result hosts_Result;
      
        // Constructor
        public Hosts()
        {

        }


        /*
        public class QueryData 
        {


            [DataMember]
            public string sql { get; set; }

            [DataMember]
            public string centerlat { get; set; }

            [DataMember]
            public string centerlon { get; set; }

            [DataMember]
            public string minlat { get; set; }

            [DataMember]
            public string maxlat { get; set; }

            [DataMember]
            public string minlon { get; set; }

            [DataMember]
            public string maxlon { get; set; }

            [DataMember]
            public string limit { get; set; }

            [DataMember]
            public double duration { get; set; }
        }
         * */

        public class Status
        {
            [DataMember]
            public string delivered { get; set; }

            [DataMember]
            public int totalresults { get; set; }

            [DataMember]
            public string status { get; set; }
        }

        public class Account
        {
            [DataMember]
            public int uid { get; set; }

            [DataMember]
            public string name { get; set; }

            [DataMember]
            public string fullname { get; set; }

            [DataMember]
            public string street { get; set; }

            [DataMember]
            public string city { get; set; }

            // This gets filled when complete profile comes down
            public string additional = "";

            [DataMember]
            public string province { get; set; }

            [DataMember]
            public string postal_code { get; set; }

            [DataMember]
            public string country { get; set; }

            [DataMember]
            public double latitude { get; set; }

            [DataMember]
            public double longitude { get; set; }

//            [DataMember]
//            public int source { get; set; }

            [DataMember]
            public string notcurrentlyavailable { get; set; }

            [DataMember]
            public double distance { get; set; }
        }

        public class Hosts_Result
        {
//            [DataMember]
//            public QueryData query_data { get; set; }

            [DataMember]
            public Status status { get; set; }

            [DataMember]
            public List<Account> accounts { get; set; }
        }
    }
}
 

