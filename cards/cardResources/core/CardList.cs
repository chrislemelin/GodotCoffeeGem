using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardList : Resource
{

	[Export] private Godot.Collections.Array<CardResource> allCards;


	public CardList () {
	
	}

	public void setCards(Godot.Collections.Array<CardResource> allCards) {
		this.allCards = allCards;
	}

	public List<CardResource> getCards() {
		return allCards.ToList().Where(card => card != null).ToList();
	}

	public Godot.Collections.Array<CardResource> getRealList() {
		return allCards;
	}

	public CardResource getRandomCard() {
		CardRarity selectedRarity = CardRarityHelper.getRandom();
		List<CardResource> cards = allCards.Where((card) => card.rarity == selectedRarity).ToList();
		RandomHelper.Shuffle(cards);
		return(cards[0]);
	}

}
