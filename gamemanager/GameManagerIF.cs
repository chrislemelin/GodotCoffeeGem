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
		return new List<CardResource>(global.deckCardList.allCards);
	}

	public void addCardToDeckList(CardResource cardResource)
	{
		if (cardResource == null) {
			return;
		}
		global.deckCardList.allCards.Add(cardResource);
	}

	public void removeCardFromDeckList(CardResource cardResource)
	{
		global.deckCardList.allCards.Remove(cardResource);
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
		if (getGlobal().deckCardList == null || getGlobal().deckCardList.allCards.Count == 0)
		{
			Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(defaultCardList.allCards);
			CardList newCardList = new CardList();
			newCardList.allCards = cardList;
			getGlobal().deckCardList = newCardList;
		}
	}

	public void resetGlobals()
	{
		Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(defaultCardList.allCards);
		getGlobal().deckCardList.allCards = cardList;
		getGlobal().currentLevel = 1;
		getGlobal().currentHealth = 2;
		getGlobal().relics = new List<RelicResource>();
		getGlobal().shownBossTutorial = false;

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
