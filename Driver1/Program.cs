using System;
using System.ServiceModel;

namespace Driver1
{
    class Program
    {
        static Server serviceInstance = new Server();

        static void Main()
        {
            Console.WriteLine("Driver1 Server");
            Console.Write("Starting WCF listener...");

            // TODO 3 endpointy zamiast 1
            using (ServiceHost host = new ServiceHost(serviceInstance))
            {
                host.Open();
                Console.WriteLine(" started.\n");
                Console.WriteLine("Press [ENTER] to quit.\n");
                Console.ReadLine();
            }
        }
    }
}
