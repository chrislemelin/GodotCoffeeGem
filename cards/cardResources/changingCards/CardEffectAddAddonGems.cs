using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectAddAddonGems : CardEffectIF
{
	[Export] GemAddonType gemAddonType;
	public CardEffectAddAddonGems()
	{

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Tile> tilesToEffect = matchBoard.getRandomNonBlackNonAddonTiles(getValue());
		matchBoard.addGemAddons(tilesToEffect.Select(x => x.getTilePosition()).ToList(), gemAddonType);
	}

}
