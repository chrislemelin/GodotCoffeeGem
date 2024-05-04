using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class RetainMultRelic : RelicResource
{
	private float mult;
	public override void afterTurnCleanUp()
	{
		FindObjectHelper.getScore(node).setMult(mult);
	}
	public override void beforeTurnCleanUp()
	{
		mult = FindObjectHelper.getScore(node).getMult();
	}
}
