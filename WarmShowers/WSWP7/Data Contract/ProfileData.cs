using System.Collections.Generic;
using System.Runtime.Serialization;

// Old data from deprecated https://warmshowers.org/user/14861/json
// {"users":[{"user":{"name":"klzig","uid":"14861","fullname":"Kurt Ziegler","login":"1438489167","access":"1438488997","created":"1258696215","street":"1405 N 22nd St","city":"Boise",
// "province":"id","postal_code":"83702","country":"us","mobilephone":"208.859.6295","comments":"I'm a year-round commuter and summer touring cyclist. I enjoy self-organized tours as well as the occasional professional tour. My daughter and I recently completed an epic cycling adventure. Would enjoy hosting up to three people and hearing about their adventures. My wife, daughter and I live in North Boise where the house is ruled by a surly but mostly harmless cat and a dog or two.\r\n\r\nPlease email before calling.",
// "preferred_notice":"Please email before calling","maxcyclists":"3","notcurrentlyavailable":"1","bed":"1","bikeshop":"13 blocks","campground":"5 miles","food":"1","kitchenuse":"1",
// "laundry":"1","lawnspace":"1","motel":"6 blocks","sag":"1","shower":"1","storage":"0","latitude":"43.633213","longitude":"-116.215910",
// "picture":"files\/pictures\/picture-14861.jpg","languagesspoken":"English","URL":"https:\/\/trike2012.wordpress.com\/"}}]}


// New data from https://warmshowers.org/services/rest/user/14861
//{"uid":"14861","name":"klzig","mode":"0","sort":"0","threshold":"0","theme":"","created":"1258696215","access":"1438489339","status":"1","timezone":"-21600","language":"en-working",
// "picture":"files/pictures/picture-14861.jpg","login":"1438489167","timezone_name":"America/Denver","signature_format":"0","fullname":"Kurt Ziegler",
// "comments":"I'm a year-round commuter and summer touring cyclist. I enjoy self-organized tours as well as the occasional professional tour.  My daughter and I recently completed an epic cycling adventure.  Would enjoy hosting up to three people and hearing about their adventures.  My wife, daughter and I live in North Boise where the house is ruled by a surly but mostly harmless cat and a dog or two.\r\n\r\nPlease email before calling.",
// "notcurrentlyavailable":"1","homephone":"","mobilephone":"208.859.6295","workphone":"","fax_number":"","preferred_notice":"Please email before calling","maxcyclists":"3",
// "motel":"6 blocks","campground":"5 miles","bikeshop":"13 blocks","languagesspoken":"English","URL":"https://trike2012.wordpress.com/","bed":"1","food":"1","laundry":"1",
// "lawnspace":"1","sag":"1","shower":"1","storage":"0","kitchenuse":"1","picture_delete":0,"picture_upload":"","form_build_id":"form-W5yVFHwUaVvlFoAwyftE6VwLqDZ-vr9E8iwERJRiNwI",
// "set_unavailable_timestamp":"1438267366","set_available_timestamp":"0","last_unavailability_pester":"1438267366","becomeavailable":"1439359200","email_opt_out":"0",
// "notcurrentlyavailable_reason":null,"cost":"","lastcorrespondence":null,"isstale":"0","isstale_date":"0","isstale_reason":null,"isunreachable":"0","unreachable_date":"0",
// "unreachable_reason":null,"hide_donation_status":null,"oid":"14861","type":"","street":"1405 N 22nd St","additional":"","city":"Boise","province":"id","postal_code":"83702","country":"us",
// "latitude":"43.633213","longitude":"-116.215910","source":"1","node_notify_mailalert":"1","comment_notify_mailalert":"1","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-14861.jpg",
// "profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-14861.jpg"}


