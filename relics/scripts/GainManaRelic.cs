using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class GainManaRelic : RelicResource
{
	public override void setUp(){
		FindObjectHelper.getMana(node).ManaGained+= execute;
	}

	private void execute(int value) {
		for(int a = 0; a < value; a++) {
			executeEffectsOrIncreaseCount();
		}
	}

}

