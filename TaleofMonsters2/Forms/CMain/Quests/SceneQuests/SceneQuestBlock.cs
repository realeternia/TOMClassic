using System.Collections.Generic;
using System.Drawing;

namespace TaleofMonsters.Forms.CMain.Quests.SceneQuests
{
    internal class SceneQuestBlock
    {
        protected int eventId;

        protected int level;

        public List<SceneQuestBlock> Children { get; private set; }

        public int Line { get; private set; } //行号

        public string Prefix { get; set; } //前缀
        public string Script { get; set; } //文本

        public int Depth { get; private set; } //tab数量

        public bool Disabled { get; set; }

        public SceneQuestBlock(int eid, int lv, string s, int depth, int line)
        {
            eventId = eid;
            level = lv;
            Script = s;
            Depth = depth;
            Line = line;
            Children = new List<SceneQuestBlock>();
        }

        public override string ToString()
        {
            return Script;
        }

        public virtual void Draw(Graphics g, int xOff, int yOff, int width)
        {
            Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(Script, font, Brushes.Wheat, xOff + 10, yOff + 2);
            font.Dispose();
        }
    }
}