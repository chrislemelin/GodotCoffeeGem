using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : GameManagerIF
{

	int moneyNeededToPass;

	[Export] public NewCardSelection newCardSelection;
	[Export] public GameOverScreen gameOverScreen;
	[Export] public Score score;
	[Export] public RelicHolderUI relicHolderUI;
	[Export] public Array<RelicResource> relicResources;
	[Export] private bool debugMode;

	public override void _Ready()
	{
		base._Ready();
		int currentLevel = global.currentLevel;
	
		moneyNeededToPass = 500 + 500 * (currentLevel - 1);
		if (debugMode) {
			moneyNeededToPass = 50;
			addCoins(100);
		}
		score.setMoneyNeeded(moneyNeededToPass);
		score.setLevel(currentLevel);
		score.setHeartsRemaining(global.currentHealth);
		score.setCoins(getCoins());
		score.setColorUpgrades(global.colorUpgrades.ToList());
		if (getRelics().Count == 0) {
			foreach(RelicResource relicResource in relicResources) {
				addRelic(relicResource);
			}
		}
		relicHolderUI.setRelicList(getRelics());
		relicHolderUI.startUpRelics();
	}
	

	public void evaluateLevel() {
		if (score.getScore() < moneyNeededToPass) {
			// its joever
			resetGlobals();
			gameOverScreen.Visible = true;
		} else {
			nextLevel();
		}
	}

	public void nextLevel() {
		int coinsGained = 20 + Math.Max(0,score.getTurnsRemaining()) * 10;
		addCoins(coinsGained);
		int cardsToChoose = getNumberOfCardToChoose();
		List<CardResource> cardPoolList = new List<CardResource>(cardPool.allCards);
		RandomHelper.Shuffle(cardPoolList);
		newCardSelection.setCardsToSelectFrom(cardPoolList.GetRange(0,cardsToChoose));
		newCardSelection.setCoins(coinsGained);
	}

	public override void advanceLevel() {
		global.currentLevel += 1;
		GetTree().ChangeSceneToFile("res://mainScenes/Home.tscn");
	}
}
