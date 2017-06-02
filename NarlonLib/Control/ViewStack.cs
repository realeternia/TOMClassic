﻿using System;
using System.Windows.Forms;

namespace NarlonLib.Control
{
    public class ViewStack : TabControl
    {
        protected override void WndProc(ref Message m)
        {
            // Hide tabs by trapping the TCM_ADJUSTRECT message
            if (m.Msg == 0x1328 && !DesignMode) m.Result = (IntPtr)1;
            else base.WndProc(ref m);
        }

        protected override void OnKeyDown(KeyEventArgs ke)
        {
            if (ke.KeyCode == Keys.Left || ke.KeyCode == Keys.Right)
            {
                ke.Handled = true;
            }
            base.OnKeyDown(ke);
        }
    }
}
