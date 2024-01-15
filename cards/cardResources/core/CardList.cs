using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardList : Resource
{

	[Export] public Godot.Collections.Array<CardResource> allCards;

	public CardList () {
	
	}

	public CardResource getRandomCard() {
		CardRarity selectedRarity = CardRarityHelper.getRandomScaled();
		List<CardResource> cards = allCards.Where((card) => card.rarity == selectedRarity).ToList();
		RandomHelper.Shuffle(cards);
		return(cards[0]);
	}
}
