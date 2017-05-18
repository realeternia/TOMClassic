namespace ConfigDatas
{
    public delegate void BuffEffectDelegate(IMonster owner);

    public delegate void SkillInitialEffectDelegate(ITargetMeasurable sp, IMonster src, int level);

    public delegate bool SkillBurstCheckDelegate(IMonster src, IMonster dest, bool isMelee);
    public delegate void SkillHitEffectDelegate(IMonster src, IMonster dest, ref int hit, int level);
    public delegate void SkillDamageEffectDelegate(IMonster src, IMonster dest, bool isActive, HitDamage damage, ref bool noDef, int level);
    public delegate void SkillAfterHitEffectDelegate(ITargetMeasurable sp, IMonster src, IMonster dest, HitDamage damage, int level);
    public delegate void SkillTimelyEffectDelegate(ITargetMeasurable sp, IMonster src, int level);
    public delegate void SkillUseCardHandleDelegate(IMonster src, IPlayer p, int cardType, int cardLevel);

    public delegate void SpellEffectDelegate(ISpell spell,IMap map, IPlayer player, IPlayer rival, IMonster target, System.Drawing.Point mouse, int level);

    public delegate bool SpellTrapAddCardDelegate(IPlayer player, IPlayer rival, ITrap trap, int cardId, int cardType);
    public delegate bool SpellTrapSummonDelegate(IPlayer player, IPlayer rival, ITrap trap, IMonster mon, int level);

    public delegate string FormatStringDelegate(int level);
}