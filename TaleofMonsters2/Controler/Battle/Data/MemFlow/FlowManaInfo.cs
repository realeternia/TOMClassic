using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    class FlowManaInfo : FlowWord
    {
        private List<PlayerManaTypes> manaList;
        internal FlowManaInfo(List<PlayerManaTypes> mana, Point point, int offX, int offY)
            : base("", point, -2, "BlueViolet", offX, offY, 1, 2, 20)
        {
            manaList = mana;
        }

        internal override void Draw(Graphics g)
        {
            for (int i = 0; i < manaList.Count; i++)
            {
                var img = HSIcons.GetIconsByEName("mix" + (int)manaList[i]);
                g.DrawImage(img, position.X+20*i, position.Y, 20, 20);
            }
         
        } 
    }
}
