using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddAddonGemsEffect : EffectResource
{	
	[Export] GemAddonType type;

	public AddAddonGemsEffect() {
		 
	}
	
	public override void execute(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.addGemAddons(matchBoard.getRandomNonBlackNonAddonTiles(value).Select(tile => tile.getTilePosition()).ToList(), type);
	}
}
