using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    public class ClientRoom
    {
        private Hashtable _userList = new Hashtable();
        private string _roomName;

        public string addUser(string userName, Socket userSocket)
        {
            try
            {
                _userList.Add(userName, userSocket);

                broadcast(Message.MessageHandle.genRoomNotifi(userName + "加入房间" + _roomName, Message.MessageTypeEnum.notification));



                return "Success";
            }
            catch (Exception e)
            {
                LogService.LogError(e.ToString());
                return e.ToString();

            }
        }

        public string removeUser(string userName)
        {
            try
            {

                _userList.Remove(userName);
                UserHandle.removeUser(userName);

                broadcast(Message.MessageHandle.genRoomNotifi(userName + "离开房间" + _roomName, Message.MessageTypeEnum.notification));



                return "Success";
            }
            catch (Exception e)
            {
                LogService.LogError(e.ToString());
                return e.ToString();

            }
        }

        public string getRoomName()
        {
            return _roomName;
        }

        public Boolean isThisRoom(string userName)
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

        public string broadcast(Message.Message msg)
        {
            LogService.Log(msg);
            try
            {
                foreach (DictionaryEntry user in _userList)
                {
                    Socket brdcastSocket = (Socket)user.Value;
                    try
                    {
                        brdcastSocket.Send(Message.MessageHandle.serialize(msg));
                    }
                    catch (Exception e)
                    {
                        //记录下没有发送到哪个客户端
                        //log
                        LogService.LogError(e.ToString());
                        Debug.WriteLine(e.ToString());
                    }
                }
                return "Success";
            }
            catch (Exception e)
            {
                LogService.LogError(e.ToString());
                Debug.WriteLine(e.ToString());
                return e.ToString();

            }
        }

        public ClientRoom(string roomName)
        {
            _roomName = roomName;
        }
        public ClientRoom()
        {

        }
    }

    public class RoomHandle
    {

        private Hashtable _roomList = new Hashtable();

        //添加一个房间
        public ClientRoom addRoom(string roomName)
        {
            if (_roomList.ContainsKey(roomName))
            {
                return (ClientRoom)_roomList[roomName];
            }
            else
            {
                ClientRoom clientRoom = new ClientRoom(roomName);
                _roomList.Add(roomName, clientRoom);
                return clientRoom;
            }
        }
        //判断用户在哪个房间
        public ClientRoom whichRoom(string userName)
        {
            ClientRoom clientRoom = new ClientRoom();
            foreach (DictionaryEntry room in _roomList)
            {
                clientRoom = (ClientRoom)room.Key;
                if (clientRoom.isThisRoom(userName))
                {
                    return clientRoom;
                }

            }
            Debug.WriteLine("出错,没有找到用户在的组");
            return clientRoom;
        }
        //判断room是否存在
        public Boolean isRoomExist(string roomName)
        {
            if (_roomList.ContainsKey(roomName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //得到一个存在的room
        public ClientRoom getRoom(string roomName)
        {
            return (ClientRoom)_roomList[roomName];
        }

    }
}
