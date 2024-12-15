using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddCoinsEffect : EffectResource
{	
	protected override void executeEffect(Node node) {
		FindObjectHelper.getGameManager(node).addCoins(value);
	}

	public override bool canRunNotInLevel() {
		return true;
	}
}
