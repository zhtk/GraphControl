using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphControl
{
    public partial class MainScreen : Form
    {
        private DeviceObject[] deviceObjects;
        private List<GraphLine> lines;

        public MainScreen(DeviceObject[] deviceObjects, List<GraphLine> lines)
        {
            InitializeComponent();

            this.deviceObjects = deviceObjects;
            this.lines = lines;

            SuspendLayout();
            this.MouseClick += new MouseEventHandler(this.ShowContextMenu);
            ResumeLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw edges
            foreach (GraphLine line in lines)
                e.Graphics.DrawLine(line.Pen, line.Begin, line.End);

            // Repaint devices on screen
            foreach (DeviceObject device in deviceObjects)
                e.Graphics.DrawImage(device.Image, device.Position);
        }

        private void ShowContextMenu(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                foreach (DeviceObject item in deviceObjects)
                {
                    Point click = new Point(e.X, e.Y);
                    Rectangle itemRect = new Rectangle(item.Position, item.Size);

                    if (itemRect.Contains(click))
                    {
                        this.ContextMenu = new ContextMenu(item.Menu);
                        this.ContextMenu.Show(this, click);
                        break;
                    }
                }

                this.ContextMenu = null;
            }
        }
    }
}
