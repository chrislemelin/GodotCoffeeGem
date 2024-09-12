using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShopCardChoiceUpgradeButton : MetaUpgradeButton
{
	public override void _Ready() {
		base._Ready();
	}

	protected override void doUpgrade() {
		gameManagerIF.addCardsInShop();
	}

	protected override int getUpgradeLevel() {
		return gameManagerIF.getShopCardChoiceUpgradeLevel();
	}

	protected override int getUpgradeMaxLevel() {
		return gameManagerIF.getShopCardChoiceUpgradeLevelMax();
	}
}
