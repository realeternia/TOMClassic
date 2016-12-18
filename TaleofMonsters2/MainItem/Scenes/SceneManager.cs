using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.NPCs;
using TaleofMonsters.MainItem.Scenes.SceneObjects;

namespace TaleofMonsters.MainItem.Scenes
{
    internal static class SceneManager
    {
        public static List<SceneObject> GetSceneObjects(int id, int mapWidth ,int mapHeight)
        {
            List<SceneObject> objects = new List<SceneObject>();
            var filePath = ConfigData.GetSceneConfig(id).TilePath;

            StreamReader sr = new StreamReader(DataLoader.Read("Scene", string.Format("{0}.scn", filePath)));
            int xoff = int.Parse(sr.ReadLine().Split('=')[1])*mapWidth/1422;
            int yoff = int.Parse(sr.ReadLine().Split('=')[1])*mapHeight/855+50;//50为固定偏移
            int wid = int.Parse(sr.ReadLine().Split('=')[1]);
            int height = int.Parse(sr.ReadLine().Split('=')[1]);

            int cellWidth = GameConstants.SceneTileStandardWidth * mapWidth/1422;
            int cellHeight = GameConstants.SceneTileStandardHeight * mapHeight / 855;
            for (int i = 0; i < height; i++)
            {
                string[] data = sr.ReadLine().Split('\t');
                for (int j = 0; j < wid; j++)
                {
                    int val = int.Parse(data[j]);
                    if (val ==0)
                    {
                        continue;
                    }

                    int lineOff = (int)(cellWidth*(height-i-1)* GameConstants.SceneTileGradient);
                    SceneTile so = new SceneTile(val,xoff + j * cellWidth+ lineOff, yoff + i * cellHeight , cellWidth, cellHeight);
                    objects.Add(so);
                }
            }
            sr.Close();
            return objects;
        }

        public static bool CanMove(int id1, int id2)
        {
            int differ = Math.Abs(id1 - id2);
            return differ == 1 || differ == 1000;
        }

        public static Image GetPreview(int id)
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
