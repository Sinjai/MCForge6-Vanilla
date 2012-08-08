using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Test {
    public class Player {

        internal readonly NetworkStream  NetworkStream;
        internal readonly TcpClient              Client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public Player(IPEndPoint EndPoint) {
            Client = new TcpClient(EndPoint);
            NetworkStream = Client.GetStream();
        }

        public void SendMessage(string message) {

        }

    }
}
