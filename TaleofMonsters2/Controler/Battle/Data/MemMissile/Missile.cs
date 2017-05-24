using System.Drawing;
using ConfigDatas;
using NarlonLib.Core;
using NarlonLib.Drawing;
using TaleofMonsters.Controler.Battle.Tool;

namespace TaleofMonsters.Controler.Battle.Data.MemMissile
{
    /// <summary>
    /// 锁定或者非锁定的投射物
    /// </summary>
    internal class Missile
    {
        private Image effectImg;//旋转后的图片，每次都从初始图片开始旋转

        private MissileConfig config;
        private NLPointF position;//missile的坐标
        private int angle=-999;//missile的角度
        private int frameOffset;//第几针，影响动画播放

        public bool IsFinished { get; set; }
        private BasicMissileControler controler;

        public Missile(string effName, int x, int y, BasicMissileControler cont)
        {
            position = new NLPointF(x, y);
            controler = cont;
            config = MissileBook.GetConfig(effName);
            controler.SetConfig(config);
        }

        public void Next()
        {
            frameOffset++;

            if (!controler.CheckFly(ref position, ref angle))
            {
                IsFinished = true;
                return;
            }

            GenerateImg();
        }

        private void GenerateImg()
        {
            angle = (angle + 360) % 360;
            var imgId = config.Image + (frameOffset/config.FrameTime)%config.FrameCount;
            Image img = null;
            if (angle > 90 && angle < 270)
            {
                img = MissileBook.GetImage(imgId, true);
            }
            else
            {
                img = MissileBook.GetImage(imgId, false);
            }

            effectImg = DrawTool.Rotate(img, angle);
        }
        
        public void Draw(Graphics g)
        {
            if (effectImg != null)
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                int ewid = effectImg.Width * size / 100;
                int eheg = effectImg.Height * size / 100;

                var x = position.X - (float)ewid / 2 + (float)size / 2;//+size/2是为了平移到中心位置
                var y = position.Y - (float)eheg / 2 + (float)size / 2;

                g.DrawImage(effectImg, x, y, ewid, eheg);
            }
        }

    }
}
