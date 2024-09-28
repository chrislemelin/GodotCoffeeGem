using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectRainbowChangeRandom : CardEffectIF
{

	public CardEffectRainbowChangeRandom() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		GemType gemType = matchBoard.getTile(selectedTiles[0]).Gem.Type;
		List<Vector2> tilePositions = matchBoard.getRandomNonSpecialNotOfTypeTiles(getValue(), gemType).Select((tile) => tile.getTilePosition()).ToList();
		matchBoard.changeGemsColorAtPosition(tilePositions, gemType);
		bool hasDrawnCard = false;
		matchBoard.addMatchActionsOnPositions(tilePositions, (list) =>  {
			if (hasDrawnCard == false){
				hand.drawCards(1);
				hasDrawnCard = true;
			}
		});
	}
}
