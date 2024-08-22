using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class DrawCardsRelic : RelicResource
{
	public override void setUp(){
		FindObjectHelper.getHand(node).cardDrawnNotFromNewTurn+= (card) => execute();
	}

	private void execute() {
		executeEffectsOrIncreaseCount();
	}
}

