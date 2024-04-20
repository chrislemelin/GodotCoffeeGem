using Godot;
using System;

public partial class IngredientUpgradeButton : MetaUpgradeButton
{
	[Export] GemType gemType;

	public override void _Ready() {
		base._Ready();
		button.Icon = gemType.getSmallTexture2D();
	}

	protected override void doUpgrade() {
		gameManagerIF.upgradeColorMeta(gemType);
	}

	protected override int getUpgradeLevel() {
		return gameManagerIF.getColorUpgradeMeta(gemType);
	}

	protected override int getUpgradeMaxLevel() {
		return gameManagerIF.getColorUpgradeMetaMax();
	}
}
