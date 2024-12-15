using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

[GlobalClass, Tool]
public partial class EffectResource : Resource
{

	[Export]
	protected bool skipEmitSignalOnInfusions = false;
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

	public virtual bool canRunNotInLevel() {
		return false;
	}


	private void createAddonGems(MatchBoard matchBoard, GemAddonType type, int count)
	{
		List<Tile> tiles = matchBoard.getRandomNonSpecialNonAddonTiles(count);
		if (skipEmitSignalOnInfusions) {
			matchBoard.addGemAddonsDontFireEvent(tiles.Select(x => x.getTilePosition()).ToList(), type);
		} else {
			matchBoard.addGemAddons(tiles.Select(x => x.getTilePosition()).ToList(), type);
		}

	}
	private void createBlackGems(MatchBoard matchBoard, List<Vector2> selectedTiles, int value)
	{
		List<Tile> tiles = matchBoard.getRandomNonSpecialNonAddonTiles(value);
		if (tiles.Count != value)
		{
			tiles = matchBoard.getRandomNonSpecialNonAddonTiles(value, selectedTiles.ToHashSet());
		}
		matchBoard.changeGemsColorAtPosition(tiles.Select(x => x.getTilePosition()).ToList(), GemType.Black);
	}

	public virtual String getDescription() {
		String description = "";
		if (BurnGems > 0) {
			description += " +"+BurnGems+TextHelper.getIngredientIconString(GemType.Black)+".";
		}
		if (EnergyGems > 0) {
			description += " +"+EnergyGems+TextHelper.getEnergyIconString()+" infusion(s).";
		}
		if (CardGems > 0) {
			description += " +"+CardGems+TextHelper.getCardIconString()+" infusion(s).";
		}
		if (CoinGems > 0) {
			description += " +"+CoinGems+TextHelper.getCoinIconString()+" infusion(s).";
		}

		if (GainEnergy > 0) {
			description += " +"+GainEnergy+TextHelper.getEnergyIconString()+".";
		}
		if (DrawCards > 0) {
			description += " +"+DrawCards+TextHelper.getCardIconString()+".";
		}
		if (GainCoins > 0) {
			description += " +"+GainCoins+TextHelper.getCoinIconString()+".";
		}
		return description;
	}
}
