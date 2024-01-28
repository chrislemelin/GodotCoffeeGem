using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public static CardRarity getRandom() {
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

    public static List<CardResource> getRandomCards(int cardCount, CardList cardPool) {
        int commonCards = 0;
		int uncommonCards = 0;
		int rareCards = 0;
		for(int count = 0; count < cardCount; count++) {
			CardRarity cardRarity = CardRarityHelper.getRandom();
			if(cardRarity == CardRarity.Common) {
				commonCards++;
			} else if (cardRarity == CardRarity.Uncommon) {
				uncommonCards++;
			} else {
				rareCards++;
			}
		}
		List<CardResource> cardsToChoose = new List<CardResource>();
		cardsToChoose.AddRange(getRandomCardsOfRarity(commonCards, CardRarity.Common, cardPool));
		cardsToChoose.AddRange(getRandomCardsOfRarity(uncommonCards, CardRarity.Uncommon, cardPool));
		cardsToChoose.AddRange(getRandomCardsOfRarity(rareCards, CardRarity.Rare, cardPool));
        RandomHelper.Shuffle(cardsToChoose);
		return cardsToChoose;
    }

    
	private static List<CardResource> getRandomCardsOfRarity(int count, CardRarity cardRarity, CardList cardPool){
		List<CardResource> cardPoolList = cardPool.allCards.Where((card) => card.rarity == cardRarity).ToList();
		RandomHelper.Shuffle(cardPoolList);
		return cardPoolList.GetRange(0, count);
	}
}

