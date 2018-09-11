using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaleofMonsters.Forms.CMain.Quests.SceneQuests
{
    internal abstract class SceneQuestBlock
    {
        protected int eventId;

        protected int level;

        public List<SceneQuestBlock> Children { get; private set; }

        public int Line { get; private set; } //行号

        public string Script { get; protected set; } //文本

        public int Depth { get; private set; } //tab数量

        public bool Disabled { get; set; }

        public Rectangle Rect { get; private set; }
        protected Control parent;

        protected SceneQuestBlock(Control p, int eid, int lv, string s, int depth, int line)
        {
            parent = p;
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

        public virtual void SetRect(Rectangle r)
        {
            Rect = r;
        }

        public virtual void SetScript(string s)
        {
            Script = s;
        }

        public virtual void Draw(Graphics g)
        {
        }
    }
}