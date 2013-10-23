using System.Collections.Generic;
using System.Runtime.Serialization;

//{"users":[{"user":{"name":"rfay","uid":"1","fullname":"Randy Fay","login":"1355641145","access":"1356048541","created":"1131376220","street":"848 Montclair Dr","city":"Palisade","province":"co",
//"postal_code":"81526","country":"us","mobilephone":"+1 303 781 7370","comments":
//"This is my admin account - please go to our <a href=\"\/users\/randyfay\">real hosting account (randyfay)<\/a>.","maxcyclists":"4","notcurrentlyavailable":"1","bed":"0","bikeshop":"","campground":"","food":"0","kitchenuse":"0","laundry":"0",
//"lawnspace":"0","motel":"","sag":"0","shower":"0","storage":"0","latitude":"39.087436","longitude":"-108.447418"}}]}
namespace WSApp.DataModel
{
    [DataContract]
    public class Profile
    {
        [DataMember]
        public Users_result users_Result;

        // Constructor
        public Profile()
        {
        }

        public Profile(Profile newProfile)
        {
            if (null != newProfile)
            {
                users_Result = new Users_result(newProfile.users_Result);
            }
            else
            {
                users_Result = null;
            }
        }

        public class User2
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
            public string picture { get; set; }
 
        }

        public class User
        {
            public User2 user { get; set; }
        }

        [DataContract]
        public class Users_result
        {
            [DataMember]
            public List<User> users { get; set; }

            public Users_result(Users_result newUsers)
            {
                if (null != newUsers)
                {
                    users = newUsers.users;
                }
                else
                {
                    users = null;
                }
            }
        }
    }
}

