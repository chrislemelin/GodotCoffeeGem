using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class AddCoinsEffect : EffectResource
{	
	public override void execute(Node node) {
		FindObjectHelper.getGameManager(node).addCoins(value);
	}
}
