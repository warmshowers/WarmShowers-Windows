using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WSApp.DataModel
{

    /*
        {"query_data":{"sql":"SELECT u.uid, w.fullname name, street, city, province, postal_code, country, latitude, longitude,
        *    \n    source, notcurrentlyavailable,
        *    \n    DEGREES(ACOS(SIN(RADIANS(%f)) * SIN(RADIANS(latitude))+COS(RADIANS(%f)) * COS(RADIANS(latitude)) * COS(RADIANS(%f-longitude)))) * 60 AS distance
        *    \n    FROM {users} u, {user_location} l, {wsuser} w\n    WHERE latitude > %f AND latitude < %f AND (longitude < %f OR longitude > %f)
        *    \n    AND u.uid = l.oid AND u.uid=w.uid AND u.status>0 AND !w.isunreachable AND !w.isstale\n    AND !notcurrentlyavailable
        *    \n    AND u.uid NOT IN (SELECT ur.uid FROM users_roles ur WHERE ur.rid = 9)\n\t\tORDER BY distance ASC",
        *    "centerlat":"47.64483","centerlon":"-122.141197","minlat":"36.43861111","maxlat":"50.82777778","minlon":"-106.3225","maxlon":"-126.1094444","limit":"2000","duration":-0.139603},
        *    "status":{"delivered":"","totalresults":"10111","status":"complete"},
        *    "accounts":[{"uid":"24301","name":"Isabelle Salmon","street":"","city":"Redmond","province":"wa","postal_code":"98052","country":"us","latitude":"47.670119","longitude":"-122.118237","source":"3","notcurrentlyavailable":"0","distance":"1.778571158329679"},
        *                {"uid":"2492","name":"Carry Porter / Nick Brown","street":"8122 126th Avenue NE","city":"Kirkland","province":"wa","postal_code":"98033","country":"us","latitude":"47.676619","longitude":"-122.172553","source":"2","notcurrentlyavailable":"0","distance":"2.28988430956231"}]}

        * 
        * 
        {
        "query_data":{
            "sql":"SELECT u.uid, w.fullname name, street, city, province, postal_code, country, latitude, longitude,\n    source, notcurrentlyavailable,\n    DEGREES(ACOS(SIN(RADIANS(%f)) * SIN(RADIANS(latitude))+COS(RADIANS(%f)) * COS(RADIANS(latitude)) * COS(RADIANS(%f-longitude)))) * 60 AS distance\n    FROM {users} u, {user_location} l, {wsuser} w\n    WHERE latitude > %f AND latitude < %f AND (longitude < %f OR longitude > %f)\n    AND u.uid = l.oid AND u.uid=w.uid AND u.status>0 AND !w.isunreachable AND !w.isstale\n    AND !notcurrentlyavailable\n    AND u.uid NOT IN (SELECT ur.uid FROM users_roles ur WHERE ur.rid = 9)\n\t\tORDER BY distance ASC",
            "centerlat":"47.64483",
            "centerlon":"-122.141197",
            "minlat":"36.43861111",
            "maxlat":"50.82777778",
            "minlon":"-106.3225",
            "maxlon":"-126.1094444",
            "limit":"25",
            "duration":0.146902
        },
        "status":{
            "delivered":"",
            "totalresults":"10115",
            "status":"complete"
        },
        "accounts":[
            {
                "uid":"24301",
                "name":"Isabelle Salmon",
                "street":"",
                "city":"Redmond",
                "province":"wa",
                "postal_code":"98052",
                "country":"us",
                "latitude":"47.670119",
                "longitude":"-122.118237",
                "source":"3",
                "notcurrentlyavailable":"0",
                "distance":"1.778571158329679"
            },
            {
                "uid":"2492",
                "name":"Carry Porter / Nick Brown",
                "street":"8122 126th Avenue NE",
                "city":"Kirkland",
                "province":"wa",
                "postal_code":"98033",
                "country":"us",
                "latitude":"47.676619",
                "longitude":"-122.172553",
                "source":"2",
                "notcurrentlyavailable":"0",
                "distance":"2.28988430956231"
            }
        ]
    }
    */

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
 

