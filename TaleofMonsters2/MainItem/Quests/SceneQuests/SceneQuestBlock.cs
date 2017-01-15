using System.Collections.Generic;

namespace TaleofMonsters.MainItem.Quests.SceneQuests
{
    public class SceneQuestBlock
    {
        protected int eventId;

        public List<SceneQuestBlock> Children { get; private set; }

        public int Line { get; private set; } //行号

        public string Script { get; protected set; } //文本

        public int Depth { get; private set; } //tab数量

        public bool Disabled { get; set; }

        public SceneQuestBlock(int eid, string s, int depth, int line)
        {
            eventId = eid;
            Script = s;
            Depth = depth;
            Line = line;
            Children = new List<SceneQuestBlock>();
        }

        public override string ToString()
        {
            return Script;
        }
    }
}