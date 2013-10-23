using System.Collections.Generic;
using System.Runtime.Serialization;

//{"users":[{"user":{"name":"rfay","uid":"1","fullname":"Randy Fay","login":"1355641145","access":"1356048541","created":"1131376220","street":"848 Montclair Dr","city":"Palisade","province":"co",
//"postal_code":"81526","country":"us","mobilephone":"+1 303 781 7370","comments":
//"This is my admin account - please go to our <a href=\"\/users\/randyfay\">real hosting account (randyfay)<\/a>.","maxcyclists":"4","notcurrentlyavailable":"1","bed":"0","bikeshop":"","campground":"","food":"0","kitchenuse":"0","laundry":"0",
//"lawnspace":"0","motel":"","sag":"0","shower":"0","storage":"0","latitude":"39.087436","longitude":"-108.447418"}}]}

//{"recommendations":[{"recommendation":{"nid":"9392","fullname":"Kyle Hardie","name":"KyleHardie","uid":"13672",
//"body":"Everything I could ask for in a Warmshowers experience! A warm and friendly family opening up their beautiful home to a haggard and stinky cyclist. 
//Kurt, Lisa and Zoe were amazing hosts, their home was strategically located in a beautifully quiet neighborhood close to the downtown ammenities but far enough removed from the hustle and noise. 
//They offered engaging conversations, warm smiles, relaxed atmosphere...an overall great experience. Thanks again Zieglers!",
//"field_hosting_date_value":1309413600,"field_guest_or_host_value":"Host","name_1":"klzig","uid_1":"14861"}}]}
namespace WSApp.DataModel
{
    [DataContract]
    public class Feedback
    {
        [DataMember]
        public Recommendations_result recommendations_Result;

        // Constructor
        public Feedback()
        {
        }

        public Feedback(Feedback newFeedback)
        {
            if (null != newFeedback)
            {
                recommendations_Result = new Recommendations_result(newFeedback.recommendations_Result);
            }
            else
            {
                recommendations_Result = null;
            }
        }

        public class Recommendation2
        {
            [DataMember]
            public int nid { get; set; }

            [DataMember]
            public string fullname { get; set; }

            [DataMember]
            public string name { get; set; }

            [DataMember]
            public int uid { get; set; }

            [DataMember]
            public string body { get; set; }

            [DataMember]
            public long field_hosting_date_value { get; set; }

            [DataMember]
            public string field_guest_or_host_value { get; set; }

            [DataMember]
            public string field_rating_value { get; set; }

            [DataMember]
            public string name_1 { get; set; }

            [DataMember]
            public int uid_1 { get; set; }
        }

        public class Recommendation
        {
            public Recommendation2 recommendation { get; set; }
        }

        [DataContract]
        public class Recommendations_result
        {
            [DataMember]
            public List<Recommendation> recommendations { get; set; }

            // Constructor
            public Recommendations_result(Recommendations_result newResult)
            {
                if (null != newResult)
                {
                    recommendations = newResult.recommendations;
                }
                else
                {
                    recommendations = null;
                }
            }
        }      
    }
}

