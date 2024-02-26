using Godot;
using System;

public partial class ManaEndTurnRelic : RelicResource
{

	[Export] int manaValue;

	public override void beforeTurnCleanUp()
	{
		if (FindObjectHelper.getMana(node).manaValue >= manaValue) {
			executeEffectsOrIncreaseCount();
		}
	}
}
