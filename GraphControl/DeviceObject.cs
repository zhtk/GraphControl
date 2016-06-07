using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.ServiceModel;
using System.IO;
using Interface;

namespace GraphControl
{
    public class RemoteDevice : IDevice
    {
        public DeviceObject Device { get; private set; }

        public RemoteDevice(DeviceObject device)
        {
            this.Device = device;
        }

        public void SetImage(byte[] image)
        {
            MemoryStream stream = new MemoryStream(image);
            Device.Image = new Bitmap(stream);
            Program.Screen.Invalidate();
        }

        public void SetMenuItems(String[] items)
        {
            if (items == null || items.Length == 0) {
                Device.Menu = null;
                return;
            }

            MenuItem[] menuItems = new MenuItem[items.Length];

            for (int i = 0; i < items.Length; ++i) {
                menuItems[i] = new MenuItem();
                menuItems[i].Text = items[i];
                menuItems[i].Click += new EventHandler(ActionOccured);
            }
            
            Device.Menu = menuItems;
        }

        private void ActionOccured(object sender, EventArgs args)
        {
            try
            {
                MenuItem item = (MenuItem) sender;
                Device.Connection.Execute(item.Text);
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("Cannot execute action: ", e.Message);
                Device.DisconnectServer();
            }
            catch (Exception e)
            { 
            }
        }
    }

    public class ServerConnection : DuplexClientBase<IDriver>, IDriver
    {
        RemoteDevice callback;

        public ServerConnection(RemoteDevice callbackInstance, String endpoint)
            : base(callbackInstance, endpoint) 
        {
            callback = callbackInstance;
        }

        public void Authenticate()
        {
            base.Channel.Authenticate();
        }

        public void Execute(string action)
        {
            base.Channel.Execute(action);
        }

        public void Disconnect()
        {
            base.Channel.Disconnect();
            callback.Device.DisconnectServer();
        }
    }

    public class DeviceObject
    {
        public String Id { get; private set; }
        public Point Position { get; set; }
        public Image Image { get; set; }
        public Size Size { get; set; }
        public List<EdgeObject> Edges { get; private set; }
        public MenuItem[] Menu { get; set; }
        private String endpoint;
        public ServerConnection Connection { get; private set; }
        
        public DeviceObject(String id, String serverInterface)
        {
            Id = id;
            Position = new Point();
            Size = new Size(0, 0);
            Edges = new List<EdgeObject>();
            endpoint = serverInterface;
            Connection = null;

            MakeMenu();
            Task.Run(() => ConnectServer());
        }

        private void ConnectServer()
        {
            RemoteDevice remote = new RemoteDevice(this);

            try
            {
                if (Connection != null)
                    Connection.Disconnect();

                Connection = new ServerConnection(remote, endpoint);
                Connection.Authenticate();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when connecting: {0}", e.Message);
                Connection = null;
            }
        }

        public void DisconnectServer()
        {
            Console.WriteLine("Disconnected from server");
            MakeMenu();
            Image = Image.FromFile(@"img/error.bmp");
            Program.Screen.Invalidate();
        }

        private void ConnectServer(object sender, EventArgs args)
        {
            ConnectServer();
        }

        private void MakeMenu()
        {
            MenuItem item = new MenuItem("Connect to server");
            item.Click += new EventHandler(ConnectServer);

            Menu = new MenuItem[] {
                item,
            };
        }

        public void AddEdge(EdgeObject edge)
        {
            Edges.Add(edge);
        }
    }

    public class EdgeObject
    {
        public DeviceObject PointA { get; private set; }
        public DeviceObject PointB { get; private set; }
        public List<GraphLine> Lines { get; private set; }

        public EdgeObject(DeviceObject from, DeviceObject to)
        {
            PointA = from;
            PointB = to;
            Lines = new List<GraphLine>();
        }

        public void AddLine(GraphLine line)
        {
            Lines.Add(line);
        }
    }

    public class GraphLine
    {
        public Point Begin { get; private set; }
        public Point End { get; private set; }
        public Pen Pen { get; set; }

        public GraphLine(Point begin, Point end)
        {
            Begin = begin;
            End = end;
            Pen = new Pen(Color.Black, 3);
        }

        public GraphLine(Point begin, Point end, Pen pen)
        {
            Begin = begin;
            End = end;
            Pen = pen;
        }
    }
}
