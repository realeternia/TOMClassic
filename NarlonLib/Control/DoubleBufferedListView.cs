using System.Drawing;
using System.Windows.Forms;

namespace NarlonLib.Control
{
    public class DoubleBufferedListView : ListView
    {
        private ListViewItem itm;

        public DoubleBufferedListView()
        {
            this.DoubleBuffered = true;
            this.SelectedIndexChanged += listViewIds_SelectedIndexChanged;
        }

        private void listViewIds_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (SelectedItems.Count == 0)
            {
                return;
            }

            if (itm != null)
            {
                itm.BackColor = Color.Transparent;
                itm.ForeColor = Color.Black;
            }
            itm = SelectedItems[0];
            itm.BackColor = Color.IndianRed;
            itm.ForeColor = Color.White;
        }
    }
}
