using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectColorChangeRandom : CardEffectIF
{

	public CardEffectColorChangeRandom() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Vector2> tilePositions = matchBoard.getRandomNonSpecialNotOfTypeTiles(getValue(), effectGemType.GetGemType()).Select((tile) => tile.getTilePosition()).ToList();
		matchBoard.changeGemsColorAtPosition(tilePositions, effectGemType.GetGemType());
		bool hasDrawnCard = false;
		matchBoard.addMatchActionsOnPositions(tilePositions, (list) =>  {
			if (hasDrawnCard == false){
				hand.drawCards(1);
				hasDrawnCard = true;
			}
		});
	}
}
