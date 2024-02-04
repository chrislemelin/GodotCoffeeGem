using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveScorePassive : EffectResource
{	
	public override void execute(Node node) {
		FindObjectHelper.getScore(node).addScore(value);
	}
}
