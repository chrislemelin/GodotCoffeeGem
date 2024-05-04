using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class RemoveColumnEffect : EffectResource
{	

	public RemoveColumnEffect() {
		 
	}
	
	public override void execute(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.removeColumn();
	}
}
