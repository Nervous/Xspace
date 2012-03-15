using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace Xspace.Multiplayer
{
    class Player
    {
        string host, pseudo;
        int port;
        Socket socket;
        StreamReader reader;
        StreamWriter writer;
        Thread t_receiver;

        public Player(string pseudo)
        {
            this.pseudo = pseudo;
        }

        public void Connect(string host, int port)
        {
            try
            {
                this.host = host;
                this.port = port;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(host, port);
                Stream stream = new NetworkStream(socket);
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
                Console.WriteLine("Welcome " + pseudo);
            }
            catch
            {
                Console.WriteLine("Fail connect");
            }
        }


    
    }
}
