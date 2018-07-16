namespace ConfigDatas
{
    public delegate void BuffEffectDelegate(IBuff buf, IMonster owner);

    public delegate void SkillInitialEffectDelegate(ISkill skl, IMonster src);

    public delegate bool SkillBurstCheckDelegate(IMonster src, IMonster dest, bool isMelee);
    public delegate void SkillHitEffectDelegate(ISkill skl, IMonster src, IMonster dest, ref int hit);
    public delegate void SkillDamageEffectDelegate(ISkill skl, IMonster src, IMonster dest, bool isActive, HitDamage damage, ref bool noDef);
    public delegate void SkillAfterHitEffectDelegate(ISkill skl, IMonster src, IMonster dest, HitDamage damage);
    public delegate void SkillTimelyEffectDelegate(ISkill skl, IMonster src);
    public delegate void SkillUseCardHandleDelegate(ISkill skl, IMonster src, IPlayer p, int cardType, int cardLevel, ref bool success);

    public delegate bool SpellCheckDelegate(ISpell spell, IPlayer player, IMonster target);
    public delegate void SpellEffectDelegate(ISpell spell, IMap map, IPlayer player, IPlayer rival, IMonster target, System.Drawing.Point mouse);

    public delegate void RelicAddCardDelegate(IPlayer player, IPlayer rival, IRelic relic, int cardId, int cardType, ref bool result);
    public delegate void RelicSummonDelegate(IPlayer player, IPlayer rival, IRelic relic, IMonster mon, int level, ref bool result);

    public delegate bool EquipMonsterPickDelegate(IMonster src);

    public delegate string FormatStringDelegate(int level);
}