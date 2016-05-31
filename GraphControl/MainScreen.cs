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
        ConfigLoader loader;

        public MainScreen()
        {
            InitializeComponent();

            loader = new ConfigLoader();
            SuspendLayout();
            // TODO
            ResumeLayout();
        }

        private void ShowContextMenu(object sender, MouseEventArgs e)
        {
            bool match = false;
            /*
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    //Microsoft.Msagl.Node.BoundingBox.Contains
                    if (item.Bounds.Contains(new Point(e.X, e.Y)))
                    {
                        MenuItem[] mi = new MenuItem[] { new MenuItem("Hello"), new MenuItem("World"), new MenuItem(item.Name) };
                        listView1.ContextMenu = new ContextMenu(mi);
                        match = true;
                        break;
                    }
                }
                if (match)
                {
                    listView1.ContextMenu.Show(listView1, new Point(e.X, e.Y));
                }
                else
                {
                    //Show listViews context menu
                }

            }*/
        }
    }
}
