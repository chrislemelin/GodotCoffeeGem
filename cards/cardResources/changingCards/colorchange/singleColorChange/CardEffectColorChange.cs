using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectColorChange : CardEffectIF
{
	public CardEffectColorChange() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return getValue() == 1 ? SelectionType.Single : SelectionType.Double; 
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		matchBoard.changeGemsColorAtPosition(selectedTiles, effectGemType.GetGemType());
		// bool hasDrawnCard = false;
		// matchBoard.addMatchActionsOnPositions(selectedTiles, (list) => {
		// 	if (list.Count >= 4 && hasDrawnCard == false) {
		// 		hand.drawCards(1);
		// 		hasDrawnCard = true;
		// 	} 
		// });
	}
}
