using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveScorePassive : EffectResource
{	
	protected override void executeEffect(Node node) {
		FindObjectHelper.getScore(node).addScoreFromNode(value, (Control) node);
	}
}
