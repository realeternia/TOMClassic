using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class VirtualRegion : IDisposable
    {
        public delegate void VRegionLeftEventHandler();
        public delegate void VRegionClickEventHandler(int id, int x, int y, MouseButtons button);
        public delegate void VRegionEnteredEventHandler(int id, int x, int y, int key);
        public delegate void VRegionDrawEventHandler(int id, int x, int y, int key, Graphics g);

        public event VRegionClickEventHandler RegionClicked;
        public event VRegionEnteredEventHandler RegionEntered;
        public event VRegionLeftEventHandler RegionLeft;

        public event VRegionDrawEventHandler CellDrawBefore;
        public event VRegionDrawEventHandler CellDrawAfter;

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
            parent.MouseMove += parent_MouseMove;
            parent.MouseClick += parent_MouseClick;
            parent.MouseUp += parent_MouseUp;
            parent.MouseDown += parent_MouseDown;
            parent.MouseLeave += parent_MouseLeave;
            Visible = true;
        }

        public void AddRegion(SubVirtualRegion region)
        {
            region.SetParent(this);
            subRegions.Add(region.Id, region);
        }

        public void ClearRegion()
        {
            subRegions.Clear();
        }

        public void SetRegionKey(int id, int value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
                region.SetKeyValue(value);
        }

        public void SetRegionType(int id, PictureRegionCellType value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
                (region as PictureRegion).SetType(value);
        }

        public void SetRegionState(int id, RegionState value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
                region.SetState(value);
        }

        public void SetRegionDecorator(int id, int did, object value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
                region.SetDecorator(did, value);
        }

        public void SetRegionVisible(int id, bool visible)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
                region.Visible = visible;
        }

        public void SetRegionEnable(int id, bool value)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
                region.Enabled = value;
        }

        public Point GetRegionPosition(int id)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
                return new Point(region.X + region.Width + 1, region.Y);
            return new Point(0,0);
        }
        public void SetRegionPosition(int id, Point pos)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
            {
                region.X = pos.X;
                region.Y = pos.Y;
            }
        }

        public SubVirtualRegion GetRegion(int id)
        {
            SubVirtualRegion region;
            if (subRegions.TryGetValue(id, out region))
                return region;
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
                            selectRegion.MouseUp();
                        }
                        selectRegion = subRegion;
                        selectRegion.Enter();
                        if (RegionEntered!=null)
                            RegionEntered(selectRegion.Id, selectRegion.X + selectRegion.Width + 1, selectRegion.Y, selectRegion.GetKeyValue());
                    }
                    return;
                }
            }
            if (selectRegion != null)
            {
                selectRegion.Left();
                selectRegion.MouseUp();
                selectRegion = null;
                if (RegionLeft!=null)
                    RegionLeft();
            }
        }

        private void CheckMouseClick(MouseButtons button)
        {
            if (RegionClicked != null && selectRegion != null && selectRegion.Id > 0)
                RegionClicked(selectRegion.Id, lastMouseX, lastMouseY, button);
        }

        public void Draw(Graphics g)
        {
            if (!Visible)
                return;

            foreach (var subVirtualRegion in subRegions.Values)
            {
                if (subVirtualRegion.Visible)
                {
                    if (CellDrawBefore != null)
                        CellDrawBefore(subVirtualRegion.Id, subVirtualRegion.X, subVirtualRegion.Y, subVirtualRegion.GetKeyValue(), g);

                    subVirtualRegion.Draw(g);

                    if (CellDrawAfter != null)
                        CellDrawAfter(subVirtualRegion.Id, subVirtualRegion.X, subVirtualRegion.Y, subVirtualRegion.GetKeyValue(), g);
                }
            }
        }

        public void Invalidate(Rectangle region)
        {
            if (parent != null)
                parent.Invalidate(region);
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

        private void parent_MouseDown(object sender, MouseEventArgs e)
        {
            if(selectRegion != null && selectRegion.Id > 0)
                selectRegion.MouseDown();
        }

        private void parent_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectRegion != null && selectRegion.Id > 0)
                selectRegion.MouseUp();
        }

        public void Dispose()
        {
            parent.MouseMove -= parent_MouseMove;
            parent.MouseClick -= parent_MouseClick;
            parent.MouseLeave -= parent_MouseLeave;
            parent.MouseDown -= parent_MouseDown;
            parent.MouseUp -= parent_MouseUp;
            parent = null;
        }
    }
}
