using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace Message
{
    public enum MessageTypeEnum { signup, text, picture, notification, warn };
    [Serializable]
    public class Message
    {
        public MessageTypeEnum messageType { get; set; }
        public string userName { get; set; }
        public string time { get; set; }

        public string text { get; set; }
        public byte[] bitmap { get; set; }

        public string room { set; get; }


        public Message(MessageTypeEnum _messageType, string _userName, string _time, string _text, byte[] _bitmap, string _room)
        {
            messageType = _messageType;
            userName = _userName;
            time = _time;
            text = _text;
            bitmap = _bitmap;
            room = _room;
        }
        public Message()
        {

        }




    }
    static public class MessageHandle
    {
        public static byte[] serialize(Message msg)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            formatter.Serialize(stream, msg);
            byte[] buffer = ((MemoryStream)stream).ToArray();
            return buffer;


        }

        public static Message deserialize(byte[] receivedBytes)
        {
            IFormatter bf = new BinaryFormatter();
            Stream stream = new MemoryStream(receivedBytes);
            Message message = (Message)bf.Deserialize(stream);
            return message;
        }

        public static Message genRoomNotifi(string toNoti, MessageTypeEnum type)
        {
            Message msg = new Message();
            msg.messageType = type;
            msg.text = toNoti;
            msg.time = System.DateTime.Now.ToString();
            return msg;

        }




    }

}
