using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddManaGemsEffect : EffectResource
{	

	public AddManaGemsEffect() {
		 
	}
	
	public override void execute(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.addGemAddons(matchBoard.getRandomNonBlackNonAddonTiles(value).Select(tile => tile.getTilePosition()).ToList(), GemAddonType.Mana);
	}
}
