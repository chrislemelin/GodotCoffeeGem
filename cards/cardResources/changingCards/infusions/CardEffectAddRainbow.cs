using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectAddRainbow : CardEffectIF
{
	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Tile> tilesToEffect = matchBoard.getRandomNonBlackNonAddonTiles(getValue());
		tilesToEffect.ForEach((tile) => matchBoard.changeGemsColorAtPosition(tile.getTilePosition(), GemType.Rainbow));
	}
}
