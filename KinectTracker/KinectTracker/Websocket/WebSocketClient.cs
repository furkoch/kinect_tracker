using KinectTracker.Websocket.EventModels;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Threading.Tasks;

namespace KinectTracker.Websocket
{
    public class WebSocketClient
    {
        public ulong port;
        public string host;
        public ulong logFileInterval;
        public bool isLogFileStatic;
        public bool arduinoPort;
        public bool outputOnConsole;
        public bool outputEventCounts;
        public WebSocket _webSocket;

        public WebSocketClient()
        {
            this.port = 3000;
            this.host = "ws://localhost";
            _webSocket = new WebSocket(host + ":" + port);
        }

        public WebSocketClient(string host, ulong port)
        {
            this.port = port;
            this.host = host;
            _webSocket = new WebSocket(host+port);
        }

        public void InitializeConnection()
        {           
            _webSocket.Connect();
        }

        public bool SendData(string eventJSON)
        {
            var status = _webSocket.Send(eventJSON);
            return status.Result;
        }

    }
}
