using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class VirtualRegion
    {
        public delegate void VRegionLeftEventHandler();
        public delegate void VRegionClickEventHandler(int id, int x, int y, MouseButtons button);
        public delegate void VRegionEnteredEventHandler(int id, int x, int y, int key);

        public event VRegionClickEventHandler RegionClicked;
        public event VRegionEnteredEventHandler RegionEntered;
        public event VRegionLeftEventHandler RegionLeft;

        private SubVirtualRegion selectRegion;
        private readonly Dictionary<int, SubVirtualRegion> subRegions;
        private Control parent;

        private int lastMouseX; //移动时记录下位置，供点击时使用
        private int lastMouseY;

        public bool Visible { get; set; }

        public VirtualRegion(Control parent)
        {
            subRegions = new Dictionary<int, SubVirtualRegion>();
            this.parent = parent;
            parent.MouseMove+=new MouseEventHandler(parent_MouseMove);
            parent.MouseClick+=new MouseEventHandler(parent_MouseClick);
            parent.MouseLeave+=new System.EventHandler(parent_MouseLeave);
            Visible = true;
        }

        public void AddRegion(SubVirtualRegion region)
        {
            region.Parent = parent;
            subRegions.Add(region.Id, region);
        }

        public void SetRegionKey(int id, int value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
            {
                region.SetKeyValue(value);
            }
        }

        public void SetRegionType(int id, PictureRegionCellType value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
            {
                (region as PictureRegion).SetType(value);
            }
        }

        public void SetRegionState(int id, RegionState value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
            {
                region.SetState(value);
            }
        }

        public void SetRegionDecorator(int id, int did, object value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
            {
                region.SetDecorator(did, value);
            }
        }

        public void SetRegionParm(int id, object value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
            {
                region.Parm = value;
            }
        }

        public Point GetRegionPosition(int id)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
            {
                return new Point(region.X + selectRegion.Width + 1, region.Y);
            }
            return new Point(0,0);
        }

        public SubVirtualRegion GetRegion(int id)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
            {
                return region;
            }
            return null;
        }

        private void CheckMouseMove(int mouseX, int mouseY)
        {
            lastMouseX = mouseX;
            lastMouseY = mouseY;
            foreach (SubVirtualRegion subRegion in subRegions.Values)
            {
                if (mouseX > subRegion.X && mouseX < subRegion.X + subRegion.Width && mouseY > subRegion.Y && mouseY < subRegion.Y + subRegion.Height)
                {
                    if (selectRegion == null || subRegion.Id != selectRegion.Id)
                    {
                        if (selectRegion!=null)
                        {
                            selectRegion.Left();
                        }
                        selectRegion = subRegion;
                        selectRegion.Enter();
                        if (RegionEntered!=null)
                        {
                            RegionEntered(selectRegion.Id, selectRegion.X + selectRegion.Width + 1, selectRegion.Y, selectRegion.GetKeyValue());
                        }
                    }
                    return;
                }
            }
            if (selectRegion != null)
            {
                selectRegion.Left();
                selectRegion = null;
                if (RegionLeft!=null)
                {
                    RegionLeft();
                }
            }
        }

        private void CheckMouseClick(MouseButtons button)
        {
            if (RegionClicked != null && selectRegion != null && selectRegion.Id > 0)
            {
                RegionClicked(selectRegion.Id, lastMouseX, lastMouseY, button);
            }
        }

        public void Draw(Graphics g)
        {
            if (!Visible)
            {
                return;
            }
            foreach (SubVirtualRegion subVirtualRegion in subRegions.Values)
            {
                subVirtualRegion.Draw(g); 
            }
        }

        private void parent_MouseMove(object sender, MouseEventArgs e)
        {
            CheckMouseMove(e.X, e.Y);
        }

        private void parent_MouseLeave(object sender, System.EventArgs e)
        {
            CheckMouseMove(-1, -1);
        }

        private void parent_MouseClick(object sender, MouseEventArgs e)
        {
            CheckMouseClick(e.Button);
        }
    }
}
