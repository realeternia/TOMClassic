using System.Windows.Forms;

namespace EffectPlayer
{
    public class DoubleBuffedPanel : Panel
    {
        public DoubleBuffedPanel()
        {
            this.DoubleBuffered = true;

            //// or

            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.OptimizedDoublebuff, true);
            //UpdateStyles();
        }
    }
}