// query by keyword:
//  {"status":{"delivered":2,"totalresults":"2","status":"complete"},"query_data":{"sql":"SELECT * FROM {users} u, {wsuser} w, {user_location} l\n            WHERE u.uid = w.uid\n            AND u.uid=l.oid\n            AND status > 0\n            AND !w.isstale AND !w.isunreachable\n            AND u.uid NOT IN (SELECT ur.uid FROM users_roles ur WHERE ur.rid = 9)\n            AND (LOWER(w.fullname) LIKE '%%%s%%' OR u.name LIKE '%%%s%%'\n            OR l.city LIKE '%%%s%%'\n            OR u.mail = '%s%%')  ORDER BY w.notcurrentlyavailable ASC",
// "keyword":"klzig","offset":0,"limit":"25"},
// "accounts":{"14861":{"uid":"14861","name":"klzig","mode":"0","sort":"0","threshold":"0","theme":"","signature":"","created":"1258696215","access":"1439361657",
// "status":"1","timezone":"-21600","language":"en-working","picture":"files/pictures/picture-14861.jpg","login":"1439361657",
// "timezone_name":"America/Denver","signature_format":"0","fullname":"Kurt Ziegler","notcurrentlyavailable":"0","notcurrentlyavailable_reason":null,
// "fax_number":"","mobilephone":"208.859.6295","workphone":"+1! 208-859-6295","homephone":"+1 208-859-6295",
// "preferred_notice":"Please email before calling","cost":"","maxcyclists":"3","storage":"0","motel":"6 blocks","campground":"5 miles","bikeshop":"13 blocks",
// "comments":"I'm a year-round commuter and summer touring cyclist. I enjoy self-organized tours as well as the occasional professional tour.  My daughter and I recently completed an epic cycling adventure.  Would enjoy hosting up to three people and hearing about their adventures.  My wife, daughter and I live in North Boise where the house is ruled by a surly but mostly harmless cat and a dog or two.\r\n\r\nPlease email before calling.",
// "shower":"1","kitchenuse":"1","lawnspace":"1","sag":"1","bed":"1","laundry":"1","food":"1","howdidyouhear":"friends & family","lastcorrespondence":null,
// "languagesspoken":"English","URL":"https://trike2012.wordpress.com/","isstale":"0","isstale_date":"0","isstale_reason":null,"isunreachable":"0","unreachable_date":"0",
// "unreachable_reason":null,"becomeavailable":"1439359200","set_unavailable_timestamp":"0","set_available_timestamp":"1438503926","last_unavailability_pester":"0",
// "hide_donation_status":null,"email_opt_out":"0","oid":"14861","type":"","street":"1405 N 22nd St","additional":"testing","city":"Boise","province":"id",
// "postal_code":"83702","country":"us","latitude":"43.633213","longitude":"-116.215910","source":"1",
// "profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-14861.jpg",
// "profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-14861.jpg"},
//"98313":{"uid":"98313","name":"klzig_test","mode":"0","sort":"0","threshold":"0","theme":"","signature":"","created":"1438580975","access":"1438667415","status":"1","timezone":"-21600","language":"en-working","picture":"","login":"1438581524","timezone_name":"America/Denver","signature_format":"0","fullname":"Kurt Ziegler Test","notcurrentlyavailable":"1","notcurrentlyavailable_reason":null,"fax_number":"","mobilephone":"","workphone":"","homephone":"","preferred_notice":"","cost":"","maxcyclists":"1","storage":"0","motel":"","campground":"","bikeshop":"","comments":"This is a test account for validating the Warm Showers for Windows Phone app.  Primarily used for testing messaging.","shower":"0","kitchenuse":"0","lawnspace":"0","sag":"0","bed":"0","laundry":"0","food":"0","howdidyouhear":"","lastcorrespondence":null,"languagesspoken":"","URL":"","isstale":"0","isstale_date":"0","isstale_reason":null,"isunreachable":"0","unreachable_date":"0","unreachable_reason":null,"becomeavailable":"1596348000","set_unavailable_timestamp":"1438580975","set_available_timestamp":"0","last_unavailability_pester":"1438580975","hide_donation_status":"0","email_opt_out":"0","oid":"98313","type":"","street":"","additional":"","city":"Boise","province":"id","postal_code":"","country":"us","latitude":"43.618710","longitude":"-116.214607","source":"5"}}}


namespace WSApp.DataModel
{
    [DataContract]
    public class Profile
    {
        [DataMember]
        public User user_Result;

        // Constructor
        public Profile()
        {
        }

        public Profile(Profile newProfile)
        {
            if (null != newProfile)
            {
                user_Result = newProfile.user_Result;
            }
            else
            {
                user_Result = null;
            }
        }





    }

    [DataContract]
    public class Users
    {

        public Users()
        {
        }

        [DataMember]
        public Users_result users_result;

        public class Users_result
        {
            //        [DataMember]
            //        public QueryData query_data { get; set; }

            [DataMember]
            public Status status { get; set; }

            [DataMember]
        //    public string accounts { get; set; }
            public Dictionary<int, object> accounts { get; set; }
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


    }

    /// <summary>
    /// Complete user dictionary as returned by POST /services/rest/hosts/by_keyword and GET /services/rest/user/<uid>
    /// </summary>
    public class User
    {

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public int uid { get; set; }

        [DataMember]
        public string fullname { get; set; }

        [DataMember]
        public long login { get; set; }

        [DataMember]
        public long access { get; set; }

        [DataMember]
        public long created { get; set; }

        [DataMember]
        public string street { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string province { get; set; }

        [DataMember]
        public string postal_code { get; set; }

        [DataMember]
        public string country { get; set; }

        [DataMember]
        public string mobilephone { get; set; }

        [DataMember]
        public string comments { get; set; }

        [DataMember]
        public string maxcyclists { get; set; }

        [DataMember]
        public string notcurrentlyavailable { get; set; }

        [DataMember]
        public string bed { get; set; }

        [DataMember]
        public string bikeshop { get; set; }

        [DataMember]
        public string campground { get; set; }

        [DataMember]
        public string food { get; set; }

        [DataMember]
        public string kitchenuse { get; set; }

        [DataMember]
        public string laundry { get; set; }

        [DataMember]
        public string lawnspace { get; set; }

        [DataMember]
        public string motel { get; set; }

        [DataMember]
        public string sag { get; set; }

        [DataMember]
        public string shower { get; set; }

        [DataMember]
        public string storage { get; set; }

        [DataMember]
        public double latitude { get; set; }

        [DataMember]
        public double longitude { get; set; }

        [DataMember]
        public string languagesspoken { get; set; }

        [DataMember]
        public string URL { get; set; }

        [DataMember]
        public string preferred_notice { get; set; }

        [DataMember]
        public string additional { get; set; }

        [DataMember]
        public string homephone { get; set; }

        [DataMember]
        public string workphone { get; set; }

		// Removed in D7
        [DataMember]
        public string picture { get; set; }

        // Added in D7 for WP
        [DataMember]
        public string profile_image_mobile_photo_456 { get; set; }
    }



}

