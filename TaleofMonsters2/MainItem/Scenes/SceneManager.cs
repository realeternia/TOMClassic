using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.NPCs;

namespace TaleofMonsters.MainItem.Scenes
{
    internal static class SceneManager
    { 
        static public Image GetPreview(int id)
        {
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(sceneConfig.Name, "Lime", 20);
            tipData.AddTextNewLine(string.Format("需要等级: {0}", sceneConfig.Level), "White");

            string[] icons = NPCBook.GetNPCIconsOnMap(id);
            if (icons.Length > 0)
            {
                tipData.AddTextNewLine("设施", "Green");
                foreach (string icon in icons)
                {
                    tipData.AddImage(HSIcons.GetIconsByEName(icon));
                }
            }

            if (sceneConfig.Func != "")
            {
                tipData.AddTextNewLine("特色", "Pink");
                string[] funcs = sceneConfig.Func.Split(';');
                foreach (string fun in funcs)
                {
                    tipData.AddImage(HSIcons.GetIconsByEName(string.Format("npc{0}", fun.ToLower())));
                }
            }
            return tipData.Image;
        }
    }
}
