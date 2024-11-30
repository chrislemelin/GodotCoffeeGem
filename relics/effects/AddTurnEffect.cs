using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddTurnEffect : EffectResource
{	
	public AddTurnEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		Score score = FindObjectHelper.getScore(node);
		score.setTurnsRemaining(score.getTurnsRemaining() + 1);
	}
}
