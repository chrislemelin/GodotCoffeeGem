using Godot;
using System;

public partial class MetaCoinRateUpgradeButton : MetaUpgradeButton
{
	public override void _Ready() {
		base._Ready();
	}

	protected override void doUpgrade() {
		gameManagerIF.addMetaCoinDropRate(1);
	}

	protected override int getUpgradeLevel() {
		return gameManagerIF.getMetaCoinDropRate();
	}

	protected override int getUpgradeMaxLevel() {
		return gameManagerIF.getMetaCoinDropRateMax();
	}
}
