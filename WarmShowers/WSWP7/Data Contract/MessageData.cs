using System.Collections.Generic;

//{"thread_id":"34632","participants":{"user_14861":{"uid":"14861","name":"klzig"},"user_41682":{"uid":"41682","name":"klzig_test"}},"read_all":false,"to":"4","message_count":"4","from":1,"start":0,"messages":[
// {"files":[],"mid":"34632","author":{"uid":"41682","name":"klzig_test"},"subject":"test from klzig_test","body":"This message was sent from the klzig_test account.","timestamp":"1357758921","format":"1","is_new":"0","thread_id":"34632"},
// {"files":[],"mid":"35461","author":{"uid":"14861","name":"klzig"},"subject":"test from klzig_test","body":"This is a test reply to kurt Ziegler Test.","timestamp":"1357966416","format":"1","is_new":"1","thread_id":"34632"},
// {"files":[],"mid":"35462","author":{"uid":"14861","name":"klzig"},"subject":"test from klzig_test","body":"Another reply.","timestamp":"1357966464","format":"1","is_new":"1","thread_id":"34632"},
// {"files":[],"mid":"35490","author":{"uid":"14861","name":"klzig"},"subject":"test from klzig_test","body":"Yet another reply.","timestamp":"1357982020","format":"1","is_new":"1","thread_id":"34632"}],
// "user":{"uid":"41682","name":"klzig_test"},"subject":"test from klzig_test"}

namespace WSApp.DataModel
{
    public class MessageThread
    {
        public Message_result message_result;

        // Constructor
        public MessageThread()
        {
        }

        /*
        public class User14861
        {
            public string uid { get; set; }
            public string name { get; set; }
        }

        public class User41682
        {
            public string uid { get; set; }
            public string name { get; set; }
        }

        public class Participants
        {
            public User14861 user_14861 { get; set; }
            public User41682 user_41682 { get; set; }
        }
         */

        public class Author
        {
            public string uid { get; set; }
            public string name { get; set; }
        }

        public class Message
        {
            public List<object> files { get; set; }
            public int mid { get; set; }
            public Author author { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
            public long timestamp { get; set; }
            public int format { get; set; }
            public int is_new { get; set; }
            public int thread_id { get; set; }
        }

        public class User
        {
            public int uid { get; set; }
            public string name { get; set; }
        }

        public class Message_result
        {
            public int thread_id { get; set; }
            public object participants { get; set; }
            public bool read_all { get; set; }
            public int to { get; set; }
            public int message_count { get; set; }
            public int from { get; set; }
            public int start { get; set; }
            public List<Message> messages { get; set; }
            public User user { get; set; }
            public string subject { get; set; }
        }
    }
}