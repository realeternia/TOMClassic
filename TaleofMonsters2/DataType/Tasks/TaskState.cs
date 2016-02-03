using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Tasks
{
    public class TaskState
    {
         [FieldIndex(Index = 1)]
        public int Tid;
         [FieldIndex(Index = 2)]
        public int State; //0,未接受,1,接受,2,完成,3,领取奖励结束,4,失败
         [FieldIndex(Index =3)]
        public int Addon;

        public TaskState()
        {
        }

        public TaskState(int tid, int state, int addon)
        {
            this.Tid = tid;
            this.State = state;
            this.Addon = addon;
        }
    }
}
