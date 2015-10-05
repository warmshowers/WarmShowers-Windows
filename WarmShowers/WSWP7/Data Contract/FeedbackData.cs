using System.Collections.Generic;
using System.Runtime.Serialization;

// old
//{"users":[{"user":{"name":"rfay","uid":"1","fullname":"Randy Fay","login":"1355641145","access":"1356048541","created":"1131376220","street":"848 Montclair Dr","city":"Palisade","province":"co",
//"postal_code":"81526","country":"us","mobilephone":"+1 303 781 7370","comments":
//"This is my admin account - please go to our <a href=\"\/users\/randyfay\">real hosting account (randyfay)<\/a>.","maxcyclists":"4","notcurrentlyavailable":"1","bed":"0","bikeshop":"","campground":"","food":"0","kitchenuse":"0","laundry":"0",
//"lawnspace":"0","motel":"","sag":"0","shower":"0","storage":"0","latitude":"39.087436","longitude":"-108.447418"}}]}

// new (D7)
//{"recommendations":[{"recommendation":{"nid":"74988","fullname":"Josh Muench","name":"vanagonjosh","uid":"67611",
// "body":"Ross was kind enough to offer us a place to stay even with a busy evening. His cycle touring experience made me feel at home chatting about routs taken and options for the rest of our trip.  He shared some cold beer and conversation on our short stay.  Highly recommend staying with Ross if you are in Hood River!","field_hosting_date":"2014-10-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}},{"recommendation":{"nid":"73369","fullname":"Cristine Duda","name":"BPRevolution","uid":"31504","body":"Ross is a very kind and generous man! Not only did he host me without being home but also took the time to provide me with valuable advice on routes in the gorge area while he was on vacation! Ross (and his great friends Jack and Sheila!) really went out of their way to make me feel comfortable and well taken care of and I appreciate that so much! Awesome! Thank you!","field_hosting_date":"2014-09-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}},{"recommendation":{"nid":"66301","fullname":"Daisy and Jason Philtron","name":"DaisyLahaina","uid":"16067","body":"Ross is a great, welcoming host who opened up his home to us even as he was running out the door.  We spent two nights at his house as we reorganized and added a new member to our cycling party.  Even though he was also hosting some cycling friends of his own, he gave us good advice and a beautiful yard to camp in.  Thanks for being our welcome to Oregon!  Also - the climb to his house is really not that bad, and there are some delicious blackberries en route...",
// "field_hosting_date":"2014-07-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}},{"recommendation":{"nid":"35679","fullname":"Stefan Wirth","name":"Stefan","uid":"47737","body":"Ross was an awesome host and accommodated our large group of five on pretty short notice. We went out for pizza, beer, and ice cream in downtown Hood River and he even transported my bike back up to the house, which was great after a 120 mile day heading into to wind in the gorge! His blueberry pancakes the next morning were some of the best I've ever had and it was a pleasure hanging out and talking about cycling, his career, and living and enjoying the outdoors in the Hood River area. Ross, would love to host you and show you around DC if you're in the area - stay in touch.","field_hosting_date":"2013-07-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}},{"recommendation":{"nid":"35410","fullname":"Natalie Locke","name":"natalie2locke","uid":"47821","body":"Ross' accommodations were well worth the climb to his house. I arrived in a group of 5 and he happily offered us all showers before driving us back downtown to eat pizza at Double Mountain Brew Pub and then go out for ice cream. We camped in his beautiful backyard and woke up to a blueberry pancake breakfast, which carried us through the headwinds in the Gorge the next day. Thanks, Ross!","field_hosting_date":"2013-07-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}},{"recommendation":{"nid":"32182","fullname":"Sarah Burch","name":"sarburch","uid":"26920","body":"I stayed at Ross's home on the first day of my cross-country tour. Ross was a great host! He shared a tasty dinner, wonderful conversation, and the comfort of his beautiful home. I highly recommend staying with Ross. :)","field_hosting_date":"2013-05-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}},{"recommendation":{"nid":"29917","fullname":"Shawn Granton","name":"urbanadventureleaguepdx","uid":"17584","body":"Ross' house was a welcome sight after a long and wet 65 mile day on the road. He was a most gracious host and it energized me for the days of riding that still lay ahead of me.","field_hosting_date":"2013-05-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}},{"recommendation":{"nid":"17593","fullname":"Andre Jon St. Laurent","name":"Andrejon","uid":"32922","body":"Ross was flying out of town when I was riding through Hood River, but he was kind enough to offer that I camp or stay at his place.  It was a wonderful location with nearby views of Mt. Hood and cherry trees on the property.  Ross even went out of his way to leave a six-pack of beer in the fridge- amazing!  Thanks again Ross!\r\n\r\nAndre St. Laurent","field_hosting_date":"2012-07-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}},{"recommendation":{"nid":"15372","fullname":"Patrick Fackrell","name":"patrickfackrell","uid":"33398","body":"I stayed with Ross at his gorgeous home in Hood River and had a great experience. Ross was totally welcoming, and he provided me with a delicious dinner, laundry, a bed to sleep in, great conversation, and a fantastic breakfast. I highly recommend staying with Ross.","field_hosting_date":"2012-06-00T00:00:00","field_guest_or_host":"Host","name_1":"RossHoag","uid_1":"34121","field_rating":"Positive"}}]}

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
//                recommendations_Result = new Recommendations_result(newFeedback.recommendations_Result);
                recommendations_Result = newFeedback.recommendations_Result;
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

