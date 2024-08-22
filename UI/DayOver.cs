using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class DayOver : Control
{
	[Export] ButtonWithCoinCost sideHustleButton;
	[Export] ButtonWithCoinCost shoppingTherapyButton;
	[Export] ButtonWithCoinCost gemUpgradeButton;
	[Export] ButtonWithCoinCost moreCardsButton;
	[Export] ButtonWithCoinCost relicButton;
	[Export] ButtonWithCoinCost upgradeCardButton;
	[Export] ButtonWithCoinCost removeCardFromDeckButton;

	[Export] Control relicShop;
	[Export] PackedScene relicUIPackedScene;

	[Export] CustomButton advanceLevelCustomButton;

	[Export] HomeGameManager gameManager;

	[Export] RichTextLabel confirmationText;

	[Export] ColorUpgrade colorUpgrade;
	[Export] NewCardSelection newCardSelection;
	[Export] CardResource upgradedSwitchCard;

	[Export] CardResource vertSwitchCard;
	// test dock 
	[Export] CardResource horizSwitchCard;
	[Export] RelicSelection relicSelection;
	[Export] Control cardShop;
	[Export] PackedScene cardScene;
	[Export] PackedScene marginContainerScene;


	private List<Button> buttons = new List<Button>();
	private List<ButtonWithCoinCost> buttonWithCoinCost = new List<ButtonWithCoinCost>();
	private List<RelicUI> relicUIsInShop = new List<RelicUI>();
	private List<CardInfoLoader> cardsInShop = new List<CardInfoLoader>();

	private CardResource? cardToReplace;

	[Export] DeckViewUI deckViewUI;


	public override void _Ready()
	{
		base._Ready();
		buttonWithCoinCost.Add(sideHustleButton);
		buttonWithCoinCost.Add(shoppingTherapyButton);
		buttonWithCoinCost.Add(removeCardFromDeckButton);
		buttonWithCoinCost.Add(gemUpgradeButton);
		buttonWithCoinCost.Add(moreCardsButton);
		buttonWithCoinCost.Add(relicButton);
		buttonWithCoinCost.Add(upgradeCardButton);
		buttons.AddRange(buttonWithCoinCost);

		//

		foreach(ButtonWithCoinCost buttonWithCoinCost in buttonWithCoinCost) {
			buttonWithCoinCost.setCurrentCoin(gameManager.getCoins());
		}

		foreach(ButtonWithCoinCost buttonWithCoinCost in buttonWithCoinCost) {
			buttonWithCoinCost.Pressed += () => {
				foreach(Button currentButton in buttons) {
					//currentButton.Disabled = true;
				}
				subtractCoins(buttonWithCoinCost);
				//advanceLevelButton.Disabled = false;
			};
		}
		
		addRelicsToShop();
		addCardsToShop();
		gameManager.coinsChanged += (int coins) => coinsChanged(coins);

		if (gameManager.getHealth() == gameManager.getMaxHealth()) {
			shoppingTherapyButton.Disabled = true;
		}
		cardToReplace = gameManager.getDeckList().ToList().Find(cardResource => cardResource.equalToCard(vertSwitchCard) || cardResource.equalToCard(horizSwitchCard));
		if (cardToReplace == null) {
			upgradeCardButton.Disabled = true;
		}

		removeCardFromDeckButton.Pressed += () => {
			deckViewUI.setUp(gameManager.getDeckList(), removeCardFromDeck, TextHelper.centered("Remove Card From Deck"));
			confirmationText.Text = "Removed Card from Deck";
		};


		sideHustleButton.Pressed += () => {
			confirmationText.Text = "Gained 10 Coins";
		};
		upgradeCardButton.Pressed += () => {
			upgradeCard();
			confirmationText.Text = "upgraded card " + cardToReplace.Title;
		};


		shoppingTherapyButton.Pressed += () => {
			gameManager.setHealth(gameManager.getHealth() + 1);
			confirmationText.Text = "Healed 1 heath!";
		};
		gemUpgradeButton.Pressed += () => {
			addColorUpgrade();
		};
		moreCardsButton.Pressed += () => {
			((HomeGameManager)gameManager).selectNewCard();
		};
		relicButton.Pressed += addRelic;
		advanceLevelCustomButton.Pressed += () => gameManager.advanceLevel();
		coinsChanged(gameManager.getCoins());
	}

	public void subtractCoins(ButtonWithCoinCost buttonWithCoinCost) {
		gameManager.addCoins(buttonWithCoinCost.cost * -1);
	}

	public void subtractCoins(RelicResource relicResource) {
		gameManager.addCoins(relicResource.cost * -1);
	}

	private void addColorUpgrade() {
		ColorUpgrade newColorUpgrade = (ColorUpgrade)colorUpgrade.Duplicate();
		GemType gemType = GemTypeHelper.getRandomColor();
		newColorUpgrade.gemType = gemType;
		gameManager.addColorUpgrade(newColorUpgrade);
		confirmationText.Text = "Upgraded the points and mult you gain from " + gemType.ToString();
	}

	private void addRelic() {
		if (relicSelection.hasSetUpRelics) {
			relicSelection.setToVisible();
		} else {
			List<RelicResource> relicResources = gameManager.getRelicPool();
			RandomHelper.Shuffle(relicResources);
			List<RelicResource> relicsInSelection = relicResources.GetRange(0,Math.Min(3,relicResources.Count));
			relicSelection.setRelics(relicsInSelection, false);
			relicSelection.Visible = true;
			relicSelection.WindowClosed += () => advanceLevelCustomButton.checkForFocus();
		}
	}

	private void upgradeCard() {
		gameManager.removeCardFromDeckList(cardToReplace);
		gameManager.addCardToDeckList(upgradedSwitchCard);
	}

	private void coinsChanged(int coins) {
		foreach(ButtonWithCoinCost buttonWithCoinCost in buttonWithCoinCost) {
			buttonWithCoinCost.setCurrentCoin(coins);
		}
		if (gameManager.getHealth() == gameManager.getMaxHealth()) {
			shoppingTherapyButton.Disabled = true;
		}
		foreach(RelicUI relicUI in relicUIsInShop) {
			relicUI.buyButton.Disabled = gameManager.getCoins() < relicUI.relicResource.cost;
		}
		foreach(CardInfoLoader cardInfoLoader in cardsInShop) {
			if(cardInfoLoader.cardResource.getCoinCost() > coins) {
				cardInfoLoader.setDisabled();
			} else {
				cardInfoLoader.setEnabled();
			}
		}
	}

	private void addRelicsToShop() {
		List<RelicResource> relicResources = gameManager.getRelicPool();
		RandomHelper.Shuffle(relicResources);
		List<RelicResource> relicsInShop = relicResources.GetRange(0,Math.Min(3,relicResources.Count));

		foreach(RelicResource relicResource in relicsInShop) {
			RelicUI relicUI = (RelicUI)relicUIPackedScene.Instantiate();
			relicUIsInShop.Add(relicUI);
			relicUI.showPrice = true;
			relicUI.setRelic(relicResource);

			relicUI.buyButton.Disabled = gameManager.getCoins() < relicResource.cost;
			buttons.Add(relicUI.buyButton);
			relicUI.buyButton.Pressed += () => {

				subtractCoins(relicResource);
				advanceLevelCustomButton.Disabled = false;
				confirmationText.Text = "Purchased " + relicResource.title;
				gameManager.addRelic(relicResource);
				relicUI.QueueFree();
			};
			relicShop.AddChild(relicUI);
		}
	}
	private void addCardsToShop() {
		List<CardResource> cards = CardRarityHelper.getRandomCards(5, gameManager.getCardPool());
			foreach(CardResource cardResource in cards) {
			CardInfoLoader cardInfoLoader = (CardInfoLoader)cardScene.Instantiate();
			MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			marginContainer.AddChild(cardInfoLoader);
			cardShop.AddChild(marginContainer);

			//cardInfoLoaders.Add(cardInfoLoader);
			cardsInShop.Add(cardInfoLoader);
			cardInfoLoader.setUpCard(cardResource);
			cardInfoLoader.setShowCoinCost(true);
			cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource, cardInfoLoader, marginContainer);
		}
	}

	private void cardClicked(InputEvent inputEvent, CardResource cardResource, CardInfoLoader cardInfoLoader, MarginContainer marginContainer) {
		if (InputHelper.isSelectedAction(inputEvent))
		{
			if (gameManager.getCoins() >= cardResource.getCoinCost() && cardsInShop.Contains(cardInfoLoader)) {
				gameManager.addCoins(-1 * cardResource.getCoinCost());
				gameManager.addCardToDeckList(cardResource);
				//cardShop.RemoveChild(marginContainer);
				cardsInShop.Remove(cardInfoLoader);
				cardInfoLoader.destroyCard();
			}
		};
	}

	private void removeCardFromDeck(CardResource cardResource) {
		gameManager.removeCardFromDeckList(cardResource);
	}
}
