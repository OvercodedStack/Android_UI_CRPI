﻿//Code Adopt from https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb
//A key note is that this class ONLY specializes in being a server, no other functions other than getters and setters are here.
//The server handles TCP strings incoming and outgoing. 


using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace TPC_Server
{
    public class TCP_Server : MonoBehaviour
    {
        #region private members 	
        /// <summary> 	
        /// TCPListener to listen for incomming TCP connection 	
        /// requests. 	
        /// </summary> 	
        private TcpListener tcpListener;
        /// <summary> 
        /// Background thread for TcpServer workload. 	
        /// </summary> 	
        private Thread tcpListenerThread;
        /// <summary> 	
        /// Create handle to connected tcp client. 	
        /// </summary> 	
        private TcpClient connectedTcpClient;
        #endregion
        public string IPC_comms_message = "null_msg";
        public string IPC_output = "null";
        public string IP_adress = "127.0.0.1";
        public int Port_adress = 27000;
        // Update is called once per frame
        [Range(0.1F, 20)]
        public float delay_time = 2.0F;
        private float last_call = 0.0F;
        private const int MAX_RETRIES = 15;
        public bool server_status;

        // Use this for initialization
        void Start()
        {
            server_status = false; 
            // Start TcpServer background thread 		
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time > last_call)
            {
                last_call = Time.time + delay_time;
                SendMessage(IPC_comms_message);
            }
        }

        public string get_msg()
        {
            return IPC_output;
        }

        public void set_msg(string msg_out)
        {
            IPC_comms_message = msg_out;
        }

        /// <summary> 	
        /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
        /// </summary> 	
        private void ListenForIncommingRequests()
        {
            try
            {
                // Create listener on localhost port 27000. 	
                Console.WriteLine(IP_adress);
                //tcpListener = new TcpListener(IPAddress.Parse(IP_adress), Port_adress);

                counter = 0; 
                tcpListener = new TcpListener(IPAddress.Any, Port_adress);
                tcpListener.Start();
                Debug.Log("Server is listening");
                Console.WriteLine("Server started!");
                Byte[] bytes = new Byte[1024];
                while (true)
                {
                    connectedTcpClient = tcpListener.AcceptTcpClient();
                    
                    {
                        // Get a stream object for reading 					
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;
                            // Read incomming stream into byte arrary. 						
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incommingData = new byte[length];
                                Array.Copy(bytes, 0, incommingData, 0, length);
                                // Convert byte array to string message. 							
                                string clientMessage = Encoding.ASCII.GetString(incommingData);
                                Debug.Log("client message received as: " + clientMessage);
                                IPC_output = clientMessage;
                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Server did not start!");
                Debug.Log("SocketException " + socketException.ToString());
            }
        }
        /// <summary> 	
        /// Send message to client using socket connection. 	
        /// </summary> 	

        private int counter; 
        private void SendMessage(string serverMessage)
        {
            if (connectedTcpClient == null)
            {
                server_status = false; 
                return;
            }

            if (counter >= MAX_RETRIES)
            {
                server_status = false;
                counter = 0;
                connectedTcpClient = null;
                return;
            }

            try
            {
                server_status = true;
                // Get a stream object for writing. 			
                NetworkStream stream = connectedTcpClient.GetStream();
                if (stream.CanWrite)
                {
                    //Debug.Log("Hi, I'm a robot!");
                    //string serverMessage = "This is a message from your server.";
                    // Convert string message to byte array.                 
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                    // Write byte array to socketConnection stream.               
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    Debug.Log("Server sent his message - should be received by client");
                    Console.WriteLine("Msg sent");
                    counter = 0; 
                }
                else if (!stream.CanWrite)
                {
                    counter++;
                    Debug.Log("Skipping");
                }
                ///Debug.Log("Hi, I'm a job!");
                ///
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }
    }
}