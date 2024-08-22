using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class GainCoinsRelic : RelicResource
{
	[Export] private int threshold = 5;
	int leftover = 0;
	public override void setUp(){
		FindObjectHelper.getGameManager(node).coinsGained += execute;
	}

	private void execute(int value) {
		leftover += value;
		while(leftover >= threshold) {
			leftover -=threshold;
			executeEffectsOrIncreaseCount();
		}
	}

}

