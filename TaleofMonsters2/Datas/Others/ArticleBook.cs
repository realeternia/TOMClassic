using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Others
{
    public class ArticleBook
    {
        public static Image GetArticleImage(int id)
        {
            var articleConfig = ConfigData.GetArticleConfig(id);
            string fname = string.Format("Article/{0}.PNG", articleConfig.Icon);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Article", string.Format("{0}.PNG", articleConfig.Icon));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static int GetRandomArticleId(int group)
        {
            var articleList = new List<int>();
            foreach (var articleConfig in ConfigData.ArticleDict.Values)
            {
                if (articleConfig.Group == group)
                    articleList.Add(articleConfig.Id);
            }
            return articleList[MathTool.GetRandom(articleList.Count)];
        }
    }
}