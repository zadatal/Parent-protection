#region ----------   ABOUT   -----------------------------
/*#################################################################                                            #
# Name: Server for GUI and python engine client                  #
# Version: 1.0                                                   #
# Windows Tested Versions: Win 7 32-bit                          #
# Python Tested Versions: 2.6 32-bit                             #
#################################################################*/
#endregion

#region ----------   USINGS   -----------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
#endregion

namespace ParentServer
{
    #region ----------   CLASSES----------------------------
    class PythonHandler
    {
        #region DATA
        // Referance to main form
        private ParentForm mainForm;
        // Socket for listening of GUI channell from python server
        TcpListener listener;
        // Socket of GUI channell from python server
        TcpClient pythonClient;
        // Thread for session from GUI channell of python server
        Thread pythonListenerThread;
        //  Flag for continue session from GUI channell of python server
        bool flagRun;
        #endregion

        #region CONSTRUCTOR
        public PythonHandler(ParentForm mainForm)
        {
            // Referance to main form
            this.mainForm = mainForm;
            // Thread for session from GUI channell of python server
            pythonListenerThread = new Thread(new ThreadStart(Run));
            pythonListenerThread.Start();
        }
        #endregion

        #region THREAD
        /// <summary>
        ///  Thread for session with PythonServer
        /// </summary>
        private void Run()
        {
            try
            {
                // Build listener for python engine
                listener = new TcpListener(9669);
                listener.Start();
                //  Wait connection from python engine and if successful then create new socket to python engine
                pythonClient = listener.AcceptTcpClient();
                mainForm.PrintToLog(DateTime.Now.ToShortTimeString() + " :  Server trying start...", Color.Black);
                listener.Stop(); // stop listening because python engine connected to GUI
                flagRun = true;
                // Asynchronic StateObject
                StateObject stateObject = new StateObject();
                stateObject.workSocket = pythonClient.Client;
                // Begins to asynchronously receive data from a connected socket with  python engine
                pythonClient.Client.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(Read_Callback), stateObject);
            }
            catch (SocketException se)
            {
                mainForm.PrintToLog(se.Message, Color.Red);
            }
            catch (Exception e)
            {
                mainForm.PrintToLog(e.Message, Color.Red);
            }
        }

        /// <summary>
        /// Asynchronously read (receive data) from input stream of connected socket 
        /// </summary>
        /// <param name="ar"> An IAsyncResult that stores state information and any user defined data for this asynchronous operation.</param>
        public void Read_Callback(IAsyncResult ar)
        {
            try
            {
                // An IAsyncResult that stores state information and any user defined data for this asynchronous operation
                StateObject stateObject = (StateObject)ar.AsyncState;
                if (pythonClient != null && flagRun)
                {
                    // Ends a pending asynchronous read.
                    int read = stateObject.workSocket.EndReceive(ar);

                    if (read > 0)
                    {
                        string msg = Encoding.ASCII.GetString(stateObject.buffer, 0, read);
                        //All of the data has been read, so check out what command

                        string[] items = msg.Split('#');
                        switch (items[0])  // command
                        {
                            case "StartChild": // Add new child
                                mainForm.AddNewChild(items[1],items[2]);
                                break;
                            case "Close":
                                mainForm.StopPythonEngine();
                                break;
                            default:
                                mainForm.PrintToLog(items[0], Color.Blue);
                                if (items[0].Contains("Aborting the server"))
                                    mainForm.StopPythonEngine();                                
                                break;
                        }
                    }
                    if ( flagRun )
                        stateObject.workSocket.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(Read_Callback), stateObject);
                }
            }
            catch (Exception se) 
            {
                mainForm.PrintToLog(se.Message, Color.Red);
                Close();
            }
        }
        #endregion

        /// <summary>
        /// Close this thread and connection
        /// </summary>
        public void Close()
        {
            if (pythonClient != null)
            {
                pythonClient.Close();
                pythonClient = null;
            }
            flagRun = false;
            if (pythonListenerThread.IsAlive)
                pythonListenerThread.Abort();
            mainForm.PrintToLog(DateTime.Now.ToShortTimeString() + " :  Server shutdown... ", Color.Black);
        }
        /// <summary>
        /// Send message to python engine client 
        /// </summary>
        /// <param name="msg">message for send</param>
        public void Send(string msg)
        {
            try
            {
                if (pythonClient != null)
                    pythonClient.Client.Send(Encoding.ASCII.GetBytes(msg));
            }
            catch (Exception se)
            {
                Close();
            }
        }
    }

    /// <summary>
    /// State object for reading client data asynchronously
    /// </summary>
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
    #endregion
}



