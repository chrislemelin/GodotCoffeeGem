using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradeCard : Control
{
	[Export] CardList cardList;
	[Export] DeckViewUI deckViewUI;
	[Export] UpgradeCardChoiceUI upgradeCardChoiceUI;
	[Signal] public delegate void UpgradeCardDoneEventHandler();


	public override void _Ready()
	{
		base._Ready();
		//upgradeCardsFromList(cardList.getCards());
		upgradeCardChoiceUI.UpgradeCardSelected += upgradeCard;
	}
	public void upgradeCardsFromList(List<CardResource> cards) {
		List<CardResource> cardsWithUpgrades = cards.Where(card => card.getCardUpgrades().Count > 0).ToList();
		if (cardsWithUpgrades.Count > 0) {
			deckViewUI.setUp(cardsWithUpgrades, null, TextHelper.centered("Upgrade a Card"), false);
			deckViewUI.setCardClickedCallBack((cardResource) => upgradeCardChoiceUI.previewCard(cardResource));
		}
		Show();

		if (cardsWithUpgrades.Count == 0) {
			EmitSignal(SignalName.UpgradeCardDone);
			Hide();
		}
	}

	public bool hasCardsToUpgrade(List<CardResource> cards) {
		List<CardResource> cardsWithUpgrades = cards.Where(card => card.getCardUpgrades().Count > 0).ToList();
		return cardsWithUpgrades.Count > 0;
	}

	private void upgradeCard(CardResource oldCard, CardResource newCard) {
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
		gameManager.removeCardFromDeckList(oldCard);
		gameManager.addCardToDeckList(newCard);
		EmitSignal(SignalName.UpgradeCardDone);
		Hide();
	}
	 
}
