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

	public ulong timeStartedRun = 0;
	public bool zenMode = false;

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
	public int userId = -1;
	public float musicVolume = .5f;
	public float sfXvolume = .5f;




	public Vector2 gridSize = new Vector2(6, 5);

}
