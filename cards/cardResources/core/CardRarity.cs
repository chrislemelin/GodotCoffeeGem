using Godot;
using System;

public enum CardRarity
{
    Common,
	Uncommon,
    Rare
}

static class CardRarityHelper
{
    //(int commonModification, int uncommonModification, int rareModification);
    public static Color getColor(this CardRarity cardRarity) {
        switch(cardRarity) {
            case CardRarity.Common:
                return new Color("858585");
            case CardRarity.Uncommon:
                return new Color("25cff5");
            case CardRarity.Rare:
            default:
                return new Color("fad046");
        }
    }

    public static int dropPercent(this CardRarity cardRarity) {
        switch(cardRarity) {
            case CardRarity.Common:
                return 65;
            case CardRarity.Uncommon:
                return 30;
            case CardRarity.Rare:
            default:
                return 5;
        }
    }

    public static CardRarity getRandomScaled() {
        int randomPercent = GD.RandRange(1,100);
        if(randomPercent <= dropPercent(CardRarity.Common)) {
            return CardRarity.Common;
        }
        if(randomPercent <= dropPercent(CardRarity.Common) + dropPercent(CardRarity.Uncommon) ) {
            return CardRarity.Uncommon;
        } else {
            return CardRarity.Rare;
        }
    }
}

