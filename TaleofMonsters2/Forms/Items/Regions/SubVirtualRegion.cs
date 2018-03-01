using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class SubVirtualRegion
    {
        public int Id { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public object Parm { get; set; }//可选信息

        public bool Visible { get; set; }
        public bool Enabled { get; set; }

        protected bool isIn;
        protected bool isMouseDown;
        protected List<IRegionDecorator> decorators;
        protected RegionState state;

        protected VirtualRegion parent;

        private int keyId;

        public SubVirtualRegion(int id, int x, int y, int width, int height)
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            decorators = new List<IRegionDecorator>();
            Visible = true;
            Enabled = true;
        }

        public void SetParent(VirtualRegion r)
        {
            parent = r;
        }

        public void AddDecorator(IRegionDecorator decorator)
        {
            decorators.Add(decorator);
        }

        public virtual void Draw(Graphics g)
        {
            foreach (var decorator in decorators)
                decorator.Draw(g, X, Y, Width, Height);
        }

        public virtual void SetKeyValue(int value)
        {
            keyId = value;
        }

        public virtual int GetKeyValue()
        {
            return keyId;
        }

        public void SetDecorator(int idx, object value)//设置子装饰器状态
        {
            if (decorators.Count > idx)
            {
                if (value != null)
                    decorators[idx].SetState(value);
                else
                    decorators.RemoveAt(idx);
            }
            else
            {
                if (value != null)
                    AddDecorator((IRegionDecorator)value);
            }
        }

        public void SetState(RegionState newState)
        {
            state = newState;
        }

        public virtual void Enter()
        {
            isIn = true;
        }

        public virtual void Left()
        {
            isIn = false;
        }

        public virtual void MouseUp()
        {
            isMouseDown = false;
        }

        public virtual void MouseDown()
        {
            isMouseDown = true;
        }

        public virtual void ShowTip(ImageToolTip tooltip, Control form, int x, int y)
        {
            
        }
    }

    public enum RegionState
    {
        Free = 0,
        Rectangled = 1,
        Blacken=2,
    }
}
