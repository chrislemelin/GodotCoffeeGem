using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

public partial class Global : Node
{
	public const String LOAD_STRING = "/root/Globals/";
	private const String SAVE_FILE_NAME = "user://userId.save";

	private const String USER_ID_SAVE_NAME = "userId";
	private const String META_COINS_SAVE_NAME = "meatCoins";
	private const String MUSIC_VOLUME_SAVE_NAME = "musicVolum";
	private const String SFX_VOLUME_SAVE_NAME = "sfxVolume";
	private const String DEBT_SAVE_NAME = "debt";
	private const String TUTORIAL_SAVE_NAME = "seenTutorial";
	private const String NUMBER_OF_LEVELS_PLAYED = "numberOfLevelsPlayed";

	public CardList deckCardList = null;
	public Array<ColorUpgrade> colorUpgrades = new Array<ColorUpgrade>();
	public List<RelicResource> relics = new List<RelicResource>();
	public DeckSelectionResource deckSelection = null;

	public int debt = 500;
	public int numberOfLevelsPlayed = 0;
	public ulong timeStartedRun = 0;
	public bool zenMode = false;
	public int currentLevel = 1;
	public int currentHealth = 2;
	public int maxHealth = 2;
	public int currentCoins = 0;
	public int currentMetaCoins = 0;
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

	public void reset()
	{
		deckCardList = null;
		colorUpgrades = new Array<ColorUpgrade>();
		relics = new List<RelicResource>();
		deckSelection = null;

		timeStartedRun = 0;
		zenMode = false;

		currentLevel = 1;
		currentHealth = 2;
		maxHealth = 2;
		currentCoins = 0;
		allCoinsGained = 0;
		numberOfCardsToChoose = 3;
		shownBossTutorial = false;
		shownGooTutorial = false;
		gooRightRow = false;
		gridSize = new Vector2(6, 5);
	}

	public override void _Ready()
	{
		if (!FileAccess.FileExists(SAVE_FILE_NAME))
		{
			save();
		}
		else
		{
			load();
		}
	}

	public void save()
	{
		if (userId == -1) 
		{
			userId = GD.RandRange(1, Int32.MaxValue);
		}
		using var userIdSave = FileAccess.Open(SAVE_FILE_NAME, FileAccess.ModeFlags.Write);
		Godot.Collections.Dictionary<String, String> saveDict = new Godot.Collections.Dictionary<String, String>();
		saveDict.Add(USER_ID_SAVE_NAME, userId.ToString());
		saveDict.Add(META_COINS_SAVE_NAME, currentMetaCoins.ToString());
		saveDict.Add(SFX_VOLUME_SAVE_NAME, sfXvolume.ToString());
		saveDict.Add(MUSIC_VOLUME_SAVE_NAME, musicVolume.ToString());
		saveDict.Add(DEBT_SAVE_NAME, debt.ToString());
		saveDict.Add(TUTORIAL_SAVE_NAME, shownWelcomeTutorial.ToString());
		saveDict.Add(NUMBER_OF_LEVELS_PLAYED, numberOfCardsToChoose.ToString());

		var jsonString = Json.Stringify(saveDict);
		// Store the save dictionary as a new line in the save file.
		userIdSave.StoreLine(jsonString);

	}

	private void load()
	{
		using var saveGame = FileAccess.Open(SAVE_FILE_NAME, FileAccess.ModeFlags.Read);
		var jsonString = saveGame.GetLine();
		// Creates the helper class to interact with JSON
		var json = new Json();
		var parseResult = json.Parse(jsonString);
		if (parseResult != Error.Ok)
		{
			GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
			return;
		}
		Godot.Collections.Dictionary<String, String> nodeData = new Godot.Collections.Dictionary<String, String>((Godot.Collections.Dictionary)json.Data);

		userId = Int32.Parse(nodeData[USER_ID_SAVE_NAME]);
		if (nodeData.ContainsKey(META_COINS_SAVE_NAME))
		{
			currentMetaCoins = Int32.Parse(nodeData[META_COINS_SAVE_NAME]);
		}
		sfXvolume = tryLoadFloat(nodeData, SFX_VOLUME_SAVE_NAME, sfXvolume);
		musicVolume = tryLoadFloat(nodeData, MUSIC_VOLUME_SAVE_NAME, musicVolume);
		debt = tryLoadInt(nodeData, DEBT_SAVE_NAME, debt);
		shownWelcomeTutorial = tryLoadBool(nodeData, TUTORIAL_SAVE_NAME, shownWelcomeTutorial);
		numberOfLevelsPlayed = tryLoadInt(nodeData, NUMBER_OF_LEVELS_PLAYED, numberOfLevelsPlayed);
	}

	private float tryLoadFloat(Godot.Collections.Dictionary<String, String> nodeData, String name, float defaultValue) {
		if (nodeData.ContainsKey(name)) {
			return float.Parse(nodeData[name]);
		}
		return defaultValue;
	}
	private int tryLoadInt(Godot.Collections.Dictionary<String, String> nodeData, String name, int defaultValue) {
		if (nodeData.ContainsKey(name)) {
			return int.Parse(nodeData[name]);
		}
		return defaultValue;
	}

	private bool tryLoadBool(Godot.Collections.Dictionary<String, String> nodeData, String name, bool defaultValue) {
		if (nodeData.ContainsKey(name)) {
			return bool.Parse(nodeData[name]);
		}
		return defaultValue;
	}
}
