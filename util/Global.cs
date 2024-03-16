using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

public partial class Global : Node
{
	public const String LOAD_STRING = "/root/Globals/";
	public CardList deckCardList = null;
	public Array<ColorUpgrade> colorUpgrades = new Array<ColorUpgrade>();
	public List<RelicResource> relics = new List<RelicResource>();
	public DeckSelectionResource deckSelection = null;

	public int currentLevel = 1;
	public int currentHealth = 2;
	public int maxHealth = 2;
	public int currentCoins = 0;
	public int allCoinsGained = 0;
	public int numberOfCardsToChoose = 3;
	public bool shownBossTutorial = false;
	public bool shownWelcomeTutorial = false;
	public bool shownGooTutorial = false;
	public bool gooRightRow = false;
	public bool collectData = true;



	public Vector2 gridSize = new Vector2(6,5);


}
