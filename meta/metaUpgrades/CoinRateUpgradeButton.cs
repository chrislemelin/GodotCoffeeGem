using Godot;
using System;

public partial class CoinRateUpgradeButton : MetaUpgradeButton
{
	public override void _Ready() {
		base._Ready();
	}

	protected override void doUpgrade() {
		gameManagerIF.addCoinDropRate(1);
	}

	protected override int getUpgradeLevel() {
		return gameManagerIF.getCoinDropRate();
	}

	protected override int getUpgradeMaxLevel() {
		return gameManagerIF.getCoinDropRateMax();
	}
}
