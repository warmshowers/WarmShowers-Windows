using System.Collections.Generic;
using System.Runtime.Serialization;

//  [{"thread_id":"34632","subject":"test from klzig_test","last_updated":"1357758921","is_new":"1","participants":[{"uid":"41682","name":"klzig_test", "fullname":"Kurt Ziegler"}]},
//   {"thread_id":"34558","subject":"subject sent from phone","last_updated":"1357746028","is_new":"0","participants":[]},
//   {"thread_id":"34557","subject":"subject sent from phone","last_updated":"1357746006","is_new":"1","participants":[]},
//   {"thread_id":"34335","subject":"subject sent from phone","last_updated":"1357682696","is_new":"1","participants":[]},
//   {"thread_id":"34309","subject":"test from iPod","last_updated":"1357680625","is_new":"0","participants":[]},
//   {"thread_id":"27526","subject":"another test","last_updated":"1355766609","is_new":"1","participants":[]},
//   {"thread_id":"27387","subject":"test from Kurt","last_updated":"1355728319","is_new":"0","participants":[]},
//   {"thread_id":"25367","subject":"test from was iPhone","last_updated":"1355157972","is_new":"0","participants":[]}]

namespace WSApp.DataModel
{
    [DataContract]
    public class Messages
    {
        [DataMember]
        public Messages_result messages_result;

        // Constructor
        public Messages()
        {
        }

        public Messages(Messages newMessages)
        {
            if (null != newMessages)
            {
//                messages_result = new Messages_result(newMessages.messages_result);
                messages_result = newMessages.messages_result;
            }
            else
            {
                messages_result = null;
            }
        }

        [DataContract]
        public class Participant
        {
            [DataMember]
            public int uid { get; set; }

            [DataMember]
            public string name { get; set; }

            [DataMember]
            public string fullname { get; set; }
        }

        [DataContract]
        public class Message
        {
            [DataMember]
            public int thread_id { get; set; }

            [DataMember]
            public string subject { get; set; }

            [DataMember]
            public long last_updated { get; set; }

            [DataMember]
            public long thread_started { get; set; }

            [DataMember]
            public int is_new { get; set; }

            [DataMember]
            public int count { get; set; }

            [DataMember]
            public List<Participant> participants { get; set; }
        }

        [DataContract]
        public class Messages_result
        {
            [DataMember]
            public List<Message> messages { get; set; }

            // Constructor
            public Messages_result()
            {
            }

            public Messages_result(Messages_result newResult)
            {
                if (null != newResult)
                {
                    messages = newResult.messages;
                }
                else
                {
                    messages = null;
                }
            }
        }
    }
}

