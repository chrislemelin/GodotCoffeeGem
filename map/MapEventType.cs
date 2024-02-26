using Godot;
using System;

public enum MapEventType
{
    Money,
    RemoveCard,
    GainCard,
    UpgradeCard,
    Heal,
    Mechanic,
    Home
}

static class MapEventTypeHelper
{
    static Random random = new Random();

    public static string getString(this MapEventType type){
        switch (type)
		{
			case MapEventType.Money:
                return "Money";
            case MapEventType.RemoveCard:
                return "Remove Card";
            case MapEventType.GainCard:
                return "Gain Card";
            case MapEventType.UpgradeCard:
                return "Upgrade Card";
            case MapEventType.Mechanic:
                return "Upgrade Coffee Machine";
            case MapEventType.Heal:
                return "Heal a Heart";                
            case MapEventType.Home:
                return "Home";                
            default:
                return "";
        }
    }

    public static MapEventType getRandom()
	{
		return (MapEventType)random.Next(5);
        //return MapEventType.RemoveCard;
	}
	
}