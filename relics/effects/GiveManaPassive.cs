using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveManaPassive : EffectResource
{
	[Export] public int value;
	
	public GiveManaPassive() {
		 
	}
	
	public override void execute(Node node) {
		node.GetNode<Mana>(FindObjectHelper.MANA_NAME).modifyMana(value);
	}
}
