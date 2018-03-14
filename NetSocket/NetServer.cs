using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace JLM.NetSocket
{
    public class NetServer : NetBase
    {
        private List<NetBase> clientList = new List<NetBase>();

        #region Events
        /// <summary>A socket has requested a connection</summary>
        public event EventHandler<NetSockConnectionRequestEventArgs> ConnectionRequested;
        #endregion

        #region Listen
        /// <summary>Listen for incoming connections</summary>
        /// <param name="port">Port to listen on</param>
        public void Listen(int port)
        {
            try
            {
                if (this.socket != null)
                {
                    try
                    {
                        this.socket.Close();
                    }
                    catch { }; // ignore problems with old socket
                }
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, port);
                this.socket.Bind(ipLocal);
                this.socket.Listen(1);
                this.socket.BeginAccept(new AsyncCallback(this.AcceptCallback), this.socket);
                this.OnChangeState(SocketState.Listening);
            }
            catch (Exception ex)
            {
                this.OnErrorReceived("Listen", ex);
            }
        }

        /// <summary>Callback for BeginAccept</summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket sock = listener.EndAccept(ar);

                if (this.state == SocketState.Listening)
                {
                    if (this.socket != listener)
                    {
                        this.Close("Async Listen Socket mismatched");
                        return;
                    }

                    if (this.ConnectionRequested != null)
                        this.ConnectionRequested(this, new NetSockConnectionRequestEventArgs(sock));
                }

                if (this.state == SocketState.Listening)
                    this.socket.BeginAccept(new AsyncCallback(this.AcceptCallback), listener);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (SocketException ex)
            {
                this.Close("Listen Socket Exception");
                this.OnErrorReceived("Listen Socket", ex);
            }
            catch (Exception ex)
            {
                this.OnErrorReceived("Listen Socket", ex);
            }
        }
        #endregion

        #region Accept
        /// <summary>Accept the connection request</summary>
        /// <param name="client">Client socket to accept</param>
        public void Accept(Socket client)
        {
            try
            {
                if (this.state != SocketState.Listening)
                    throw new Exception("Cannot accept socket is " + this.state.ToString());

                var clientSock = new NetClient(client);
                clientList.Add(clientSock);

                //   this.socket = client;

                client.ReceiveBufferSize = this.byteBuffer.Length;
                client.SendBufferSize = this.byteBuffer.Length;

                clientSock.SetKeepAlive();

                clientSock.Disconnected = Disconnected;
                clientSock.DataArrived = DataArrived; //客户端事件等于服务器事件
                clientSock.ErrorReceived = ErrorReceived;

                //   clientSock.OnChangeState(SocketState.Connected);
                OnConnected(client);
                clientSock.Receive();
            }
            catch (Exception ex)
            {
                this.OnErrorReceived("Accept", ex);
            }
        }
        #endregion

        public override void Oneloop()
        {
            foreach (var netBase in clientList)
            {
                netBase.Oneloop();
            }

            clientList.RemoveAll(s => s.State == SocketState.Closed);
        }
    }
}