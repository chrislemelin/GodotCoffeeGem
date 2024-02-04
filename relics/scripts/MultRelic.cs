using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class MultRelic : RelicResource
{
	float currentMult = 1.0f;
	[Export] float targetMult;
	public override void beforeTurnCleanUp()
	{
		currentMult = FindObjectHelper.getScore(node).getMult();
	}

	public override void afterTurnCleanUp()
	{
		if (currentMult >= targetMult) {
			executeEffects();
		}
		currentMult = FindObjectHelper.getScore(node).getMult();
	}


}
