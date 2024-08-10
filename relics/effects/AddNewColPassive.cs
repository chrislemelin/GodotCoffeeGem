using Godot;
using System;

[GlobalClass, Tool]
public partial class AddNewColPassive : EffectResource
{	
	public AddNewColPassive() {
		 
	}
	
	protected override void executeEffect(Node node) {
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(node);
		gameManagerIF.changeGridSize(gameManagerIF.getGridSize() + new Vector2(1,0));
	}
}
