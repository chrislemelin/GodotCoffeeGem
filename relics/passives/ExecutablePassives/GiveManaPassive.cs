using Godot;
using System;

[GlobalClass, Tool]
public partial class GiveManaPassive : ExecutablePassive
{
	[Export] public int value;
	
	public GiveManaPassive() {
		 
	}
	
	public override void execute(Node node) {
		GD.Print("gave mana");
		node.GetNode<Mana>(FindObjectHelper.MANA_NAME).modifyMana(value);
	}
}
