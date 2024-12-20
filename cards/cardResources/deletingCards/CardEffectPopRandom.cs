using Godot;
using Godot.Bridge;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectPopRandom : CardEffectIF
{
	[Export] private Array<EffectResource> bonusEffectsThree;
	[Export] private Array<EffectResource> bonusEffectsSixPlus;

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
		if(positions.Count == getValue()) {
			matchBoard.GetTree().CreateTimer(.6).Timeout += () => {
				foreach(EffectResource effectResource in bonusEffectsThree) {
					effectResource.execute(matchBoard);
				}
			};
		}
		if(positions.Count > getValue()) {
			positions = positions.GetRange(0, getValue());
		}
		//this should only be for the pop-call card, should move into its own card at some point
		matchBoard.deleteGemAtPositions(positions, true);
		if (positions.Count >= 6) {
			foreach(EffectResource effectResource in bonusEffectsSixPlus) {
					effectResource.execute(node);
				}
		}
	}

	public override List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile) {
		List<Tile> tiles = matchBoard.getTilesWithColorOfGem(tile.Gem.Type);
		return tiles.Select(x => x.getTilePosition()).ToList();
	}
}
