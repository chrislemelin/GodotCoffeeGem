using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class ShowFutureGemsEffect : EffectResource
{	
	[Export] GemAddonType type;

	public ShowFutureGemsEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.setShowFutureTiles(true);
	}
}
