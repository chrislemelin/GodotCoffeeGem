using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddAddonGemsEffect : EffectResource
{	
	[Export] GemAddonType type;

	public AddAddonGemsEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.addGemAddons(matchBoard.getRandomNonSpecialNonAddonTiles(value).Select(tile => tile.getTilePosition()).ToList(), type);
	}
}
