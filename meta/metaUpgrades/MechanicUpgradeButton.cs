using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MechanicUpgradeButton : MetaUpgradeButton
{
	public override void _Ready() {
		base._Ready();
	}

	protected override void doUpgrade() {
		gameManagerIF.setMechanicUnlocked(true);
	}

	protected override int getUpgradeLevel() {
		return gameManagerIF.getMechanicUnlocked() ? 1 : 0;
	}

	protected override int getUpgradeMaxLevel() {
		return 1;
	}
}
