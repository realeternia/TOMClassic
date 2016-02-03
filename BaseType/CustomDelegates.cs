namespace ConfigDatas
{
    public delegate void BuffEffectDelegate(IMonster owner);

    public delegate void SkillInitialEffectDelegate(IMonster src, int level);

    public delegate bool SkillBurstCheckDelegate(IMonster src, IMonster dest, bool isActive);
    public delegate void SkillHitEffectDelegate(IMonster src, IMonster dest, ref int hit, int level);
    public delegate void SkillDamageEffectDelegate(IMonster src, IMonster dest, bool isActive, HitDamage damage, ref int minDamage, ref bool deathHit, int level);
    public delegate void SkillAfterHitEffectDelegate(IMonster src, IMonster dest, HitDamage damage, bool deadHit, int level);
    public delegate void SkillTimelyEffectDelegate(IMonster src, int level, ref bool skipRound);

    public delegate void SpellEffectDelegate(ISpell spell,IMap map, IPlayer player, IPlayer rival, IMonster target, System.Drawing.Point mouse, int level);

    public delegate bool SpellTrapAddCardDelegate(IPlayer player, IPlayer rival, ITrap trap, int cardId, int cardRealId, int cardType);
    public delegate bool SpellTrapSummonDelegate(IPlayer player, IPlayer rival, ITrap trap, IMonster mon, int level);

    public delegate string FormatStringDelegate(int level);
}