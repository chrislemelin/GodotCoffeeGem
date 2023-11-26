using Godot;
using Godot.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectPopRandom : CardEffectIF
{
	public CardEffectPopRandom() {
	}

	public override SelectionType getSelectionType()
	{
		return SelectionType.Single;
	}


	public override void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{
		Tile tile = matchBoard.getTile(selectedTiles[0]);
		List<Vector2> positions = getAllTilesToEffect(matchBoard, tile);
		RandomHelper.Shuffle(positions);
		if(positions.Count == Value) {
			hand.drawCards(1);
			mana.modifyMana(1);
		}
		if(positions.Count > Value) {
			positions = positions.GetRange(0, Value);
		}
		matchBoard.deleteGemAtPositions(positions);
	}

	public override List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile) {
		List<Tile> tiles = matchBoard.getTilesWithColorOfGem(tile.Gem.Type);
		return tiles.Select(x => x.getPosition()).ToList();
	}
}
