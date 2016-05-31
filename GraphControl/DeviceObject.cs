using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GraphControl
{
    class DeviceObject
    {
        private String id;
        public Point Position { get; set; }
        public Image Image { get; set; }
        public Size Size { get; set; }
        // TODO Server interface
        
        public DeviceObject(String id, String server)
        {
            this.id = id;
            Position = new Point();
            Size = new Size(0, 0);
            // TODO
        }

        public String Id
        {
            get
            {
                return id;
            }
        }
    }
}
