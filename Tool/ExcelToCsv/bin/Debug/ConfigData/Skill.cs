namespace ConfigDatas
{
	public class SkillConfig
	{
		public int Id;
		public string Name;
		public string Type;
		public SkillInitialEffectDelegate OnAdd;
		public SkillInitialEffectDelegate OnRemove;
		public SkillBurstCheckDelegate CanBurst;
		public SkillHitEffectDelegate CheckHit;
		public SkillDamageEffectDelegate CheckDamage;
		public SkillAfterHitEffectDelegate AfterHit;
		public SkillTimelyEffectDelegate OnRoundUpdate;
		public SkillTimelyEffectDelegate CheckSpecial;
		public int Active;
		public int LockSpell;
		public bool IsRandom;
		public FormatStringDelegate GetDescript;
		public int DescriptBuffId;
		public string Effect;
		public string Cover;
		public int Mark;
		public string Icon;
		public SkillConfig(){}
		public SkillConfig(int Id,string Name,string Type,SkillInitialEffectDelegate OnAdd,SkillInitialEffectDelegate OnRemove,SkillBurstCheckDelegate CanBurst,SkillHitEffectDelegate CheckHit,SkillDamageEffectDelegate CheckDamage,SkillAfterHitEffectDelegate AfterHit,SkillTimelyEffectDelegate OnRoundUpdate,SkillTimelyEffectDelegate CheckSpecial,int Active,int LockSpell,bool IsRandom,FormatStringDelegate GetDescript,int DescriptBuffId,string Effect,string Cover,int Mark,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Type= Type;
			this.OnAdd= OnAdd;
			this.OnRemove= OnRemove;
			this.CanBurst= CanBurst;
			this.CheckHit= CheckHit;
			this.CheckDamage= CheckDamage;
			this.AfterHit= AfterHit;
			this.OnRoundUpdate= OnRoundUpdate;
			this.CheckSpecial= CheckSpecial;
			this.Active= Active;
			this.LockSpell= LockSpell;
			this.IsRandom= IsRandom;
			this.GetDescript= GetDescript;
			this.DescriptBuffId= DescriptBuffId;
			this.Effect= Effect;
			this.Cover= Cover;
			this.Mark= Mark;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
