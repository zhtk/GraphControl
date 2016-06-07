using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Driver1
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Server : Interface.IDriver
    {
        protected ConcurrentDictionary<Interface.IDevice, int> devices = new ConcurrentDictionary<Interface.IDevice, int>();
        protected Bitmap image;
        protected int serverTemperature;

        public Server()
        {
            image = new Bitmap("data/server.png");
            serverTemperature = 50;
        }

        public void Authenticate()
        {
            OperationContext.Current.Channel.Faulted += (sender, args) =>
                Console.WriteLine("{0} - Client connection failed.", DateTime.Now);
            OperationContext.Current.Channel.Closed += (sender, args) =>
            {
                int unused;
                devices.TryRemove((Interface.IDevice) sender, out unused);
                Console.WriteLine("{0} - Client connection closed. {1} active.", 
                                  DateTime.Now, devices.Count);
            };

            Interface.IDevice user = OperationContext.Current.GetCallbackChannel<Interface.IDevice>();
            devices.TryAdd(user, 0);
            Console.WriteLine("{0} - Client called 'Authenticate'. {1} active.", 
                              DateTime.Now, devices.Count);

            BroadcastState();
        }

        protected void BroadcastState()
        {
            var users = devices.ToArray();

            foreach (var device in users) {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Png);
                        device.Key.SetImage(ms.ToArray());
                    }
                }
                catch (CommunicationException e) 
                {
                    Console.WriteLine("Błąd podczas pisania do klienta: {0}", e.Message);
                }
            }
        }

        public void Execute(string action)
        { 
            // TODO

            BroadcastState();
        }

        public void Disconnect()
        {
            Console.WriteLine("{0} - Client called 'Disconnect', {1} users active", 
                              DateTime.Now, devices.Count - 1);
        }
    }
}
