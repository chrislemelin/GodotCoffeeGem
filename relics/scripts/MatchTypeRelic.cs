using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class MatchTypeRelic : RelicResource
{
	float currentMult = 1.0f;
	[Export] int minMatchSize;
	[Export] GemType ingredientType;

	public override void ingredientsMatched(Match match)
	{
		if (match.ingredients.Count >= minMatchSize && match.GetGemType() == ingredientType) {
			executeEffects();
		}
	}


}