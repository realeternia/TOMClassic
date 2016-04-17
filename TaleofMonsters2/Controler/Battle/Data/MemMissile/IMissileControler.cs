using System;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Core;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemMissile
{
    internal abstract class BasicMissileControler
    {
        protected MissileConfig config;

        public void SetConfig(MissileConfig configData)
        {
            config = configData;
        }

        public virtual bool CheckFly(ref NLPointF point, ref int angle)
        {
            return false;
        }

        protected bool FlyProc(Point targetPosition, ref NLPointF position, ref int angle)
        {
            if (MathTool.GetDistance(targetPosition, position.ToPoint()) < 10)//todo 10是一个估算值
            {
                return false;
            }

            var posDiff = new NLPointF(targetPosition.X - position.X, targetPosition.Y - position.Y);
            posDiff = posDiff.Normalize() * (float)config.Speed;
            position = position + posDiff;
            var angleD = Math.Atan(-posDiff.Y / posDiff.X) / Math.PI * 180;
            angle = posDiff.X >= 0 ? (int)angleD : (int)angleD + 180;

            if (MathTool.GetDistance(targetPosition, position.ToPoint()) < 10)//todo 10是一个估算值
            {
                return false;
            }

            return true;
        }
    }

    internal class TraceMissileControler : BasicMissileControler
    {
        protected LiveMonster parent;//母体
        private LiveMonster target;//目标

        public TraceMissileControler(LiveMonster self, LiveMonster mon)
        {
            parent = self;
            target = mon;
        }

        public override bool CheckFly(ref NLPointF position, ref int angle)
        {
            if (target == null || !target.IsAlive || parent == null || !parent.IsAlive)
            {
                return false;
            }

            if (!FlyProc(target.Position,ref position, ref angle))
            {
                parent.HitTarget(target);
                BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(parent.Arrow), target, false));
                return false;
            }
            return true;
        }
    }

    internal class LandMissileControler : BasicMissileControler
    {
        protected LiveMonster parent;//母体
        private Point targetPos;

        public LandMissileControler(LiveMonster self, Point pos)
        {
            parent = self;
            targetPos = pos;
        }

        public override bool CheckFly(ref NLPointF position, ref int angle)
        {
            if (parent == null || !parent.IsAlive)
            {
                return false;
            }

            if (!FlyProc(targetPos, ref position, ref angle))
            {
                var mon = BattleLocationManager.GetPlaceMonster(targetPos.X, targetPos.Y);
                if (mon != null)
                {
                    parent.HitTarget(mon);
                    BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(parent.Arrow), mon, false));
                }
                return false;
            }
            return true;
        }
    }

    internal class SpellTraceMissileControler : BasicMissileControler
    {
        private ISpell spell;
        private LiveMonster target;//目标

        public SpellTraceMissileControler(LiveMonster mon, ISpell spl)
        {
            spell = spl;
            target = mon;
        }

        public override bool CheckFly(ref NLPointF position, ref int angle)
        {
            if (target == null || !target.IsAlive)
            {
                return false;
            }

            if (!FlyProc(target.Position, ref position, ref angle))
            {
                target.OnMagicDamage(spell.Damage, ConfigData.GetSpellConfig(spell.Id).Attr);
                return false;
            }
            return true;
        }
    }
}
