using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

[GlobalClass, Tool]
public partial class EffectResource : Resource
{
	[Export]
	protected int value;
	[Export] public int BurnGems { get; private set; } = 0;
	[Export] public int EnergyGems { get; private set; } = 0;
	[Export] public int CardGems { get; private set; } = 0;
	[Export] public int CoinGems { get; private set; } = 0;
	[Export] public int DrawCards { get; private set; } = 0;
	[Export] public int GainEnergy { get; private set; } = 0;
	[Export] public int GainCoins { get; private set; } = 0;

	[Export] public float GainMult { get; private set; } = 0;
	public void execute(Node node) {
		executeEffect(node);

		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		Hand hand = FindObjectHelper.getHand(node);
		Mana mana = FindObjectHelper.getMana(node);
		Score score = FindObjectHelper.getScore(node);
		GameManagerIF gameManager = FindObjectHelper.getGameManager(node);

		
		if (matchBoard != null) {
			createAddonGems(matchBoard, GemAddonType.Mana, EnergyGems);
			createAddonGems(matchBoard, GemAddonType.Card, CardGems);
			createAddonGems(matchBoard, GemAddonType.Money, CoinGems);
			createBlackGems(matchBoard, new List<Vector2>(), BurnGems);
			hand.drawCards(DrawCards);
			mana.modifyMana(GainEnergy);
			score.addMult(GainMult);
			score.addCoins(GainCoins);
		}
	}

	protected virtual void executeEffect(Node node) {
		
	}


	private void createAddonGems(MatchBoard matchBoard, GemAddonType type, int count)
	{
		List<Tile> tiles = matchBoard.getRandomNonBlackNonAddonTiles(count);
		matchBoard.addGemAddons(tiles.Select(x => x.getTilePosition()).ToList(), type);
	}
	private void createBlackGems(MatchBoard matchBoard, List<Vector2> selectedTiles, int value)
	{
		List<Tile> tiles = matchBoard.getRandomNonBlackNonAddonTiles(value);
		if (tiles.Count != value)
		{
			tiles = matchBoard.getRandomNonBlackNonAddonTiles(value, selectedTiles.ToHashSet());
		}
		matchBoard.changeGemsColorAtPosition(tiles.Select(x => x.getTilePosition()).ToList(), GemType.Black);
	}
}
