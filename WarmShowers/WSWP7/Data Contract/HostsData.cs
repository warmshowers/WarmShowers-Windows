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

    // new query by location (contains a little more data?)
    // {"status":{"delivered":11,"totalresults":"11","status":"complete"},"query_data":{"sql":"SELECT u.uid, u.created, u.access, u.login, w.fullname name, w.fullname AS fullname, street, city, province, postal_code, country, latitude, longitude,\n    source, picture, notcurrentlyavailable,\n    DEGREES(ACOS(SIN(RADIANS(%f)) * SIN(RADIANS(latitude))+COS(RADIANS(%f)) * COS(RADIANS(latitude)) * COS(RADIANS(%f-longitude)))) * 60 AS distance,\n    CONCAT(latitude, ',', longitude) AS position\n    FROM {users} u, {user_location} l, {wsuser} w\n    WHERE latitude > %f AND latitude < %f AND longitude >%f AND longitude <%f \n    AND u.uid = l.oid AND u.uid=w.uid AND u.status>0 AND !w.isunreachable AND !w.isstale\n    AND !notcurrentlyavailable\n    AND u.uid NOT IN (SELECT ur.uid FROM users_roles ur WHERE ur.rid = 9)\n\t\tORDER BY distance ASC","centerlat":"47.669444","centerlon":"-122.123889","minlat":"47.6102242881867","maxlat":"47.7285965930161","minlon":"-122.211779625","maxlon":"-122.035998375","limit":"100","duration":0.002695},
    // "accounts":[{"uid":"88175","created":"1430065228","access":"1434640521","login":"1434640521","name":"chong kim ","fullname":"chong kim ","street":"","city":"Redmond","province":"wa","postal_code":"98052","country":"us","latitude":"47.670119","longitude":"-122.118237","source":"1","picture":"files/pictures/picture-88175.jpg","notcurrentlyavailable":"0","distance":"0.2319277567224286","position":"47.670119,-122.118237","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-88175.jpg","profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-88175.jpg"},
    // {"uid":"2492","created":"1139592790","access":"1436046750","login":"1436046300","name":"Carry Porter / Nick Brown","fullname":"Carry Porter / Nick Brown","street":"13024 NE 112th St","city":"Kirkland","province":"wa","postal_code":"98033","country":"us","latitude":"47.701281","longitude":"-122.166518","source":"2","picture":"files/pictures/picture-2492.jpg","notcurrentlyavailable":"0","distance":"2.5717291885540554","position":"47.701281,-122.166518","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-2492.jpg","profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-2492.jpg"},{"uid":"62026","created":"1395264512","access":"1395264759","login":"1395264578","name":"Randy Oakley","fullname":"Randy Oakley","street":"1435 170TH PL NE","city":"BELLEVUE","province":"wa","postal_code":"98008","country":"us","latitude":"47.623516","longitude":"-122.113089","source":"2","picture":"","notcurrentlyavailable":"0","distance":"2.7900459692525805","position":"47.623516,-122.113089"},{"uid":"56334","created":"1383467661","access":"1438489437","login":"1438450312","name":"Elliott Ridgway","fullname":"Elliott Ridgway","street":"3660 116th ave NE ","city":"Bellevue","province":"wa","postal_code":"98004","country":"us","latitude":"47.643725","longitude":"-122.185400","source":"2","picture":"files/pictures/picture-56334.jpg","notcurrentlyavailable":"0","distance":"2.9259389414702697","position":"47.643725,-122.185400","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-56334.jpg","profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-56334.jpg"},{"uid":"55308","created":"1381293622","access":"1413918085","login":"1413918084","name":"Miles Morris","fullname":"Miles Morris","street":"10901 NE 39th Place","city":"Bellevue","province":"wa","postal_code":"98004","country":"us","latitude":"47.645816","longitude":"-122.193809","source":"2","picture":"","notcurrentlyavailable":"0","distance":"3.1614056851589294","position":"47.645816,-122.193809"},{"uid":"54635","created":"1379960993","access":"1395631586","login":"1395631586","name":"Diana Lind","fullname":"Diana Lind","street":"15735 NE 1ST ST","city":"BELLEVUE","province":"wa","postal_code":"98008","country":"us","latitude":"47.611231","longitude":"-122.129800","source":"2","picture":"files/pictures/picture-54635.jpg","notcurrentlyavailable":"0","distance":"3.500944976240128","position":"47.611231,-122.129800","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-54635.jpg","profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-54635.jpg"},{"uid":"54650","created":"1379980773","access":"1428343618","login":"1427520929","name":"Jesper Lind","fullname":"Jesper Lind","street":"15735 NE 1ST ST","city":"Bellevue","province":"wa","postal_code":"98008","country":"us","latitude":"47.611231","longitude":"-122.129800","source":"2","picture":"","notcurrentlyavailable":"0","distance":"3.500944976240128","position":"47.611231,-122.129800"},{"uid":"37915","created":"1346373501","access":"1402273522","login":"1402273522","name":"Gary Meyers","fullname":"Gary Meyers","street":"","city":"Kirkland","province":"wa","postal_code":"98033","country":"us","latitude":"47.681488","longitude":"-122.208735","source":"5","picture":"files/pictures/picture-37915.png","notcurrentlyavailable":"0","distance":"3.5031023888048116","position":"47.681488,-122.208735","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-37915.png","profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-37915.png"},{"uid":"65878","created":"1400651340","access":"1402347761","login":"1402347852","name":"Freya Alexandra","fullname":"Freya Alexandra","street":"","city":"Bellevue","province":"wa","postal_code":"98007","country":"us","latitude":"47.610543","longitude":"-122.142426","source":"3","picture":"files/pictures/picture-65878.jpg","notcurrentlyavailable":"0","distance":"3.6126415392641373","position":"47.610543,-122.142426","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-65878.jpg","profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-65878.jpg"},{"uid":"28956","created":"1321396910","access":"1421773751","login":"1421773751","name":"Eduardo Gonzalez","fullname":"Eduardo Gonzalez","street":"","city":"Kirkland","province":"wa","postal_code":"98033","country":"us","latitude":"47.718780","longitude":"-122.196570","source":"3","picture":"files/pictures/picture-28956.png","notcurrentlyavailable":"0","distance":"4.168717565556173","position":"47.718780,-122.196570","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-28956.png","profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-28956.png"},{"uid":"53452","created":"1377868005","access":"1420205444","login":"1420205241","name":"Joe and Mare Sullivan","fullname":"Joe and Mare Sullivan","street":"13829  101 Place NE","city":"Kirkland","province":"wa","postal_code":"98034","country":"us","latitude":"47.724931","longitude":"-122.205965","source":"2","picture":"","notcurrentlyavailable":"0","distance":"4.697808798724525","position":"47.724931,-122.205965"}]}


    [DataContract]
    public class Hosts
    {
        [DataMember]
        public Hosts_Result hosts_Result;
      
        // Constructor
        public Hosts()
        {

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

        public class Status
        {
            [DataMember]
            public string delivered { get; set; }

            [DataMember]
            public int totalresults { get; set; }

            [DataMember]
            public string status { get; set; }
        }

        /// <summary>
        /// Abbreviated user account info that comes down in POST /services/rest/hosts/by_location
        /// </summary>
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
    }
}
 

