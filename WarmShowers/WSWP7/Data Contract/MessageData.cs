using System.Collections.Generic;

// old
//{"thread_id":"34632","participants":{"user_14861":{"uid":"14861","name":"klzig"},"user_41682":{"uid":"41682","name":"klzig_test"}},"read_all":false,"to":"4","message_count":"4","from":1,"start":0,"messages":[
// {"files":[],"mid":"34632","author":{"uid":"41682","name":"klzig_test"},"subject":"test from klzig_test","body":"This message was sent from the klzig_test account.","timestamp":"1357758921","format":"1","is_new":"0","thread_id":"34632"},
// {"files":[],"mid":"35461","author":{"uid":"14861","name":"klzig"},"subject":"test from klzig_test","body":"This is a test reply to kurt Ziegler Test.","timestamp":"1357966416","format":"1","is_new":"1","thread_id":"34632"},
// {"files":[],"mid":"35462","author":{"uid":"14861","name":"klzig"},"subject":"test from klzig_test","body":"Another reply.","timestamp":"1357966464","format":"1","is_new":"1","thread_id":"34632"},
// {"files":[],"mid":"35490","author":{"uid":"14861","name":"klzig"},"subject":"test from klzig_test","body":"Yet another reply.","timestamp":"1357982020","format":"1","is_new":"1","thread_id":"34632"}],
// "user":{"uid":"41682","name":"klzig_test"},"subject":"test from klzig_test"}

// new (D7)
//{"pmtid":1373463,"subject":"Hosting request for the Jaialdi 2015 (weekend).","participants":[{"uid":"14861","name":"klzig","fullname":"Kurt Ziegler"},
// {"uid":"78696","name":"Xabi","fullname":"Xabier Azkorra"}],"messages":[
// {"mid":"1373463","author":"78696","timestamp":"1438096171","body":"Hello Kurt,\r\n\r\nI started touring the 29th of June from San Francisco with main destination Boise, ID, and the Jaialdi 2015 (then I want to go back to the West coast through Oregon and return to SF by the coast on highway 1/101). \r\n\r\nI am Basque and the reason of the tour is to track Basque commumities around the West states wih main destination: Jaialdi 2015 this week. All the info is on my blog and the link is on my profile.\r\n\r\nI'm staying in Kuna for some days with a family involved in the Basque and cyclist communities, anf will move to Boise on Tuesday to stay with a WS family on Wednesday and Thursday. \r\nI would appreciate it so much if I could stay with you during the weekend.\r\n\r\nThanks a lot for your answer!\r\nXabi.","is_new":"0"},{"mid":"1377089","author":"14861","timestamp":"1438177535","body":"Hi Xabier,\r\n\r\nWe have a guest bedroom available and would love to host you this weekend.  Which nights do you plan to stay?\r\n\r\nLooking forward to meeting you!\r\n\r\nkurt","is_new":"0"},{"mid":"1385513","author":"14861","timestamp":"1438358617","body":"Hi Xabier,\r\n\r\nAre you still planning to stay with us this weekend?  Which night(s)?\r\n\r\nThanks,\r\nkurt","is_new":"0"},
// {"mid":"1387534","author":"78696","timestamp":"1438416182","body":"I'm very sorry. I thought I had emailed back every contacted WS but I didn't contact you... I can finally stay for the weekend with the same fanily so I don't need to move anything. Thank you so much for all your support, \r\nXabi.","is_new":"0"},
// {"mid":"1387578","author":"14861","timestamp":"1438417241","body":"No worries, glad it worked out!\n________________________________\nFrom: Warmshowers Messages<mailto:wsl@warmshowers.org>\nSent: \u200e8/\u200e1/\u200e2015 2:03 AM\nTo: klzig@live.com<mailto:klzig@live.com>\nSubject: Hosting request for the Jaialdi 2015 (weekend). (from Xabier Azkorra at Warmshowers.org) [mid.1387534.14861.E3ZuVNMqyTF00GC3El6+eM49AkM=]","is_new":"0"},
// {"mid":"1388560","author":"78696","timestamp":"1438442926","body":"Thanks Kurt!","is_new":"0"}]}

namespace WSApp.DataModel
{
    public class MessageThread
    {
        public Message_result message_result;

        // Constructor
        public MessageThread()
        {
        }

        public class Author
        {
            public string uid { get; set; }
            public string name { get; set; }
        }

        public class Message
        {
 //           public List<object> files { get; set; }
            public int mid { get; set; }
            public string author { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
            public long timestamp { get; set; }
//            public int format { get; set; }
            public int is_new { get; set; }
//            public int thread_id { get; set; }
        }

        public class User
        {
            public int uid { get; set; }
            public string name { get; set; }
            public string fullname { get; set; }
        }

        public class Message_result
        {
 //           public int thread_id { get; set; }
            public int pmtid { get; set; }
            public object participants { get; set; }
//            public bool read_all { get; set; }
//            public int to { get; set; }
//            public int message_count { get; set; }
//            public int from { get; set; }
//            public int start { get; set; }
            public List<Message> messages { get; set; }
//            public User user { get; set; }
            public string subject { get; set; }
        }
    }
}