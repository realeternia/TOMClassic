using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NarlonLib.Drawing;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Tools;

namespace TaleofMonsters.DataType.Effects
{
    internal static class EffectBook
    {
        private static Dictionary<string, Effect> effectType = new Dictionary<string, Effect>();

        public static int Count
        {
            get { return effectType.Count; }
        }

        public static Effect GetEffect(string name)
        {
            if (!effectType.ContainsKey(name))
            {
                effectType.Add(name, GetEffectFromFile("Effect", string.Format("{0}.eff", name)));
            }
            return effectType[name];
        }

        private static Effect GetEffectFromFile(string path, string name)
        {
            StreamReader sr = new StreamReader(DataLoader.Read(path, name));
            Effect effect = new Effect(name);
            var datas = sr.ReadLine().Split('\t');
            effect.SoundName = datas[0];
            if (datas.Length > 1)
                effect.SpeedDown = int.Parse(datas[1]);
            int frameCount = int.Parse(sr.ReadLine());
            effect.Frames = new EffectFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                int frameUnitCount = int.Parse(sr.ReadLine());
                EffectFrame frame = new EffectFrame();
                frame.Units = new EffectFrame.EffectFrameUnit[frameUnitCount];
                for (int j = 0; j < frameUnitCount; j++)
                {
                    string read = sr.ReadLine();
                    string[] arrays = read.Split('\t');
                    var fu = new EffectFrame.EffectFrameUnit();
                    fu.Frameid = int.Parse(arrays[0]);
                    fu.X = int.Parse(arrays[1]);
                    fu.Y = int.Parse(arrays[2]);
                    fu.Width = int.Parse(arrays[3]);
                    fu.Height = int.Parse(arrays[4]);
                    if (arrays.Length >= 6)
                    {
                        fu.Effect = int.Parse(arrays[5]);
                    }
                    frame.Units[j] = fu;
                }
                effect.Frames[i] = frame;
            }
			sr.Close();
            return effect;
        }

        public static Image GetEffectImage(string name, int type, bool flip)
        {
            string fname = string.Format("Effect/{0}t{1}{2}", name, type, flip ? "f" : "");
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("Effect", string.Format("{0}.PNG", name));
                switch (type)
                {
                    case 1: image.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                    case 2: image.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                    case 3: image.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                    case 4: image.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
                    case 5: image.RotateFlip(RotateFlipType.Rotate90FlipX); break;
                    case 6: image.RotateFlip(RotateFlipType.Rotate180FlipX); break;
                    case 7: image.RotateFlip(RotateFlipType.Rotate270FlipX); break;
                    default: ImagePixelTool.Effect((Bitmap)image, (ImagePixelTool.ImagePixelEffects)(type / 10), type % 10); break;
                }
                if (flip)
                {
                    image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }
    }
}
