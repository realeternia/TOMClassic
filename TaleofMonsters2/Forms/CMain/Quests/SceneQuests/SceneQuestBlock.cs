using System.Collections.Generic;
using System.Drawing;

namespace TaleofMonsters.Forms.CMain.Quests.SceneQuests
{
    internal abstract class SceneQuestBlock
    {
        protected int eventId;

        protected int level;

        public List<SceneQuestBlock> Children { get; private set; }

        public int Line { get; private set; } //行号

        public string Prefix { get; set; } //前缀
        public string Script { get; set; } //文本

        public int Depth { get; private set; } //tab数量

        public bool Disabled { get; set; }

        protected SceneQuestBlock(int eid, int lv, string s, int depth, int line)
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
        }
    }
}