using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class CreateRandomGemsEffects : EffectResource
{	
	[Export] GemType type;

	public CreateRandomGemsEffects() {
		 
	}
	
	protected override void executeEffect(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.changeGemsColorAtPosition(matchBoard.getRandomNonSpecialNotOfTypeTiles(value, type).Select(tile => tile.getTilePosition()).ToList(), type, true);
	}
}
