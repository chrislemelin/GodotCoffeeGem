using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]

public partial class CardEffectPopAllBlack : CardEffectIF
{
	public CardEffectPopAllBlack()
	{

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Tile> blackIngredients = matchBoard.getTilesWithColorOfGem(GemType.Black);
		matchBoard.deleteGemAtPositions(blackIngredients.Select(tile => tile.getTilePosition()).ToList());
	}


}
