using System;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Quests.SceneQuests
{
    public class SceneQuestAnswer : SceneQuestBlock
    {
        public SceneQuestAnswer(string s, int depth, int line)
            : base(s, depth, line)
        {
            CheckScript();
        }

        private void CheckScript()
        {
            if (Script[0] == '|')
            {
                string[] infos = Script.Split('|');
                Script = infos[infos.Length - 1];
                string[] parms = infos[1].Split('-');

                if (parms[0] == "flagno")
                {
                    Disabled = UserProfile.InfoRecord.CheckFlag((uint)((MemPlayerFlagTypes)Enum.Parse(typeof(MemPlayerFlagTypes), parms[1])));
                }
                else if (parms[0] == "flag")
                {
                    Disabled = !UserProfile.InfoRecord.CheckFlag((uint)((MemPlayerFlagTypes)Enum.Parse(typeof(MemPlayerFlagTypes), parms[1])));
                }
            }
        }
    }
}