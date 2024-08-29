using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class ZeroCardEnergyRelic : RelicResource
{

	private bool executedThisTurn = false;
	public override void setUp()
	{
		base.setUp();
		FindObjectHelper.getMana(node).ManaChanged += checkManaAndCards;
		//FindObjectHelper.getHand(node).handChanged += checkManaAndCards;

	}

	public override void newTurn()
	{
		base.setUp();
		executedThisTurn = false;

	}


	private void checkManaAndCards() {
		Hand hand = FindObjectHelper.getHand(node);
		Mana mana = FindObjectHelper.getMana(node);
		//if (hand.getAllCards().Count == 0 && mana.getMana() == 0 && !executedThisTurn) {
		//	executeEffects();
		//	executedThisTurn = true;
		//}
	}

}
