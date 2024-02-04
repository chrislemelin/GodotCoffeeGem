using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveMultPassive : EffectResource
{	
	[Export] float multIncrease;
	public override void execute(Node node) {
		FindObjectHelper.getScore(node).addMult(multIncrease);
	}
}
