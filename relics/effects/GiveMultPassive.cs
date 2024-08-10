using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveMultPassive : EffectResource
{	
	[Export] float multIncrease;
	protected override void executeEffect(Node node) {
		FindObjectHelper.getScore(node).addMult(multIncrease);
	}
}
