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

        public static Image GetImage(int id)
        {
            if (!effectType.ContainsKey(id))
            {
                var img = PicLoader.Read("Missile", string.Format("{0}.PNG", id));
                if (img == null)
                {
                    img = PicLoader.Read("Missile", "0.PNG");
                }
                effectType.Add(id, img);
            }
            return effectType[id];
        }

        public static MissileConfig GetConfig(string name)
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
