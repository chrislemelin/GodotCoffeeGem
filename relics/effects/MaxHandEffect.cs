using Godot;
using System;

[GlobalClass, Tool]
public partial class MaxHandEffect : EffectResource
{
	[Export] public int value;
	
	public MaxHandEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		FindObjectHelper.getHand(node).modifyNewCardsDrawnOnNewTurn(value);
	}
}
