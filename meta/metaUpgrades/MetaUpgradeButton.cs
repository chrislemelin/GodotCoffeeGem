using Godot;
using System;

public partial class MetaUpgradeButton : Node
{
	[Export] protected Button button;
	[Export] protected GameManagerIF gameManagerIF;
	[Export] protected RichTextLabel upgradeLevel;
	[Export] protected RichTextLabel costLabel;
	[Export] protected int costBase = 10;
	[Export] protected int costIncrement = 5;


	public override void _Ready() {
		button.Pressed += () =>  {
			gameManagerIF.addMetaCoins(-getCost());
			doUpgrade();
			updateLabels();
		};
		gameManagerIF = FindObjectHelper.getGameManager(this);
		updateLabels();
		costLabel.Text = getCost().ToString();
		gameManagerIF.metaCoinsChanged += (value) => updateLabels();
	}

	public void updateLabels () {
		int currentLevel = getUpgradeLevel();
		int maxLevel = getUpgradeMaxLevel();
		int currentMetaCoins = gameManagerIF.getMetaCoins();
		upgradeLevel.Text =  currentLevel + "/" + maxLevel; 
		if (currentLevel == maxLevel) {
			button.Disabled = true;
		}
		if (currentMetaCoins < getCost()) {
			button.Disabled = true;
		}
		costLabel.Text = getCost().ToString();
	}

	private int getCost() {
		int currentLevel = getUpgradeLevel();
		return currentLevel * costIncrement + costBase;
	}

	protected virtual  void doUpgrade() {
	}

	protected virtual int getUpgradeLevel() {
		return 0;
	}

	protected virtual int getUpgradeMaxLevel() {
		return 0;
	}
}
