using System;
using System.Collections.Concurrent;
using System.IO;
using System.ServiceModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Driver2
{
    /// <summary>
    /// Serwer aplikacji
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Server : Interface.IDriver
    {
        protected ConcurrentDictionary<Interface.IDevice, int> devices = new ConcurrentDictionary<Interface.IDevice, int>();
        protected Bitmap image;

        public Server()
        {
            image = new Bitmap(@"data/server.png");
        }

        /// <summary>
        /// Podłączenie się do sterownika i wpisanie na jego listę subskrynentów
        /// </summary>
        public void Authenticate()
        {
            OperationContext.Current.Channel.Faulted += (sender, args) =>
                Console.WriteLine("{0} - Client connection failed.", DateTime.Now);
            OperationContext.Current.Channel.Closed += (sender, args) =>
            {
                int unused;
                devices.TryRemove((Interface.IDevice)sender, out unused);
                Console.WriteLine("{0} - Client connection closed. {1} active.",
                                  DateTime.Now, devices.Count);
            };

            Interface.IDevice user = OperationContext.Current.GetCallbackChannel<Interface.IDevice>();
            devices.TryAdd(user, 0);
            Console.WriteLine("{0} - Client called 'Authenticate'. {1} active.",
                              DateTime.Now, devices.Count);

            BroadcastState();
        }

        /// <summary>
        /// Rozgłasza stan urządzenia wszystkim podłączonym subskrynentom
        /// </summary>
        protected void BroadcastState()
        {
            var users = devices.ToArray();

            foreach (var device in users)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Png);
                        device.Key.SetImage(ms.ToArray());
                    }

                    device.Key.SetMenuItems(new string[] { "Uruchom chkdisk" });
                }
                catch (CommunicationException e)
                {
                    Console.WriteLine("Error when writing to client: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Wydaje sterownikowi rozkaz wykonania pewnej akcji
        /// </summary>
        /// <param name="action"> Napis, który pojawił się na menu kontekstowym </param>
        public void Execute(string action)
        {
            BroadcastState();
        }

        /// <summary>
        /// Rozłącza z sterownikiem
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine("{0} - Client called 'Disconnect', {1} users active",
                              DateTime.Now, devices.Count - 1);
        }
    }
}
