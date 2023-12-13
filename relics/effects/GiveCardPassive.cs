using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveCardPassive : EffectResource
{
	[Export] public int value;
	
	public GiveCardPassive() {
		 
	}
	
	public override void execute(Node node) {
		FindObjectHelper.getHand(node).drawCards(value);
	}
}
