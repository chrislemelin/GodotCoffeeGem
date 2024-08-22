using Godot;
using Godot.Collections;
using System;

public partial class MetaResetButton : Button
{
	[Export] GameManagerIF gameManagerIF;
	[Export] Array<IngredientUpgradeButton> ingredientUpgradeButtons = new();
	[Export] int metaCoinModifyValue = 0;
	public override void  _Ready() {
		Pressed += () =>  {
			gameManagerIF.resetMetaProgression();
			gameManagerIF.addMetaCoins(-gameManagerIF.getMetaCoins());

			foreach(IngredientUpgradeButton ingredientUpgradeButton in ingredientUpgradeButtons) {
				//ingredientUpgradeButton.updateLabels();
			}
		};
	}
	
}
