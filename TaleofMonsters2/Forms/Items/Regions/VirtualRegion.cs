using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class VirtualRegion
    {
        public delegate void VRegionLeftEventHandler();
        public delegate void VRegionClickEventHandler(int info, int x, int y, MouseButtons button);
        public delegate void VRegionEnteredEventHandler(int info, int x, int y, int key);

        public event VRegionClickEventHandler RegionClicked;
        public event VRegionEnteredEventHandler RegionEntered;
        public event VRegionLeftEventHandler RegionLeft;

        private SubVirtualRegion selectRegion;
        private readonly Dictionary<int, SubVirtualRegion> subRegions;
        private Control parent;

        private int lastMouseX;
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
            subRegions.Add(region.id, region);
        }

        public void SetRegionInfo(int id, int value)
        {
            if (subRegions.ContainsKey(id))
            {
                subRegions[id].SetKeyValue(value);
            }
        }

        public void SetRegionType(int id, VirtualRegionCellType value)
        {
            if (subRegions.ContainsKey(id))
            {
                subRegions[id].SetType(value);
            }
        }

        public void SetRegionState(int id, RegionState value)
        {
            if (subRegions.ContainsKey(id))
            {
                subRegions[id].SetState(value);
            }
        }

        public void SetRegionDecorator(int id, int did, object value)
        {
            if (subRegions.ContainsKey(id))
            {
                subRegions[id].SetDecorator(did, value);
            }
        }

        public void SetRegionParm(int id, object value)
        {
            if (subRegions.ContainsKey(id))
            {
                subRegions[id].SetParm(value);
            }
        }

        public Point GetRegionPosition(int id)
        {
            if (subRegions.ContainsKey(id))
            {
                return new Point(subRegions[id].x+ selectRegion.width + 1, subRegions[id].y);
            }
            return new Point(0,0);
        }

        private void CheckMouseMove(int mouseX, int mouseY)
        {
            lastMouseX = mouseX;
            lastMouseY = mouseY;
            foreach (SubVirtualRegion subRegion in subRegions.Values)
            {
                if (mouseX > subRegion.x && mouseX < subRegion.x + subRegion.width && mouseY > subRegion.y && mouseY < subRegion.y + subRegion.height)
                {
                    if (selectRegion == null || subRegion.id != selectRegion.id)
                    {
                        if (selectRegion!=null)
                        {
                            selectRegion.Left();
                        }
                        selectRegion = subRegion;
                        selectRegion.Enter();
                        if (RegionEntered!=null)
                        {
                            RegionEntered(selectRegion.info, selectRegion.x + selectRegion.width + 1, selectRegion.y, selectRegion.GetKeyValue());
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
            if (RegionClicked != null && selectRegion != null && selectRegion.id > 0)
            {
                RegionClicked(selectRegion.info, lastMouseX, lastMouseY, button);
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
