using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardList : Resource
{

	[Export] bool removeDuplicates = false;
	[Export] private Godot.Collections.Array<CardResource> allCards;


	public CardList () {
	}

	public void setCards(Godot.Collections.Array<CardResource> allCards) {
		this.allCards = allCards;
	}

	public void addCards(Godot.Collections.Array<CardResource> cards) {
		allCards.AddRange(cards);
	}

	public List<CardResource> getCards() {
		List<CardResource> returnCards = allCards.ToList().Where(card => card != null).ToList();
		if (removeDuplicates) {
			List<CardResource> dups = returnCards.GroupBy(x => x.Title)
			.Where(g => g.Count() > 1)
			.Select(y => y.First())
			.ToList();
			List<CardResource> returnCardsNoDups = returnCards.GroupBy(x => x.Title).Select(
				y => {
					return y.First();
			}).ToList();
			if (dups.Count > 0) {
				GD.Print("found dups: ");
				foreach(CardResource cardResource in dups) {
					GD.Print(cardResource.Title);
				}
			}
			returnCards = returnCardsNoDups;
		}
		return returnCards;
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
