using Godot;
using System;

public enum MapEventType
{
	Money,
	RemoveCard,
	GainCard,
	UpgradeCard,
	Mechanic,
	RelicShop,
	Heal,
	Home
}

static class MapEventTypeHelper
{
	static Random random = new Random();

	public static string getString(this MapEventType type){
		int cost = type.getCost();
		string returnString = "";
		switch (type)
		{
			case MapEventType.Money:
				returnString = "Money";
				break;
			case MapEventType.RemoveCard:
				returnString = "Remove Card";
				break;
			case MapEventType.GainCard:
				returnString = "Gain Card";
				break;
			case MapEventType.UpgradeCard:
				returnString = "Upgrade Card";
				break;
			case MapEventType.Mechanic:
				returnString = "Mechanic";
				break;
			case MapEventType.RelicShop:
				returnString = "Relics";
				break;
			case MapEventType.Heal:
				returnString = "Heal a Heart";
				break;
			case MapEventType.Home:
				returnString = "Home";
				break;
		}
		if (cost > 0) {
			returnString +="\n("+cost+" Coins)";
		}
		return returnString;
	}

	public static int getCost(this MapEventType type) {
		switch (type)
		{
			case MapEventType.Mechanic:
				return 50;
			case MapEventType.RelicShop:
				return 40;             
			default:
				return 0;
		}
	}

	public static MapEventType getRandom()
	{
		//return MapEventType.RelicShop;
		return (MapEventType)random.Next(6);
		//return MapEventType.RemoveCard;
	}
	
}
