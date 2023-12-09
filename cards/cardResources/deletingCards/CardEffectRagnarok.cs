using Godot;
using Godot.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectRagnarok : CardEffectIF
{
	public CardEffectRagnarok() {
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		matchBoard.deleteGemAtPositions(matchBoard.getAllTiles().Select(tile => tile.getPosition()).ToList());
	}
	
}
