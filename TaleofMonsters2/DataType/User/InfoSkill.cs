using System.Collections.Generic;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User
{
    public class InfoSkill
    {
        [FieldIndex(Index = 3)]
         public Dictionary<int, int> SkillAttrs;

        public InfoSkill()
        {
            SkillAttrs = new Dictionary<int, int>();
        }

        public int GetSkillAttrLevel(int sid)
        {
            if (SkillAttrs.ContainsKey(sid))
            {
                return SkillAttrs[sid];
            }
            return 0;
        }

        public void AddSkillAttrLevel(int sid)
        {
            if (SkillAttrs.ContainsKey(sid))
            {
                SkillAttrs[sid]++;
            }
            else
            {
                SkillAttrs.Add(sid, 1);
            }
        }
    }
}
