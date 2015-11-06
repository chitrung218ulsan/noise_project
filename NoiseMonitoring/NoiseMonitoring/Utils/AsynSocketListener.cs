using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoiseMonitoring.Utils
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1500;
        public byte[] buffer = new byte[BufferSize];
        
    }
    class AsynSocketListener
    {
        #region Members
        private static bool logEnable = true;
        private static AsynSocketListener instance;
        private static int dataPort;

        private static ManualResetEvent allDone = new ManualResetEvent(false);
        #endregion

        #region Methods
        public static AsynSocketListener Instance
        {
            get
            {
                // If the instance is null then create one and init the Queue
                if (instance == null)
                {
                    instance = new AsynSocketListener();
                   
                }
                return instance;
            }
        }


        public static String DataPort
        {
            set
            {
                try
                {
                    dataPort = Convert.ToInt32(value);
                    if (dataPort < 0 || dataPort > 65535)
                    {
                        System.Windows.Forms.MessageBox.Show("dataPort > 0 or dataPort < 65535");
                    }
                }

                catch (FormatException formatEx)
                {
                    throw new Exception(formatEx.Message + formatEx.Source + formatEx.TargetSite);
                }
                catch (OverflowException ex)
                {
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// Start to listen connection from client
        /// </summary>
        public void StartListenning()
        {
            try
            {
                if(logEnable)
                {
                    LogWriter writer = LogWriter.Instance;
                    writer.WriteToLog("Start Listening");
                }
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, dataPort);
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                listener.Bind(localEndPoint);
                listener.Listen(100);
                while (true)
                {

                    
                    /*while (AsynchronousSocketListener.suspend)
                    {
                        allDone.WaitOne();
                    }*/

                    
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "at: " + ex.TargetSite);
                LogWriter writer = LogWriter.Instance;
                writer.WriteToLog(ex.Message + "at: " + ex.TargetSite.ToString() );
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                
                allDone.Set();

                //MessageBox.Show("Accept");
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = null;
                handler = listener.EndAccept(ar);
                IPEndPoint ipEndPoint = handler.RemoteEndPoint as IPEndPoint;

               
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (SocketException ex)
            {
                LogWriter writer = LogWriter.Instance;
                writer.WriteToLog(ex.Message + " " + ex.TargetSite.ToString());
            }
            catch (Exception ex)
            {
                //writeLog(e.TargetSite + " " + e.Source + " " + e.Message);
                LogWriter writer = LogWriter.Instance;
                writer.WriteToLog(ex.Message + " " + ex.TargetSite.ToString());
            }
            
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
                IPEndPoint ipEndPoint = handler.RemoteEndPoint as IPEndPoint;
                try
                {
                
                }
                catch (SocketException e)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Disconnect(true);
                    handler.Close();
                    handler.Dispose();
                }
            }
            
            catch(Exception e)
            {
                
            }
        }
        #endregion
    }
}
