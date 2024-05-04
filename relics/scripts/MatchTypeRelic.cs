using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[GlobalClass, Tool]
public partial class MatchTypeRelic : RelicResource
{
	[Export] int minMatchSize;
	[Export] GemType ingredientType;

	public override void ingredientsMatched(Match match)
	{
		base.ingredientsMatched(match);
		if (match.ingredients.Count >= minMatchSize && match.GetGemType() == ingredientType) {
			executeEffectsOrIncreaseCount();
		}
	}

	public override void newTurn()
	{
		base.newTurn();	
	}

}
