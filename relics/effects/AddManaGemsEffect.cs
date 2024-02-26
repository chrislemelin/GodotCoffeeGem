using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AdddManaGemsEffect : EffectResource
{	
	[Export] GemAddonType type;

	public AdddManaGemsEffect() {
		 
	}
	
	public override void execute(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.addGemAddons(matchBoard.getRandomNonBlackNonAddonTiles(value).Select(tile => tile.getTilePosition()).ToList(), type);
	}
}
