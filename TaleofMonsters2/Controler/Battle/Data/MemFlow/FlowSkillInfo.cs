using System.Drawing;
using TaleofMonsters.DataType.Skills;

namespace TaleofMonsters.Controler.Battle.Data.MemFlow
{
    internal class FlowSkillInfo : FlowWord
    {
        private int skillId;
        public override bool NoOverlap { get { return true; } }

        public FlowSkillInfo(int sId, Point point, int size, string color, int offX, int offY, string addon) 
            : base("", point, size, color, offX, offY, 0, 1, 30)
        {
            skillId = sId;
            string skillName = ConfigDatas.ConfigData.SkillDict[skillId].Name;
            if (addon == "")
            {
                word = skillName;
            }
            else
            {
                word = string.Format("{0}({1})", skillName, addon);
            }
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(SkillBook.GetSkillImage(skillId, 24, 24), Position.X, Position.Y, 24, 24);

            g.DrawString(word, font, Brushes.Black, Position.X + 27, Position.Y + 1);
            Brush brush = new SolidBrush(color);
            g.DrawString(word, font, brush, Position.X + 25, Position.Y);
            brush.Dispose();
        } 
    }
}
