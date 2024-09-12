using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StartingMoneyUpgradeButton : MetaUpgradeButton
{
	public override void _Ready() {
		base._Ready();
	}

	protected override void doUpgrade() {
		gameManagerIF.addStartingCoins();
	}

	protected override int getUpgradeLevel() {
		return gameManagerIF.getStartingCoinsUpgradeLevel();
	}

	protected override int getUpgradeMaxLevel() {
		return gameManagerIF.getStartingCoinsUpgradeLevelMax();
	}
}
