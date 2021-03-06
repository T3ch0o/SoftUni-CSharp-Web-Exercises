﻿namespace SIS.WebServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using SIS.WebServer.Api.Interfaces;

    using static System.Console;

    public class Server
    {
        private const string LocalhostIpAddress = "127.0.0.1";

        private readonly int _port;

        private readonly TcpListener _listener;

        private readonly IHttpHandler _handler;

        private bool _isRuning;

        public Server(int port, IHttpHandler handler)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Parse(LocalhostIpAddress), port);
            _handler = handler;
        }

        public void Run()
        {
            _listener.Start();
            _isRuning = true;

            WriteLine($"Server started at http//{LocalhostIpAddress}:{_port}");

            ListenLoop().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task ListenLoop()
        {
            while (_isRuning)
            {
                Socket client = await _listener.AcceptSocketAsync();
                ConnectionHandler connectionHandler = new ConnectionHandler(client, _handler);
                await connectionHandler.ProcessRequestAsync();
            }
        }
    }
}