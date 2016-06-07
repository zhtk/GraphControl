using System;
using System.ServiceModel;

namespace Driver2
{
    class Program
    {
        static Server serviceInstance = new Server();

        static void Main()
        {
            Console.WriteLine("Driver1 Server");
            Console.WriteLine("Close window to stop server");

            while (true)
            {
                Console.Write("Starting WCF listener...");

                using (ServiceHost host = new ServiceHost(serviceInstance))
                {
                    host.Open();
                    Console.WriteLine(" started.\n");

                    System.Threading.Thread.Sleep(5000);
                    host.Abort();
                }

                Console.WriteLine("Oooops, it seems that server has crashed!");
            }
        }
    }
}
