namespace PathologicalGames
{
	public static class PoolManager
	{
		public static readonly SpawnPoolsDict Pools = new SpawnPoolsDict();

		public static SpawnPool PoolGameBlocks => Pools["Blocks"];

		public static bool IsEnablePoolGameBlocks
		{
			get
			{
				if (Pools != null && Pools.ContainsKey("Blocks"))
				{
					return true;
				}
				return false;
			}
		}

		public static SpawnPool PoolGameEffect => Pools["GameEffect"];

		public static bool IsEnablePoolGameEffect
		{
			get
			{
				if (Pools != null && Pools.ContainsKey("GameEffect"))
				{
					return true;
				}
				return false;
			}
		}

		public static SpawnPool PoolGamePlaying => Pools["GamePlaying"];
	}
}
