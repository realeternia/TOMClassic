using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Tasks;
using TaleofMonsters.Forms;

namespace TaleofMonsters.MainItem.Scenes.SceneObjects
{ 
    internal class SceneNPC : SceneObject
    {
        private List<int> taskAvails;
        private List<int> taskFinishs;

        public SceneNPC(int npcid)
        {
            Id = npcid;

            taskAvails = TaskBook.GetAvailTask(Id);
            taskFinishs = TaskBook.GetFinishingTask(Id);
        }

        public override void Draw(Graphics g, int target)
        {

        }
    }
}

