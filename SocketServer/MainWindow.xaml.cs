using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SocketServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            serverStart();
        }
        public static RoomHandle roomHandle = new RoomHandle();
        Socket serverSocket = null;
        Boolean isListen = true;
        public void serverStart()
        {
            if (serverSocket == null)
            {
                try
                {
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //监听套接字
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 8080);//端口
                    serverSocket.Bind(endPoint);//绑定
                    serverSocket.Listen(100);//最大连接数
                    //开始监听
                    Thread th = new Thread(startListen);
                    th.IsBackground = true;
                    th.Start();

                }
                catch (Exception e)
                {
                    serverSocket.Close();
                    Debug.WriteLine(e.ToString());
                    LogService.LogError(e.ToString());
                }
            }
        }

        private void startListen()
        {
            isListen = true;
            Socket clientSocket = default(Socket);
            while (isListen)
            {
                try
                {
                    clientSocket = serverSocket.Accept();
                }
                catch (SocketException e)
                {
                    Debug.WriteLine(e.ToString());
                    LogService.LogError(e.ToString());
                }
                Byte[] bytesFromClient = new Byte[4096];
                if (clientSocket != null && clientSocket.Connected)
                {
                    try
                    {
                        int len = clientSocket.Receive(bytesFromClient);
                        byte[] receivedBytes = new byte[len];
                        Array.Copy(bytesFromClient, receivedBytes, len);
                        if (len > -1)
                        {
                            Message.Message msg = Message.MessageHandle.deserialize(receivedBytes);

                            //如果是新用户
                            if (msg.messageType == Message.MessageTypeEnum.signup)
                            {
                                if (!UserHandle.isUserExist(msg.userName))
                                {
                                    //注册一个房间
                                    ClientRoom clientRoom = null;
                                    if (roomHandle.isRoomExist(msg.room))
                                    {
                                        clientRoom = roomHandle.getRoom(msg.room);
                                    }
                                    else
                                    {
                                        clientRoom = roomHandle.addRoom(msg.room);
                                    }
                                    //为刚创建的房间添加当前用户
                                    clientRoom.addUser(msg.userName, clientSocket);
                                    User newUser = new User(msg.userName, clientSocket, clientRoom);
                                    UserHandle.addUser(msg.userName, newUser);
                                    newUser.startListen();
                                }
                                else
                                {
                                    clientSocket.Send(Message.MessageHandle.serialize(Message.MessageHandle.genRoomNotifi("用户名已经存在,请换一个!", Message.MessageTypeEnum.warn)));
                                    LogService.Log(Message.MessageHandle.genRoomNotifi("用户名" + msg.userName + "已经存在,请换一个!", Message.MessageTypeEnum.warn));
                                    //?

                                }

                            }
                        }
                    }

                    catch (Exception e)
                    {

                        Debug.WriteLine(e.ToString());
                        LogService.LogError(e.ToString());
                    }
                }
            }
        }
    }
}
