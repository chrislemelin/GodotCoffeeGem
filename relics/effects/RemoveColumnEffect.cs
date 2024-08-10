using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class RemoveColumnEffect : EffectResource
{	

	public RemoveColumnEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		matchBoard.removeColumn();
	}
}
