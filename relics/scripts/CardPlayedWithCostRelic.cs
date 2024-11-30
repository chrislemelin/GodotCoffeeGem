using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardPlayedWithCostRelic : RelicResource
{
	[Export] private int targetEnergyCost;
	private bool executedThisTurn = false;
	public override void setUp()
	{
		base.setUp();
		FindObjectHelper.getHand(node).cardPlayed += (CardResource card) => {
			if (card.getEnergyCost().Equals(targetEnergyCost)) {
				incrementCounter();
			}
		};

	}
}
