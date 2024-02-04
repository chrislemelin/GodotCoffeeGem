using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class DestroyBurntEffect : EffectResource
{	
	[Export] int scoreGained;
	public DestroyBurntEffect() {
		 
	}
	
	public override void execute(Node node) {
		
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);

		List<Tile> tiles = matchBoard.getTilesWithColorOfGem(GemType.Black, value);
		if (tiles.Count > 0) {
			matchBoard.deleteGemAtPositions(tiles.Select(tile => tile.getTilePosition()).ToList());
			FindObjectHelper.getScore(node).addScore(scoreGained);
		}
	}
}
