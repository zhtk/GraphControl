﻿using System;
using System.ServiceModel;

namespace Driver1
{
    class Program
    {
        /// <summary>
        /// Obiekt sterownika udającego pierwszy serwer
        /// </summary>
        static Server1 serviceInstance1 = new Server1();
        /// <summary>
        /// Obiekt sterownika udającego drugi serwer
        /// </summary>
        static Server2 serviceInstance2 = new Server2();
        /// <summary>
        /// Obiekt sterownika udającego switch
        /// </summary>
        static SwitchServer serviceInstance3 = new SwitchServer();

        /// <summary>
        /// Główna funkcja, tworzy obiekty sterowników i zaczyna nasłuchiwanie
        /// </summary>
        static void Main()
        {
            Console.WriteLine("Driver1 Server");
            Console.Write("Starting WCF listener...");

            using (ServiceHost host1 = new ServiceHost(serviceInstance1))
            using (ServiceHost host2 = new ServiceHost(serviceInstance2))
            using (ServiceHost host3 = new ServiceHost(serviceInstance3))
            {
                host1.Open();
                host2.Open();
                host3.Open();
                Console.WriteLine(" started.\n");
                Console.WriteLine("Press [ENTER] to quit.\n");
                Console.ReadLine();
            }
        }
    }
}
