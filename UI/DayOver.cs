using System.Collections.Generic;
using Godot;

public partial class DayOver : Control
{
	[Export] ButtonWithCoinCost sideHustleButton;
	[Export] ButtonWithCoinCost shoppingTherapyButton;
	[Export] ButtonWithCoinCost gemUpgradeButton;
	[Export] ButtonWithCoinCost moreCardsButton;
	[Export] ButtonWithCoinCost relicButton;


	[Export] Button advanceLevelButton;
	[Export] HomeGameManager gameManager;

	[Export] RichTextLabel confirmationText;

	[Export] ColorUpgrade colorUpgrade;

	[Export] NewCardSelection newCardSelection;


	private List<ButtonWithCoinCost> buttons = new List<ButtonWithCoinCost>();

	public override void _Ready()
	{
		base._Ready();
		buttons.Add(sideHustleButton);
		buttons.Add(shoppingTherapyButton);
		buttons.Add(gemUpgradeButton);
		buttons.Add(moreCardsButton);
		buttons.Add(relicButton);

		foreach(ButtonWithCoinCost buttonWithCoinCost in buttons) {
			buttonWithCoinCost.setCurrentCoin(gameManager.getCoins());
		}

		foreach(ButtonWithCoinCost buttonWithCoinCost in buttons) {
			buttonWithCoinCost.Pressed += () => {
				foreach(ButtonWithCoinCost currentButton in buttons) {
					currentButton.Disabled = true;
				}
				subtractCoins(buttonWithCoinCost);
				advanceLevelButton.Disabled = false;
			};
		}

		if (gameManager.getHealth() == gameManager.getMaxHealth()) {
			shoppingTherapyButton.Disabled = true;
		}
		sideHustleButton.Pressed += () => {
			confirmationText.Text = "Gained 10 Coins";
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
}
