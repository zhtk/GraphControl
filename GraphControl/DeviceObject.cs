using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace GraphControl
{
    public class DeviceObject
    {
        public String Id { get; private set; }
        public Point Position { get; set; }
        public Image Image { get; set; }
        public Size Size { get; set; }
        public List<EdgeObject> Edges { get; private set; }
        public MenuItem[] Menu { get; private set; }

        // TODO Server interface
        
        public DeviceObject(String id, String server)
        {
            Id = id;
            Position = new Point();
            Size = new Size(0, 0);
            Edges = new List<EdgeObject>();
            // TODO server interface

            MakeMenu();
        }

        private void MakeMenu()
        {
            Menu = new MenuItem[] {
                new MenuItem("Connect to server"),
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
