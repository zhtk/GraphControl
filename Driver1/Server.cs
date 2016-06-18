using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Driver1
{
    /// <summary>
    /// Klasa bazowa emulująca urządzenie, w którym można zmieniać temperaturę
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class BaseServer : Interface.IDriver
    {
        // Worek z klientami
        protected ConcurrentDictionary<Interface.IDevice, int> devices = new ConcurrentDictionary<Interface.IDevice, int>();
        // Zmienne trzymające stan serwera
        protected Bitmap image;
        protected int serverTemperature;
        // Napisy z dostępnymi operacjami
        private string operation1 = "Zmniejsz chłodzenie";
        private string operation2 = "Zwiększ chłodzenie";

        public BaseServer()
        {
            image = new Bitmap(@"data/server.png");
            serverTemperature = 50;
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

        /// <summary>
        /// Nadrukowuje na obrazek serwera napis z temperaturą
        /// </summary>
        /// <returns> Gotowy obraz </returns>
        protected Bitmap PrepareImage()
        {
            Bitmap img = new Bitmap(image);
            RectangleF rectf = new RectangleF(10, 70, 80, 30);
            Graphics g = Graphics.FromImage(img);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(serverTemperature + " st. C", new Font("Tahoma", 14), Brushes.Red, rectf);

            g.Flush();
            return img;
        }

        /// <summary>
        /// Rozgłasza stan urządzenia wszystkim podłączonym subskrynentom
        /// </summary>
        protected void BroadcastState()
        {
            Bitmap stateImage = PrepareImage();
            var users = devices.ToArray();

            foreach (var device in users) {
                try
                {
                    // Ustawia obrazek z stanem
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stateImage.Save(ms, ImageFormat.Png);
                        device.Key.SetImage(ms.ToArray());
                    }

                    // Ustawia menu kontekstowe z dostępnymi operacjami
                    device.Key.SetMenuItems(new string[] {operation1, operation2});
                }
                catch (CommunicationException e) 
                {
                    Console.WriteLine("Error when writing to client: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Wydaje sterownikowi rozkaz wykonania jakiejś akcji
        /// </summary>
        /// <param name="action"> Napis, który pojawił się na menu kontekstowym </param>
        public void Execute(string action)
        {
            if (action == null)
                return;

            if (action == operation1 && serverTemperature <= 80)
                serverTemperature += 10;

            if (action == operation2 && serverTemperature >= 40)
                serverTemperature -= 10;

            BroadcastState();
        }

        /// <summary>
        /// Rozłącza klienta z sterownikiem
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine("{0} - Client called 'Disconnect', {1} users active", 
                              DateTime.Now, devices.Count - 1);
        }
    }

    /// <summary>
    /// Klasa reprezentująca pierwszy serwer
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Server1 : BaseServer
    {
    }

    /// <summary>
    /// Klasa reprezentująca drugi serwer
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Server2 : BaseServer
    {
    }

    /// <summary>
    /// Klasa reprezentująca switch, wyświetla tylko jego obrazek
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class SwitchServer : Interface.IDriver
    {
        protected ConcurrentDictionary<Interface.IDevice, int> devices = new ConcurrentDictionary<Interface.IDevice, int>();
        protected Bitmap image;

        public SwitchServer()
        {
            image = new Bitmap(@"data/switch.png");
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
        /// Rozgłasza stan urządzenia wszystkim podłączonym subskrynentom - ustawia 
        /// obrazek i ukrywa menu kontekstowe
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

                    device.Key.SetMenuItems(null);
                }
                catch (CommunicationException e)
                {
                    Console.WriteLine("Error when writing to client: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Wydaje sterownikowi rozkaz wykonania pewnej akcji (w tym przypadku nic nie robi)
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
