using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManagerIF : Node2D
{

	protected Global global;
	[Export] public CardList cardPool;
	[Export] public RelicList relicList;
	[Export] public CardList defaultCardList;


	[Signal]
	public delegate void healthChangedEventHandler(int newHealth);

	[Signal]
	public delegate void coinsChangedEventHandler(int newCoins);
	public List<CardResource> getDeckList()
	{
		loadGlobalAndSetDeckToDefault();
		return new List<CardResource>(global.deckCardList.getCards());
	}

	public void addCardToDeckList(CardResource cardResource)
	{
		if (cardResource == null) {
			return;
		}
		global.deckCardList.getCards().Add(cardResource);
	}

	public void removeCardFromDeckList(CardResource cardResource)
	{
		global.deckCardList.getCards().Remove(cardResource);
	}

	public override void _Ready()
	{
		loadGlobalAndSetDeckToDefault();
	}

	private void loadGLobal()
	{
		if (global == null)
		{
			global = GetNode<Global>(Global.LOAD_STRING);
		}
	}


	protected void loadGlobalAndSetDeckToDefault()
	{
		if (defaultCardList != null){
			if (getGlobal().deckCardList == null || getGlobal().deckCardList.getCards().Count == 0)
			{
				Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(defaultCardList.getCards());
				CardList newCardList = new CardList();
				newCardList.setCards(cardList);
				getGlobal().deckCardList = newCardList;
			}
		}
	}

	public void resetGlobals()
	{
		Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(defaultCardList.getCards());
		getGlobal().deckCardList.setCards(cardList);
		getGlobal().currentLevel = 1;
		getGlobal().currentHealth = 2;
		getGlobal().relics = new List<RelicResource>();
		getGlobal().shownBossTutorial = false;
	}

	public void setCardList(List<CardResource> cardResources)
	{
		Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(cardResources);
		CardList newCardList = new CardList();
		newCardList.setCards(cardList);
		getGlobal().deckCardList = newCardList;
	}

	public void addCardToGlobalDeckAndAdvanceLevel(CardResource cardResource)
	{
		addCardToDeckList(cardResource);
		advanceLevel();
	}

	public List<RelicResource> getRelicPool()
	{
		return relicList.allRelics.Duplicate().ToList();
	}

	public virtual void advanceLevel()
	{
	}

	public void setHealth(int newHealth)
	{
		getGlobal().currentHealth = Math.Clamp(newHealth, 0, getGlobal().maxHealth);
		EmitSignal(SignalName.healthChanged, getGlobal().currentHealth);
	}

	public int getHealth()
	{
		return getGlobal().currentHealth;
	}

	public int getNumberOfCardToChoose()
	{
		return getGlobal().numberOfCardsToChoose;
	}

	public void setNumberOfCardToChoose(int value)
	{
		getGlobal().numberOfCardsToChoose = value;
	}

	public void setMaxHealth(int newHealth)
	{
		getGlobal().maxHealth = newHealth;
	}

	public int getMaxHealth()
	{
		return getGlobal().maxHealth;
	}

	public bool getGooRightRow()
	{
		return getGlobal().gooRightRow;
	}

	public void setGooRightRow(bool value)
	{
		getGlobal().gooRightRow = value;
	}


	public int getCoins()
	{
		return getGlobal().currentCoins;
	}

	public Vector2 getGridSize()
	{
		return getGlobal().gridSize;
	}

	public void changeGridSize(Vector2 newSize)
	{
		getGlobal().gridSize = newSize;
	}


	public List<RelicResource> getRelics()
	{
		return getGlobal().relics;
	}

	public void addRelic(RelicResource relicResource)
	{
		relicResource.executeAddedToInvEffects(this);
		getGlobal().relics.Add((RelicResource)relicResource.Duplicate());
	}

	public void syncRelics(List<RelicResource> relicResources)
	{
		getGlobal().relics = relicResources;
	}

	public void addCoins(int coinValue)
	{
		getGlobal().currentCoins += coinValue;
		EmitSignal(SignalName.coinsChanged, global.currentCoins);
	}

	public void addColorUpgrade(ColorUpgrade colorUpgrade)
	{
		getGlobal().colorUpgrades.Add(colorUpgrade);
	}

	protected Global getGlobal()
	{
		loadGLobal();
		return global;
	}
}
