using System.Collections;
using System.Drawing;
using NarlonLib.Core;
using TaleofMonsters.Rpc;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class VirtualRegionMoveMediator
    {
        private VirtualRegion vRegion;

        public VirtualRegionMoveMediator(VirtualRegion region)
        {
            vRegion = region;
        }

        public void FireShake(int rid)
        {
            TalePlayer.Start(Shake(rid));
        }

        private IEnumerator Shake(int rid)
        {
            var subRegion = vRegion.GetRegion(rid);
            vRegion.SetRegionPosition(rid, new Point(subRegion.X + 10, subRegion.Y - 10));
            vRegion.Invalidate(new Rectangle(subRegion.X - 10, subRegion.Y - 10, subRegion.Width + 20, subRegion.Height + 20));
            yield return new NLWaitForSeconds(0.25f);
            subRegion = vRegion.GetRegion(rid);
            vRegion.SetRegionPosition(rid, new Point(subRegion.X - 10, subRegion.Y + 10));
            vRegion.Invalidate(new Rectangle(subRegion.X - 10, subRegion.Y - 10, subRegion.Width + 20, subRegion.Height + 20));
        }
    }
}