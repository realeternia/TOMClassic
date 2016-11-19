using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.NPCs;
using TaleofMonsters.DataType.Scenes.SceneObjects;
using ConfigDatas;

namespace TaleofMonsters.DataType.Scenes
{
    internal static class SceneBook
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

        static public SceneObject[] GetWarps(int id)
        {
            List<SceneWarp> warpObjs = new List<SceneWarp>();
            foreach (SceneWarpConfig warpConfig in ConfigData.SceneWarpDict.Values)
            {
                if (warpConfig.FromMap!=id)
                    continue;

                SceneWarp warp = new SceneWarp( warpConfig.Id, warpConfig.X, warpConfig.Y, warpConfig.ToMap);
                warp.X = (int)(warp.X * Config.Config.SceneTextureFactorX);
                warp.Y = (int)(warp.Y * Config.Config.SceneTextureFactorX);
                warp.Width = (int)(warp.Width * Config.Config.SceneTextureFactorSise);
                warp.Height = (int)(warp.Height * Config.Config.SceneTextureFactorSise);
                warpObjs.Add(warp);
            }
            return warpObjs.ToArray();
        }
    }
}
