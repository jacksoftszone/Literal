// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LiteralTest {
    class MockIrcd {
        TcpListener serversocket;
        bool isListening;

        public MockIrcd(int port) {
            serversocket = new TcpListener(IPAddress.Loopback, port);
            isListening = true;
            AcceptLoop();
        }

        private async void AcceptLoop() {
            while (isListening) {
                TcpClient client = await serversocket.AcceptTcpClientAsync();
                HandleClient(client);
            }
        }

        private async void HandleClient(TcpClient client) {
            NetworkStream stream = client.GetStream();
            byte[] bytes = new byte[1024];
            while (client.Connected) {
                // Get next block of message/s
                int read = await stream.ReadAsync(bytes, 0, 1024);
                string messages = Encoding.UTF8.GetString(bytes, 0, read);
            }
        }

        public void Stop() {
            isListening = false;
            serversocket.Stop();
        }
    }
}
