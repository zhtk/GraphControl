﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GraphControl
{
    /// <summary>
    /// Ładuje konfigurację z podanego pliku, domyślnie operator.xml
    /// </summary>
    public class ConfigLoader
    {
        private XDocument document;
        private Dictionary<String, DeviceObject> objects = new Dictionary<String, DeviceObject>();
        private List<GraphLine> lines = new List<GraphLine>();
        /// <summary>
        /// Podaje szerokość okna z panelem operatorskim
        /// </summary>
        public int ScreenWidth { get; private set; }
        /// <summary>
        /// Podaje wysokość okna z panelem operatorskim
        /// </summary>
        public int ScreenHeight { get; private set; }
        
        public ConfigLoader(String file = "Operator.xml")
        {
            document = XDocument.Load(file);
            LoadScreenSettings();
            CreateObjects();
            LinkObjects();
        }

        /// <summary>
        /// Daje tablicę z obiektami, które reprezentują urządzenia na ekranie
        /// operatorskim
        /// </summary>
        public DeviceObject[] DeviceObjects
        {
            get
            {
                return objects.Select(item => item.Value).ToArray();
            }
        }

        /// <summary>
        /// Daje linie tworzące krawędzie na ekranie
        /// </summary>
        public List<GraphLine> EdgeLines
        {
            get
            {
                return lines;
            }
        }

        /// <summary>
        /// Wczytuje ustawienia ekranu z XMLa
        /// </summary>
        private void LoadScreenSettings()
        {
            var settings = document.Element("operator").Element("screen").Elements();
            foreach (var elem in settings) {
                if (elem.Name == "width")
                    ScreenWidth = int.Parse(elem.Value);
                else if (elem.Name == "height")
                    ScreenHeight = int.Parse(elem.Value);
            }
        }

        /// <summary>
        /// Ładuje i tworzy obiekty urządzeń
        /// </summary>
        private void CreateObjects()
        {
            var objects = document.Element("operator").Element("objects").Elements();
            Image img = new Bitmap(@"img/error.bmp");

            foreach (var elem in objects) {
                DeviceObject obj = new DeviceObject(elem.Attribute("id").Value, 
                                                    elem.Attribute("interface").Value);
                obj.Position = new Point(int.Parse(elem.Attribute("posx").Value),
                                         int.Parse(elem.Attribute("posy").Value));
                obj.Size = new Size(int.Parse(elem.Attribute("width").Value),
                                    int.Parse(elem.Attribute("height").Value));
                obj.Image = new Bitmap(img);

                this.objects[obj.Id] = obj;
            }
        }

        /// <summary>
        /// Wczytuje jedną, podaną krawędź, tworzy linki pomiędzy obiektami urządzeń
        /// i wczytuje linie składające się na krawędź
        /// </summary>
        /// <param name="edge"> Referencja do tej części konfiguracji, z której
        /// ma zostać wczytana krawędź </param>
        private void ParseEdge(XElement edge)
        {
            
            var points = edge.Element("connecting");

            DeviceObject pointA = objects[points.Attribute("pointA").Value];
            DeviceObject pointB = objects[points.Attribute("pointB").Value];

            EdgeObject edgeObject = new EdgeObject(pointA, pointB);

            foreach (var line in edge.Elements("line")) {
                Point begin = new Point(int.Parse(line.Attribute("beginx").Value),
                                        int.Parse(line.Attribute("beginy").Value));
                Point end = new Point(int.Parse(line.Attribute("endx").Value),
                                      int.Parse(line.Attribute("endy").Value));

                GraphLine graphLine = new GraphLine(begin, end);
                lines.Add(graphLine);
                edgeObject.AddLine(graphLine);
            }

            pointA.AddEdge(edgeObject);
            pointB.AddEdge(edgeObject);
        }

        /// <summary>
        /// Wczytuje krawędzie i łączy obiekty urządzeń
        /// </summary>
        private void LinkObjects()
        {
            var edges = document.Element("operator").Element("edges").Elements();

            foreach (var edge in edges)
                ParseEdge(edge);
        }
    }
}
