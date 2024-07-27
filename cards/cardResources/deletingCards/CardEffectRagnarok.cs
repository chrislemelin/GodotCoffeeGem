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
		HashSet<Tile> burntTiles = matchBoard.getTilesWithColorOfGem(GemType.Black).ToHashSet();
		//FindObjectHelper.getScore(matchBoard).addScore(50*tiles.Count);
		//HashSet<Tile> burntTiles
		HashSet<Tile> nonBurntTiles = matchBoard.getRandomNonBlackTile().ToHashSet();
		matchBoard.deleteGemAtPositionsWithScoringForEach(burntTiles.Select(tile => tile.getTilePosition()).ToList(), 50);
		matchBoard.deleteGemAtPositions(nonBurntTiles.Select(tile => tile.getTilePosition()).ToList());
	}
	
}
