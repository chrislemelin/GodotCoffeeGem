using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddGooEffect : EffectResource
{	
	[Export] GemAddonType type;

	public AddGooEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.gooColumn(0);
		matchBoard.gooRightMostColumn();
	}
}
