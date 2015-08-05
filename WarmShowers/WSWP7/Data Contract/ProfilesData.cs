using System.Collections.Generic;
using System.Runtime.Serialization;

// This format is deprecated, the result of a call to https://warmshowers.org/user/<uid>/json

//{"users":[{"user":{"name":"rfay","uid":"1","fullname":"Randy Fay","login":"1355641145","access":"1356048541","created":"1131376220","street":"848 Montclair Dr","city":"Palisade","province":"co",
//"postal_code":"81526","country":"us","mobilephone":"+1 303 781 7370","comments":
//"This is my admin account - please go to our <a href=\"\/users\/randyfay\">real hosting account (randyfay)<\/a>.","maxcyclists":"4","notcurrentlyavailable":"1","bed":"0","bikeshop":"","campground":"","food":"0","kitchenuse":"0","laundry":"0",
//"lawnspace":"0","motel":"","sag":"0","shower":"0","storage":"0","latitude":"39.087436","longitude":"-108.447418"}}]}

//{"uid":"14861","name":"klzig","mode":"0","sort":"0","threshold":"0","theme":"","created":"1258696215","access":"1438489339","status":"1","timezone":"-21600","language":"en-working","picture":"files/pictures/picture-14861.jpg","login":"1438489167","timezone_name":"America/Denver","signature_format":"0","fullname":"Kurt Ziegler","comments":"I'm a year-round commuter and summer touring cyclist. I enjoy self-organized tours as well as the occasional professional tour.  My daughter and I recently completed an epic cycling adventure.  Would enjoy hosting up to three people and hearing about their adventures.  My wife, daughter and I live in North Boise where the house is ruled by a surly but mostly harmless cat and a dog or two.\r\n\r\nPlease email before calling.","notcurrentlyavailable":"1","homephone":"","mobilephone":"208.859.6295","workphone":"","fax_number":"","preferred_notice":"Please email before calling","maxcyclists":"3","motel":"6 blocks","campground":"5 miles","bikeshop":"13 blocks","languagesspoken":"English","URL":"https://trike2012.wordpress.com/","bed":"1","food":"1","laundry":"1","lawnspace":"1","sag":"1","shower":"1","storage":"0","kitchenuse":"1","picture_delete":0,"picture_upload":"","form_build_id":"form-W5yVFHwUaVvlFoAwyftE6VwLqDZ-vr9E8iwERJRiNwI","set_unavailable_timestamp":"1438267366","set_available_timestamp":"0","last_unavailability_pester":"1438267366","becomeavailable":"1439359200","email_opt_out":"0","notcurrentlyavailable_reason":null,"cost":"","lastcorrespondence":null,"isstale":"0","isstale_date":"0","isstale_reason":null,"isunreachable":"0","unreachable_date":"0","unreachable_reason":null,"hide_donation_status":null,"oid":"14861","type":"","street":"1405 N 22nd St","additional":"","city":"Boise","province":"id","postal_code":"83702","country":"us","latitude":"43.633213","longitude":"-116.215910","source":"1","node_notify_mailalert":"1","comment_notify_mailalert":"1","profile_image_mobile_profile_photo_std":"https://www.warmshowers.org/files/imagecache/mobile_profile_photo_std/pictures/picture-14861.jpg","profile_image_profile_picture":"https://www.warmshowers.org/files/imagecache/profile_picture/pictures/picture-14861.jpg"}


//namespace WSApp.DataModel
//{
//    [DataContract]
//    public class Profile
//    {
//        [DataMember]
//        public Users_result users_Result;

//        // Constructor
//        public Profile()
//        {
//        }

//        public Profile(Profile newProfile)
//        {
//            if (null != newProfile)
//            {
//                users_Result = new Users_result(newProfile.users_Result);
//            }
//            else
//            {
//                users_Result = null;
//            }
//        }

//        public class User2
//        {
//            [DataMember]
//            public string name { get; set; }

//            [DataMember]
//            public int uid { get; set; }

//            [DataMember]
//            public string fullname { get; set; }

//            [DataMember]
//            public long login { get; set; }

//            [DataMember]
//            public long access { get; set; }

//            [DataMember]
//            public long created { get; set; }

//            [DataMember]
//            public string street { get; set; }

//            [DataMember]
//            public string city { get; set; }

//            [DataMember]
//            public string province { get; set; }

//            [DataMember]
//            public string postal_code { get; set; }

//            [DataMember]
//            public string country { get; set; }

//            [DataMember]
//            public string mobilephone { get; set; }

//            [DataMember]
//            public string comments { get; set; }

//            [DataMember]
//            public string maxcyclists { get; set; }

//            [DataMember]
//            public string notcurrentlyavailable { get; set; }

//            [DataMember]
//            public string bed { get; set; }

//            [DataMember]
//            public string bikeshop { get; set; }

//            [DataMember]
//            public string campground { get; set; }

//            [DataMember]
//            public string food { get; set; }

//            [DataMember]
//            public string kitchenuse { get; set; }

//            [DataMember]
//            public string laundry { get; set; }

//            [DataMember]
//            public string lawnspace { get; set; }

//            [DataMember]
//            public string motel { get; set; }

//            [DataMember]
//            public string sag { get; set; }

//            [DataMember]
//            public string shower { get; set; }

//            [DataMember]
//            public string storage { get; set; }

//            [DataMember]
//            public double latitude { get; set; }

//            [DataMember]
//            public double longitude { get; set; }

//            [DataMember]
//            public string picture { get; set; }

//            [DataMember]
//            public string languagesspoken { get; set; }

//            [DataMember]
//            public string URL { get; set; }

//            [DataMember]
//            public string preferred_notice { get; set; }
 
//        }

//        public class User
//        {
//            public User2 user { get; set; }
//        }

//        [DataContract]
//        public class Users_result
//        {
//            [DataMember]
//            public List<User> users { get; set; }

//            public Users_result(Users_result newUsers)
//            {
//                if (null != newUsers)
//                {
//                    users = newUsers.users;
//                }
//                else
//                {
//                    users = null;
//                }
//            }
//        }
//    }
//}

