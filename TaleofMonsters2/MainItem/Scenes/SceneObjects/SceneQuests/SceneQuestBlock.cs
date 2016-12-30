using System.Collections.Generic;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects.SceneQuests
{
    public class SceneQuestBlock
    {
        public List<SceneQuestBlock> Children { get; private set; }

        public int Line { get; private set; } //行号

        public string Script { get; private set; } //文本

        public int Depth { get; private set; } //tab数量
        public SceneQuestBlock(string s, int depth, int line)
        {
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