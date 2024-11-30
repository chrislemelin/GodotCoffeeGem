using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class BurntIngredientConditionalEffect : EffectResource
{	
	[Export] EffectResource effect;
	[Export] int burntRequirement;

	public BurntIngredientConditionalEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard.getTilesWithColorOfGem(GemType.Black).Count >= burntRequirement) {
			effect.execute(node);
		}
	}
}
