using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{

	Global global;
	int currentLevel;
	int moneyNeededToPass;

	[Export] public CardList cardPool;
	[Export] public CardList defaultCardList;
	[Export] public NewCardSelection newCardSelection;
	[Export] public GameOverScreen gameOverScreen;
	[Export] public Score score;


	public List<CardResource> getDeckList (){
		loadGlobalAndSetDeckToDefault();
		return new List<CardResource>(global.deckCardList.allCards);
	}

	public override void _Ready()
	{
		loadGlobalAndSetDeckToDefault();
		currentLevel = global.currentLevel;
		moneyNeededToPass = 500 + 500 * (currentLevel - 1);
		score.setMoneyNeeded(moneyNeededToPass);
		score.setLevel(currentLevel);
	}

	public void advanceOrGameOver(int currentMoney) {
		if (currentMoney <= moneyNeededToPass) {
			// its joever
			resetGlobals();
			gameOverScreen.Visible = true;
		} else {
			nextLevel();
		}
	}

	private void loadGlobalAndSetDeckToDefault() {
		if (global == null) {
			global = GetNode<Global>(Global.LOAD_STRING);
		}		
		if (global.deckCardList == null || global.deckCardList.allCards.Count == 0) {
			Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(defaultCardList.allCards);
			CardList newCardList = new CardList();
			newCardList.allCards = cardList;
			global.deckCardList = newCardList;
		}
	}

	public void resetGlobals(){
		Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(defaultCardList.allCards);
		global.deckCardList.allCards = cardList;
		global.currentLevel = 1;
	}

	public void nextLevel() {
		int cardsToChoose = 4;
		List<CardResource> cardPoolList = new List<CardResource>(cardPool.allCards);
		RandomHelper.Shuffle(cardPoolList);
		newCardSelection.setCardsToSelectFrom(cardPoolList.GetRange(0,cardsToChoose));
	}

	public void addCardToGlobalDeck(CardResource cardResource) {
		global.deckCardList.allCards.Add(cardResource);
		global.currentLevel += 1;
		GetTree().ReloadCurrentScene();
	}
}
