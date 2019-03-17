﻿using System;
using System.Threading;
using System.ServiceModel;

namespace TrueMarbleData
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ServiceHost host;
            NetTcpBinding tcp_binding;

            tcp_binding = new NetTcpBinding();
            tcp_binding.OpenTimeout = new TimeSpan(0, 10, 0);
            tcp_binding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcp_binding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

            host = new ServiceHost(typeof(TMDataControllerImpl));
            host.AddServiceEndpoint(typeof(ITMDataController), tcp_binding, "net.tcp://localhost:50001/TMData");
            host.Open();

            Console.WriteLine("Initialized");
            Thread.Sleep(Timeout.Infinite);

            host.Close();
        }
    }
}
