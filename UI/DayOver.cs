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


	[Export] Control relicShop;
	[Export] PackedScene relicUIPackedScene;


	[Export] Button advanceLevelButton;
	[Export] HomeGameManager gameManager;

	[Export] RichTextLabel confirmationText;

	[Export] ColorUpgrade colorUpgrade;
	[Export] NewCardSelection newCardSelection;
	[Export] CardResource upgradedSwitchCard;
	[Export] CardResource vertSwitchCard;
	[Export] CardResource horizSwitchCard;


	private List<Button> buttons = new List<Button>();
	private List<ButtonWithCoinCost> buttonWithCoinCost = new List<ButtonWithCoinCost>();

	private CardResource? cardToReplace;


	public override void _Ready()
	{
		base._Ready();
		buttonWithCoinCost.Add(sideHustleButton);
		buttonWithCoinCost.Add(shoppingTherapyButton);
		buttonWithCoinCost.Add(gemUpgradeButton);
		buttonWithCoinCost.Add(moreCardsButton);
		buttonWithCoinCost.Add(relicButton);
		buttonWithCoinCost.Add(upgradeCardButton);
		buttons.AddRange(buttonWithCoinCost);


		foreach(ButtonWithCoinCost buttonWithCoinCost in buttonWithCoinCost) {
			buttonWithCoinCost.setCurrentCoin(gameManager.getCoins());
		}

		foreach(ButtonWithCoinCost buttonWithCoinCost in buttonWithCoinCost) {
			buttonWithCoinCost.Pressed += () => {
				foreach(Button currentButton in buttons) {
					currentButton.Disabled = true;
				}
				subtractCoins(buttonWithCoinCost);
				advanceLevelButton.Disabled = false;
			};
		}
		
		addRelicsToShop();



		if (gameManager.getHealth() == gameManager.getMaxHealth()) {
			shoppingTherapyButton.Disabled = true;
		}
		cardToReplace = gameManager.getDeckList().ToList().Find(cardResource => cardResource.equalToCard(vertSwitchCard) || cardResource.equalToCard(horizSwitchCard));
		if (cardToReplace == null) {
			upgradeCardButton.Disabled = true;
		}


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
		advanceLevelButton.Pressed += () => gameManager.advanceLevel();
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
		confirmationText.Text = "Upgraded the points and mult you gain from " + gemType.ToString() + " gems";
	}

	private void addRelic() {
		List<RelicResource> relicResources = gameManager.getRelicPool();
		RandomHelper.Shuffle(relicResources);
		RelicResource relicResource = relicResources[0];

		gameManager.addRelic(relicResource);
		confirmationText.Text = relicResource.description;
	}

	private void upgradeCard() {
		gameManager.removeCardFromDeckList(cardToReplace);
		gameManager.addCardToDeckList(upgradedSwitchCard);
	}

	private void addRelicsToShop() {
		List<RelicResource> relicResources = gameManager.getRelicPool();
		RandomHelper.Shuffle(relicResources);
		List<RelicResource> relicsInShop = relicResources.GetRange(0,Math.Min(2,relicResources.Count));

		foreach(RelicResource relicResource in relicsInShop) {
			RelicUI relicUI = (RelicUI)relicUIPackedScene.Instantiate();
			relicUI.showPrice = true;
			relicUI.setRelic(relicResource);

			relicUI.buyButton.Disabled = gameManager.getCoins() < relicResource.cost;
			buttons.Add(relicUI.buyButton);
			relicUI.buyButton.Pressed += () => {
				foreach(Button currentButton in buttons) {
					currentButton.Disabled = true;
				}
				subtractCoins(relicResource);
				advanceLevelButton.Disabled = false;
				confirmationText.Text = "Purchased " + relicResource.title;
				gameManager.addRelic(relicResource);
				relicUI.QueueFree();
			};
			relicShop.AddChild(relicUI);
		}

	}
}
