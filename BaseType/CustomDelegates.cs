namespace ConfigDatas
{
    public delegate void BuffEffectDelegate(IBuff buf, IMonster owner);

    public delegate void SkillInitialEffectDelegate(ISkill skl, IMonster src);

    public delegate bool SkillBurstCheckDelegate(IMonster src, IMonster dest, bool isMelee);

    public delegate void SkillEventDelegate(ISkill skl, IPlayer p, IMonster src, IMonster dest, IHitDamage damage, int cardType, int cardLevel, ref bool success);

    public delegate bool SpellCheckDelegate(ISpell spell, IPlayer player, IMonster target);
    public delegate void SpellEffectDelegate(ISpell spell, IMap map, IPlayer player, IPlayer rival, IMonster target, System.Drawing.Point mouse);

    public delegate void RelicEventDelegate(IPlayer player, IRelic relic, int cardId, int cardType, IMonster mon, ref bool result);

    public delegate bool EquipMonsterPickDelegate(IMonster src);

    public delegate string FormatStringDelegate(int level);
}