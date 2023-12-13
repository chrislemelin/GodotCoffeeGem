using Godot;
using System;

[GlobalClass, Tool]
public partial class AddNewColPassive : EffectResource
{	
	public AddNewColPassive() {
		 
	}
	
	public override void execute(Node node) {
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(node);
		gameManagerIF.changeGridSize(gameManagerIF.getGridSize() + new Vector2(1,0));
	}
}
