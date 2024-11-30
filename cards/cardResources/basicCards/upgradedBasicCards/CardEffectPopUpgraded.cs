using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]

public partial class CardEffectPopUpgraded : CardEffectIF
{
	public CardEffectPopUpgraded() {
		 
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Vector2 selectedTile = selectedTiles[0];
		Gem gemToDelete = matchBoard.getTile(selectedTile).Gem;
		GemAddonType gemAddonType = GemAddonType.None;
		if (gemToDelete.AddonType != GemAddonType.None) {
			gemAddonType = gemToDelete.AddonType;
		}

		matchBoard.deleteGemAtPosition(selectedTile);
		
		if(gemAddonType != GemAddonType.None) {
			List<Tile> addonTiles = matchBoard.getRandomNonSpecialNonAddonTiles(2);
			matchBoard.addGemAddons(addonTiles.Select(x => x.getTilePosition()).ToList(), gemAddonType);
		}
	}


}
