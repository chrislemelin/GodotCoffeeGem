using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectCashIn : CardEffectIF
{
	public CardEffectCashIn()
	{

	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.None;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		List<Tile> addonTiles = matchBoard.getTilesWithAddon(GemAddonType.Mana);
		addonTiles.AddRange(matchBoard.getTilesWithAddon(GemAddonType.Card));
		addonTiles.AddRange(matchBoard.getTilesWithAddon(GemAddonType.Money));
		foreach (Tile tile in addonTiles)
		{
			tile.Gem.doAddonEffect();
		}
		matchBoard.addGemAddons(addonTiles.Select(tile => tile.getTilePosition()).ToList(), GemAddonType.None);
	}
}
