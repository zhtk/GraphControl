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
    /// <summary>
    /// Klasa odbierająca callbacki od serwera i wykonująca w związku z tym jakąś akcję
    /// </summary>
    public class RemoteDevice : IDevice
    {
        public DeviceObject Device { get; private set; }

        public RemoteDevice(DeviceObject device)
        {
            this.Device = device;
        }

        /// <summary>
        /// Ustawia zdjęcie obrazujące stan urządzenia
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(byte[] image)
        {
            MemoryStream stream = new MemoryStream(image);
            Device.Image = new Bitmap(stream);
            Program.Screen.Invalidate();
        }

        /// <summary>
        /// Ustawia możliwe do wykonania na obiekcie akcje
        /// </summary>
        /// <param name="items"></param>
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

        /// <summary>
        /// Wysyła do sterownika żądanie podjęcia jakiejś akcji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

    /// <summary>
    /// Nakładka na interfejs IDriver
    /// </summary>
    public class ServerConnection : DuplexClientBase<IDriver>, IDriver
    {
        RemoteDevice callback;

        public ServerConnection(RemoteDevice callbackInstance, String endpoint)
            : base(callbackInstance, endpoint) 
        {
            callback = callbackInstance;
        }

        /// <summary>
        /// Podłączenie się do sterownika i wpisanie na jego listę subskrynentów
        /// </summary>
        public void Authenticate()
        {
            base.Channel.Authenticate();
        }

        /// <summary>
        /// Wydaje sterownikowi rozkaz wykonania pewnej akcji
        /// </summary>
        /// <param name="action"> Napis, który pojawił się na menu kontekstowym </param>
        public void Execute(string action)
        {
            base.Channel.Execute(action);
        }

        /// <summary>
        /// Rozłącza z sterownikiem
        /// </summary>
        public void Disconnect()
        {
            base.Channel.Disconnect();
            callback.Device.DisconnectServer();
        }
    }

    /// <summary>
    /// Reprezentuje jedno urządzenie wyświetlane na ekranie
    /// </summary>
    public class DeviceObject
    {
        /// <summary>
        /// Identyfikator urządzenia
        /// </summary>
        public String Id { get; private set; }
        /// <summary>
        /// Pozycja obrazka urządzenia na ekranie
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// Obrazek wyświetlany na ekranie
        /// </summary>
        public Image Image { get; set; }
        /// <summary>
        /// Rozmiar obiektu na ekranie
        /// </summary>
        public Size Size { get; set; }
        /// <summary>
        /// Krawędzie wychodzące z obiektu
        /// </summary>
        public List<EdgeObject> Edges { get; private set; }
        /// <summary>
        /// Menu kontekstowe zawierające listę możliwych do wykonania akcji
        /// </summary>
        public MenuItem[] Menu { get; set; }
        /// <summary>
        /// Nazwa endpointa sterownika w App.config do którego ma się podłączyć obiekt
        /// </summary>
        private String endpoint;
        /// <summary>
        /// Obiekt reprezentujący aktywne połączenie z sterownikiem
        /// </summary>
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

        /// <summary>
        /// Podłącza obiekt do zdalnego sterownika
        /// </summary>
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

        /// <summary>
        /// Rozłącza obiekt z zdalnym sterownikiem
        /// </summary>
        public void DisconnectServer()
        {
            Console.WriteLine("Disconnected from server");
            MakeMenu();
            Image = Image.FromFile(@"img/error.bmp");
            Program.Screen.Invalidate();
        }

        /// <summary>
        /// Podłącza obiekt do zdalnego sterownika. Funkcja używana do obsługi zdarzeń
        /// </summary>
        private void ConnectServer(object sender, EventArgs args)
        {
            ConnectServer();
        }

        /// <summary>
        /// Tworzy początkowe menu kontekstowe z jedną opcją: podłączającą do serwera
        /// </summary>
        private void MakeMenu()
        {
            MenuItem item = new MenuItem("Connect to server");
            item.Click += new EventHandler(ConnectServer);

            Menu = new MenuItem[] {
                item,
            };
        }

        /// <summary>
        /// Dodaje krawędź wychodzącą z obiektu
        /// </summary>
        /// <param name="edge"></param>
        public void AddEdge(EdgeObject edge)
        {
            Edges.Add(edge);
        }
    }

    /// <summary>
    /// Reprezentuje krawędź, na którą składają się łączone obiekty i kreski na ekranie
    /// </summary>
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

        /// <summary>
        /// Dodaje kreskę składającą się na krawędź
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(GraphLine line)
        {
            Lines.Add(line);
        }
    }

    /// <summary>
    /// Reprezentuje jedną kreskę na ekranie
    /// </summary>
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
