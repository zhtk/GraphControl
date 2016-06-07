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
    class RemoteDevice : IDevice
    {
        private DeviceObject device;

        public RemoteDevice(DeviceObject device)
        {
            this.device = device;
        }

        public void SetImage(byte[] image)
        {
            MemoryStream stream = new MemoryStream(image);
            device.Image = new Bitmap(stream);
        }

        public void SetMenuItems(String[] items)
        {
            // TODO poprawka itemów i delegaci
            //device.Menu = items;
        }
    }

    public class ServerConnection : DuplexClientBase<IDriver>, IDriver
    {
        public ServerConnection(IDevice callbackInstance, String endpoint)
            : base(callbackInstance, endpoint) {}

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
        private ServerConnection connection = null;
        
        public DeviceObject(String id, String serverInterface)
        {
            Id = id;
            Position = new Point();
            Size = new Size(0, 0);
            Edges = new List<EdgeObject>();
            endpoint = serverInterface;

            MakeMenu();
            ConnectServer(); // TODO Przenieść do tasków
        }

        private void ConnectServer()
        {
            RemoteDevice remote = new RemoteDevice(this);

            try
            {
                if (connection != null)
                    connection.Disconnect();

                connection = new ServerConnection(remote, endpoint);
                // TODO Handler na wypadek zerwania połączenia
                connection.Authenticate();
            }
            catch (Exception e) 
            { 
                
            }
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
