using Godot;
using System;

[GlobalClass, Tool]
public partial class AddCardsToChooseEffect : EffectResource
{	

	public AddCardsToChooseEffect() {
		 
	}
	
	protected override void executeEffect(Node node) {
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(node);
		gameManagerIF.setNumberOfCardToChoose(gameManagerIF.getNumberOfCardToChoose() + value);
	}
}
