using Godot;
using Godot.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class PopAllExceptCardEffect : CardEffectIF
{
	[Export] GemType gemType;
	public PopAllExceptCardEffect()
	{
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Vector2> positions = getAllTilesToEffect(matchBoard);
		matchBoard.deleteGemAtPositions(positions);
	}

	private  List<Vector2> getAllTilesToEffect(MatchBoard matchBoard)
	{
		List<Tile> tiles = matchBoard.getTilesWithCondition(tile => tile.Gem != null && tile.Gem.Type != gemType);
		return tiles.Select(x => x.getTilePosition()).ToList();
	}
}
