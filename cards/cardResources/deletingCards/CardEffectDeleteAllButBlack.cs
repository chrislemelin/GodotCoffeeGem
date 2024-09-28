using Godot;
using Godot.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectDeleteAllButBlack : CardEffectIF
{
	public CardEffectDeleteAllButBlack() {
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Tile> tiles = matchBoard.getRandomNonBlackTile(-1);
		matchBoard.deleteGemAtPositions(tiles.Select(tile => tile.getTilePosition()).ToList());
		FindObjectHelper.getCamera(node).playDistort();
	}
	
}
