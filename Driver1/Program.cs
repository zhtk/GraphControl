using System;
using System.ServiceModel;

namespace Driver1
{
    class Program
    {
        static Server1 serviceInstance1 = new Server1();
        static Server2 serviceInstance2 = new Server2();
        static SwitchServer serviceInstance3 = new SwitchServer();

        static void Main()
        {
            Console.WriteLine("Driver1 Server");
            Console.Write("Starting WCF listener...");

            // TODO 3 endpointy zamiast 1
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
