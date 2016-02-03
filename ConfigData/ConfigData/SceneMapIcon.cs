namespace ConfigDatas
{
	public class SceneMapIconConfig
	{
		public int Id;
		public int Level;
		public int IconX;
		public int IconY;
		public int IconWidth;
		public int IconHeight;
		public string Icon;
		public SceneMapIconConfig(){}
		public SceneMapIconConfig(int Id,int Level,int IconX,int IconY,int IconWidth,int IconHeight,string Icon)
		{
			this.Id= Id;
			this.Level= Level;
			this.IconX= IconX;
			this.IconY= IconY;
			this.IconWidth= IconWidth;
			this.IconHeight= IconHeight;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
