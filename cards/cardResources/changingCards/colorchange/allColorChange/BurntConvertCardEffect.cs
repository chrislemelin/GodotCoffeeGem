using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


[GlobalClass, Tool]
public partial class BurntConvertCardEffect : CardEffectIF
{
	public BurntConvertCardEffect() {
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Vector2> positions = getAllTiles(matchBoard);
		matchBoard.changeGemsColorAtPosition(positions, effectGemType.GetGemType(), true);
	}

	public  List<Vector2> getAllTiles(MatchBoard matchBoard)
	{
		List<Tile> tiles = matchBoard.getTilesWithColorOfGem(GemType.Black);
		return tiles.Select(x => x.getTilePosition()).ToList();
	}
}

