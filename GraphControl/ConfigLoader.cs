using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GraphControl
{
    class ConfigLoader
    {
        XDocument document;
        Dictionary<String, DeviceObject> objects = new Dictionary<String, DeviceObject>();
        private int width, height;
        
        public ConfigLoader(String file = "Operator.xml")
        {
            document = XDocument.Load(file);
            LoadScreenSettings();
            CreateObjects();
            LinkObjects();
        }

        public int Width 
        {
            get 
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        /// <summary>
        /// Loads screen settings from XML
        /// </summary>
        private void LoadScreenSettings()
        {
            var settings = document.Element("operator").Element("screen").Elements();
            foreach (var elem in settings) {
                if (elem.Name == "width")
                    width = int.Parse(elem.Value);
                else if (elem.Name == "height")
                    height = int.Parse(elem.Value);
            }
        }

        /// <summary>
        /// Loads device objects from XML
        /// </summary>
        private void CreateObjects()
        {
            var objects = document.Element("operator").Element("objects").Elements();
            Image img = new Bitmap("img/error.bmp");

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

        private void LinkObjects()
        { 
            // TODO
        }
    }
}
