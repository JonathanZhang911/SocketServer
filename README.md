##简单的Socket服务器
----
实现了多用户多房间的转发,可以自定义消息类型的简单的Socket服务器

应用场景主要是需要多用户分房间的消息转发,比如聊天室,状态同步之类的.


`分为三个部分`


###Message Class Library

* 包含了客户端和服务段所需要的Message类
* 客户端和服务段需要通过这个dll来序列和反序列化通讯中接受和发送的消息
* 可以自行修改这个类,根据自己项目消息的要求修改
* 客户端和服务端都需要引用这个Project
* 可以生成DLL,供其它Project应用,如Unity什么的


###SocketServer
* 服务端WPF程序
* 主要包含User,Room,LogService构成
* 可以实现多个用户不同房间同时发送或者接受消息
* LogService需要自己根据前面的Message修改
* 可以在User中添加注册或者验证等功能(项目没有添加)
* 可以添加点对点消息转发功能(项目里面没有)

###Client
* 简单的客户端Demo
* 主要通过ChatService类,接受到消息的处理方式需要在这个类中重新写
 
##LICENSE
MIT
