using NarlonLib.Control;

namespace TaleofMonsters.MainItem
{
    class SystemToolTip
    {
        private static ImageToolTip tooltip = new ImageToolTip();

        public static ImageToolTip Instance
        {
            get { return tooltip; }
        }
    }
}
