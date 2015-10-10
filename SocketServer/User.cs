using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    public class User
    {
        private string _userName { set; get; }
        private Socket _userSocket { set; get; }

        private ClientRoom _clientRoom { set; get; }
        public User(string userName, Socket userSocket, ClientRoom clientRoom)
        {
            _userName = userName;
            _userSocket = userSocket;
            _clientRoom = clientRoom;


        }
        public void startListen()
        {
            Thread th = new Thread(chatServer);
            th.IsBackground = true;
            th.Start();
        }

        private void chatServer()
        {
            Boolean isListen = true;

            while (isListen)
            {
                Byte[] bytesFromClient = new Byte[2097152];
                int len = 0;
                try
                {
                    len = _userSocket.Receive(bytesFromClient);
                    byte[] receiveByte = new byte[len];
                    receiveByte = bytesFromClient;
                    Message.Message msg = Message.MessageHandle.deserialize(receiveByte);
                    _clientRoom.broadcast(msg);


                }
                catch (Exception e)
                {


                    isListen = false;
                    _clientRoom.removeUser(_userName);


                    _userSocket.Close();
                    _userSocket = null;
                    Debug.WriteLine(e);
                    Debug.WriteLine(_userName + "退出聊天");
                    LogService.LogError(e.ToString());

                }
            }
        }
        public string getUserName()
        {
            return _userName;
        }
        public Socket getSocket()
        {
            return _userSocket;
        }


    }
    public static class UserHandle
    {
        private static Hashtable _userList = new Hashtable();

        public static void addUser(string userName, User user)
        {
            _userList.Add(userName, user);
        }
        public static void removeUser(string userName)
        {
            User tempUser = (User)_userList[userName];

            _userList.Remove(userName);

        }
        public static Boolean isUserExist(string userName)
        {
            if (_userList.ContainsKey(userName))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

    }
}
