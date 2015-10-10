using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public class ChatService
    {
        private string _userName;
        private string _userRoom;
        private Socket _clientSocket = null;
        private Boolean isListen = true;

       
        public ChatService(string userName, string userRoom)
        {
            _userName = userName;
            _userRoom = userRoom;
        }

        public Boolean connectServer()
        {
            Message.Message msg = genMessage(Message.MessageTypeEnum.signup);
            if (_clientSocket == null || !_clientSocket.Connected)
            {
                try
                {
                    _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _clientSocket.BeginConnect(IPAddress.Loopback, 8080, (args) =>
                    {
                        if (args.IsCompleted)
                        {

                            _clientSocket.Send(Message.MessageHandle.serialize(msg));
                            Thread th = new Thread(dataFromServer);
                            th.IsBackground = true;
                            th.Start();


                        }
                    }, null);

                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    return false;
                }
            }
            else
            {
                Debug.WriteLine("已经连接到Server");
                return true;
            }

        }

        private Message.Message genMessage(Message.MessageTypeEnum msgType)
        {
            Message.Message msg = new Message.Message();
            msg.userName = _userName;
            msg.room = _userRoom;
            msg.time = System.DateTime.Now.ToString();
            msg.messageType = msgType;
            return msg;
        }

        public Boolean sendText(string text)
        {
            try
            {
                Message.Message msg = genMessage(Message.MessageTypeEnum.text);
                msg.text = text;

                _clientSocket.Send(Message.MessageHandle.serialize(msg));
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }

        }

        private void dataFromServer()
        {
            isListen = true;
            try
            {
                while (isListen)
                {
                    Byte[] bytesFromServer = new Byte[2097152];

                    int len = _clientSocket.Receive(bytesFromServer);
                    byte[] receivedBytes = new byte[len];
                    receivedBytes = bytesFromServer;
                    Message.Message msg = (Message.Message)Message.MessageHandle.deserialize(receivedBytes);

                    //此时消息已经被反序列化为Message类,这里添加接受到消息后做的处理



                    if (msg.messageType == Message.MessageTypeEnum.text)
                    {
                        if (msg.userName == _userName)
                        {
                            //do something
                        }
                        else
                        {
                            //do something
                        }

                    }
                    //else if (msg.messageType == Message.MessageTypeEnum.picture)
                    //{
                    //    if (msg.userName == _userName)
                    //    {
                    //        ShowPicture(msg.userName, LoadImage(msg.bitmap), "mine");
                    //    }
                    //    else
                    //    {
                    //        ShowPicture(msg.userName, LoadImage(msg.bitmap), "other");
                    //    }

                    //}
                    //else if (msg.messageType == Message.MessageTypeEnum.notification)
                    //{
                    //    showMassage(msg.userName, msg.text, "noti");
                    //}
                    //else if (msg.messageType == Message.MessageTypeEnum.warn)
                    //{
                    //    showWarn(msg.text);

                    //}


                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }   

    }
}
