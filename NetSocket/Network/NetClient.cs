using System;
using System.Net;
using System.Net.Sockets;

namespace JLM.NetSocket
{
    public class NetClient : NetBase
    {
        #region Constructor
        public NetClient() 
            : base() { }

        public NetClient(Socket s) 
            : base()
        {
            socket = s;

            state = SocketState.Connected;
        }

        #endregion

        public int ClientId { get; set; }

        public IPAddress Ip
        {
            get { return ((IPEndPoint)socket.RemoteEndPoint).Address;}
        }

        #region Connect
        /// <summary>Connect to the computer specified by Host and Port</summary>
        public bool Connect(IPEndPoint endPoint)
        {
            if (this.state == SocketState.Connected)
                return true; // already connecting to something

            try
            {
                if (this.state != SocketState.Closed)
                    throw new Exception("Cannot connect socket is " + this.state.ToString());

                this.OnChangeState(SocketState.Connecting);

                if (this.socket == null)
                    this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IAsyncResult connResult = this.socket.BeginConnect(endPoint, new AsyncCallback(this.ConnectCallback), this.socket);
                connResult.AsyncWaitHandle.WaitOne(500, true);  //�ȴ�500m��

                if (!connResult.IsCompleted)
                {
                    this.Close("Connect Exception");
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHandlerRegister.Log("Connect " + ex);
                this.Close("Connect Exception");
            }
            return false;
        }

        /// <summary>Callback for BeginConnect</summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket sock = (Socket)ar.AsyncState;
                sock.EndConnect(ar);

                if (this.socket != sock)
                {
                    this.Close("Async Connect Socket mismatched");
                    return;
                }

                if (this.state != SocketState.Connecting)
                    throw new Exception("Cannot connect socket is " + this.state.ToString());

                this.socket.ReceiveBufferSize = this.byteBuffer.Length;
                this.socket.SendBufferSize = this.byteBuffer.Length;
				
                this.SetKeepAlive();

                this.OnChangeState(SocketState.Connected);
                this.OnConnected(this);

                this.Receive();
            }
            catch (Exception ex)
            {
                this.Close("Socket Connect Exception");
                LogHandlerRegister.Log("Socket Connect " + ex);
            }
        }
        #endregion

        public override void Oneloop()
        {
            msgPump.HandleReceive();
        }
    }
}