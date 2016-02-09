using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;

namespace TaleofMonsters.Controler.Battle.Data.MemMissile
{
    internal class MissileBook
    {
        static Dictionary<int, Image> effectType = new Dictionary<int, Image>();
        static Dictionary<string, MissileConfig> cachedConfigDict;

        static public Image GetImage(string name)
        {
            var config = GetConfig(name);

            if (!effectType.ContainsKey(config.Image))
            {
                var img = PicLoader.Read("Missile", string.Format("{0}.PNG", name));
                if (img == null)
                {
                    img = PicLoader.Read("Missile", "0.PNG");
                }
                effectType.Add(config.Image, img);
            }
            return effectType[config.Image];
        }

        static public MissileConfig GetConfig(string name)
        {
            if (cachedConfigDict == null)
            {
                cachedConfigDict = new Dictionary<string, MissileConfig>();
                foreach (var missile in ConfigData.MissileDict.Values)
                {
                    cachedConfigDict[missile.TypeName] = missile;
                }
            }

            MissileConfig configData;
            if (cachedConfigDict.TryGetValue(name, out configData))
            {
                return configData;
            }
            return cachedConfigDict["null"];
        }
    }
}
