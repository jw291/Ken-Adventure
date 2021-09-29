using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	#region Stat
	[Serializable]
	public class Stat
	{
		public int level;
		public int maxHp;
		public int minattack;
		public int maxattack;
		public int totalExp;
	}

	[Serializable]
	public class StatData : ILoader<int, Stat>
	{
		public List<Stat> stats = new List<Stat>();

		public Dictionary<int, Stat> MakeDict()
		{
			Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
			foreach (Stat stat in stats)
				dict.Add(stat.level, stat);
			return dict;
		}
	}
	#endregion

	#region Item
	[Serializable]
	public class Item
	{
		public string name;
		public string parts;
		public int minattack;
		public int maxattack;
		public int price;
	}

	[Serializable]
	public class ItemData : ILoader<string, Item>
	{
		public List<Item> items = new List<Item>();

		public Dictionary<string, Item> MakeDict()
		{
			Dictionary<string, Item> dict = new Dictionary<string, Item>();
			foreach (Item item in items)
				dict.Add(item.name, item);
			return dict;
		}
	}
	#endregion
}