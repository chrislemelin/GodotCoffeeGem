using Godot;
using System;

[GlobalClass, Tool]
public partial class AddCardsToChooseEffect : EffectResource
{	

	public AddCardsToChooseEffect() {
		 
	}
	
	public override void execute(Node node) {
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(node);
		gameManagerIF.setNumberOfCardToChoose(gameManagerIF.getNumberOfCardToChoose() + value);
	}
}
