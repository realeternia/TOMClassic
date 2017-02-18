using System;
using System.Collections.Generic;
using System.Text;

namespace EffectPlayer
{
    public class Frame
    {
        FrameUnit[] units;

        public FrameUnit[] Units
        {
            get { return units; }
            set { units = value; }
        }
        public Frame() { }

    }
}
