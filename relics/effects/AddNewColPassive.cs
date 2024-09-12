using Godot;
using System;

[GlobalClass, Tool]
public partial class AddNewColPassive : EffectResource
{	
	public AddNewColPassive() {
		 
	}
	
	protected override void executeEffect(Node node) {
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(node);
		gameManagerIF.addGridUpgrade();
	}
}
