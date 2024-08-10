using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveCardPassive : EffectResource
{
	[Export] public int value;
	
	public GiveCardPassive() {
		 
	}
	
	protected override void executeEffect(Node node) {
		FindObjectHelper.getHand(node).drawCards(value);
	}
}
