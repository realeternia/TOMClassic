using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Forms
{
    internal sealed partial class BlackWallForm : BasePanel
    {
        private static BlackWallForm instance;
        public static BlackWallForm Instance
        {
            get
            {
                if (instance == null) 
                {
                    instance = new BlackWallForm();
                }
                return instance;
            }
        }

        public BlackWallForm()
        {
            InitializeComponent();
        }

        public override void Init(int width, int height)
        {
            Width = width;
            Height = height;
            base.Init(width, height);
            Location = new Point(0,0); //做一个特殊处理，不自动偏移
            SystemMenuManager.IsHotkeyEnabled = false;
        }

        public override void OnRemove()
        {
            base.OnRemove();

            SystemMenuManager.IsHotkeyEnabled = true;
        }

        private void BlackWallForm_Paint(object sender, PaintEventArgs e)
        {
            Color col = Color.FromArgb(180, Color.Black);
            SolidBrush brush = new SolidBrush(col);
            e.Graphics.FillRectangle(brush, 0, 0, formWidth, formHeight);
            brush.Dispose();
        }
    }
}