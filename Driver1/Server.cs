using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Driver1
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Server : Interface.IDriver
    {
        protected ConcurrentDictionary<Interface.IDevice, int> devices = new ConcurrentDictionary<Interface.IDevice, int>();
        // State of the server
        protected Bitmap image;
        protected int serverTemperature;
        // Available operations
        private string operation1 = "Zmniejsz chłodzenie";
        private string operation2 = "Zwiększ chłodzenie";

        public Server()
        {
            image = new Bitmap(@"data/server.png");
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

        protected void BroadcastState()
        {
            Bitmap stateImage = PrepareImage();
            var users = devices.ToArray();

            foreach (var device in users) {
                try
                {
                    // Set image with state
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stateImage.Save(ms, ImageFormat.Png);
                        device.Key.SetImage(ms.ToArray());
                    }

                    // Set context menu with available operations
                    device.Key.SetMenuItems(new string[] {operation1, operation2});
                }
                catch (CommunicationException e) 
                {
                    Console.WriteLine("Error when writing to client: {0}", e.Message);
                }
            }
        }

        public void Execute(string action)
        {
            if (action == operation1 && serverTemperature <= 80)
                serverTemperature += 10;

            if (action == operation2 && serverTemperature >= 40)
                serverTemperature -= 10;

            BroadcastState();
        }

        public void Disconnect()
        {
            Console.WriteLine("{0} - Client called 'Disconnect', {1} users active", 
                              DateTime.Now, devices.Count - 1);
        }
    }
}
