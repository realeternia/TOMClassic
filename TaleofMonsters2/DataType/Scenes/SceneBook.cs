using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;

namespace TaleofMonsters.DataType.Scenes
{
    internal static class SceneBook
    {
        private static Dictionary<string, int> sceneQuestNameDict = null;
        public static int GetSceneQuestByName(string name)
        {
            if (sceneQuestNameDict == null)
            {
                sceneQuestNameDict = new Dictionary<string, int>();
                foreach (SceneQuestConfig questConfig in ConfigData.SceneQuestDict.Values)
                {
                    sceneQuestNameDict.Add(questConfig.Ename, questConfig.Id);
                }
            }
            int questId;
            if (!sceneQuestNameDict.TryGetValue(name, out questId))
            {
                throw new KeyNotFoundException("scene quest name not found " + name);
            }
            return questId;
        }

        public static Image GetSceneNpcImage(int id)
        {
            string fname = string.Format("NPC/{0}.PNG", ConfigData.NpcDict[id].Figue);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("NPC", string.Format("{0}.PNG", ConfigData.NpcDict[id].Figue));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static Image GetSceneQuestImage(int id)
        {
            string fname = string.Format("SceneQuest/{0}.PNG", ConfigData.SceneQuestDict[id].Figue);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("SceneQuest", string.Format("{0}.JPG", ConfigData.SceneQuestDict[id].Figue));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static string[] GetNPCIconsOnMap(int id)
        {
            List<string> strs = new List<string>();
            foreach (NpcConfig npcConfig in ConfigData.NpcDict.Values)
            {
                if (npcConfig.MapId == id && npcConfig.Icon != "")
                    strs.Add(npcConfig.Icon);
            }
            return strs.ToArray();
        }
    }
}
