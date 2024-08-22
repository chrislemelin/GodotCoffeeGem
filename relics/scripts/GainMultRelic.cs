using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class GainMultRelic : RelicResource
{
	private float leftover = 0;
	public override void setUp(){
		FindObjectHelper.getScore(node).multGained+= execute;
		FindObjectHelper.getNewTurnButton(node).TurnCleanUp += () => leftover = 0;
	}

	private void execute(float value) {
		leftover += value;
		while(leftover >= 1) {
			leftover -=1;
			executeEffectsOrIncreaseCount();
		}
	}


}

