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
	private const String GAMEPLAY_RECORDING_TUTORIAL_NAME = "seenGameplayCollectionTutorial";
	private const String TUTORIAL_OVERTIME_SAVE_NAME = "seenOvertimeTutorial";
	private const String SEEN_BOSS_DIALOUGE_SAVE_NAME = "seenBossDialouge";
	private const String NUMBER_OF_LEVELS_PLAYED = "numberOfLevelsPlayed";
	private const String COLLECTION_GAMEPLAY_DATA_SAVE_NAME = "collectGameplayData";
	private const String NUMBER_OF_RUNS_PLAYED = "numberOfRunsPlayed";
	private const String LEADER_BOARD_SCORE = "highscores";

	public CardList deckCardList = null;
	public Array<ColorUpgrade> colorUpgrades = new Array<ColorUpgrade>();
	public List<RelicResource> relics = new List<RelicResource>();
	public List<ActivatableRelicResource> activableRelics = new List<ActivatableRelicResource>();
	public DeckSelectionResource deckSelection = null;

	public Vector2 gridSize = new Vector2(6, 5);
	public int gridUpgrades = 0;
	public int debtMax = 10_000;
	public int debt = 10_000;
	public int numberOfLevelsPlayed = 0;
	public int numberOfRunsPlayed = 0;
	public ulong timeStartedRun = 0;
	public bool zenMode = false;
	public int currentLevel = 1;
	public int currentHealth = 3;
	public int maxHealth = 3;
	public int currentCoins = 0;
	public int currentMetaCoins = 0;
	public int allCoinsGained = 0;
	public int totalScore = 0;
	public int numberOfCardsToChoose = 3;
	public int numberOfCardsInShop = 2;
	public bool shownBossTutorial = false;
	public bool shownBossDialouge = false;
	public bool shownWelcomeTutorial = false;
	public bool shownGameplayCollectionTutorial = false;
	public bool shownOvertimeTutorial = false;
	public bool shownGooTutorial = false;
	public bool gooRightRow = false;
	public bool collectData = true;
	public int userId = -1;
	public float musicVolume = .5f;
	public float sfXvolume = .5f;

	private int highScoreMax = 8;
	// private List<Tuple<String,int>> highScores = new List<Tuple<string, int>>() 
	// {new Tuple<string, int>("Chris L", 10_000_000),
	//  new Tuple<string, int>("HP", 5_000_000),
	//  new Tuple<string, int>("Casper", 1_000_000),
	//  new Tuple<string, int>("Clyde", 100_000)};

	private List<RunResource> highScoresNew = new List<RunResource>(){
		new RunResource("Chris L", 1_000_000, true),
		new RunResource("HP",500_000, true),
		new RunResource("Casper",100_000, true),
		new RunResource("Clyde",50_000, true),
		new RunResource("Bob",25_000, true),
		new RunResource("Joe",10_000, true)

	};

	private List<RunResource> highScoresDefault = new List<RunResource>(){
		new RunResource("Chris L", 1_000_000, true),
		new RunResource("HP",500_000, true),
		new RunResource("Casper",100_000, true),
		new RunResource("Clyde",50_000, true),
		new RunResource("Bob",25_000, true),
		new RunResource("Joe",10_000, true)
	};


	public void reset(MetaGlobal metaGlobal)
	{
		colorUpgrades = new Array<ColorUpgrade>();
		relics = new List<RelicResource>();
		activableRelics = new List<ActivatableRelicResource>();


		timeStartedRun = 0;
		zenMode = false;

		gridUpgrades = 0;
		currentLevel = 1;
		currentHealth = 3;
		maxHealth = 3;
		currentCoins = metaGlobal.getStartingCoinsValue();
		allCoinsGained = metaGlobal.getStartingCoinsValue();
		numberOfCardsInShop = 2 + metaGlobal.cardsInShopBonus;
		totalScore = 0;
		numberOfCardsToChoose = 3;
		shownBossTutorial = false;
		shownGooTutorial = false;
		gooRightRow = false;
		gridSize = new Vector2(6, 5);
	}

	public override void _Ready()
	{
		//save();
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
		saveDict.Add(GAMEPLAY_RECORDING_TUTORIAL_NAME, shownGameplayCollectionTutorial.ToString());
		saveDict.Add(NUMBER_OF_LEVELS_PLAYED, numberOfLevelsPlayed.ToString());
		saveDict.Add(TUTORIAL_OVERTIME_SAVE_NAME, shownOvertimeTutorial.ToString());
		saveDict.Add(NUMBER_OF_RUNS_PLAYED, numberOfRunsPlayed.ToString());
		saveDict.Add(SEEN_BOSS_DIALOUGE_SAVE_NAME, shownBossDialouge.ToString());
		saveDict.Add(COLLECTION_GAMEPLAY_DATA_SAVE_NAME, collectData.ToString());
		for(int scoreCount = 0; scoreCount < highScoresNew.Count; scoreCount++) {
			saveDict.Add(LEADER_BOARD_SCORE + scoreCount, highScoresNew[scoreCount].toJson());
		} 

		var jsonString = Json.Stringify(saveDict);
		// Store the save dictionary as a new line in the save file.
		userIdSave.StoreLine(jsonString);
	}

	public void resetMetaProgression(){
		highScoresNew.Clear();
		highScoresNew.AddRange(highScoresDefault);
		debt = debtMax;

		save();
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
		shownGameplayCollectionTutorial = tryLoadBool(nodeData, GAMEPLAY_RECORDING_TUTORIAL_NAME, shownGameplayCollectionTutorial);
		shownOvertimeTutorial = tryLoadBool(nodeData, TUTORIAL_OVERTIME_SAVE_NAME, shownOvertimeTutorial);
		numberOfLevelsPlayed = tryLoadInt(nodeData, NUMBER_OF_LEVELS_PLAYED, numberOfLevelsPlayed);
		numberOfRunsPlayed = tryLoadInt(nodeData, NUMBER_OF_RUNS_PLAYED, numberOfRunsPlayed);
		shownBossDialouge = tryLoadBool(nodeData, SEEN_BOSS_DIALOUGE_SAVE_NAME, shownBossDialouge);
		collectData = tryLoadBool(nodeData, COLLECTION_GAMEPLAY_DATA_SAVE_NAME, collectData);

		int scoreCount = 0;
		//highScoresNew.Clear();
		while (true) {
			String key = LEADER_BOARD_SCORE + scoreCount++;
			if (nodeData.ContainsKey(key)) {
				RunResource run = loadScoreFromString(nodeData[key]);
				if (run != null) {
					if (scoreCount == 1)
					{
						highScoresNew.Clear();
					}
					highScoresNew.Add(loadScoreFromString(nodeData[key]));
				}
			} else {
				break;
			}
		} 
	}

	private void saveHighScore() {
		
	}

	public void addNewHighScore(RunResource newHighScore) {
		newHighScore = new RunResource(newHighScore.name.Replace(",",""), newHighScore.score, newHighScore.completed);
		bool addedToScore = false;
		for(int scoreCount = 0; scoreCount < highScoresNew.Count; scoreCount++) {
			if (newHighScore.score > highScoresNew[scoreCount].score) {
				highScoresNew.Insert(scoreCount, newHighScore);
				addedToScore = true;
				break;
			}
		}
		if (!addedToScore) {
			highScoresNew.Add(newHighScore);
		}
		if (highScoresNew.Count > highScoreMax) {
			highScoresNew.RemoveAt(highScoresNew.Count -1);
		}
		save(); 
	}

	public bool isScoreAHighScore(int value) {
		if (highScoresNew.Count < highScoreMax) {
			return true;
		}
		if (value > highScoresNew[highScoresNew.Count-1].score) {
			return true;
		}
		return false;
	}

	public List<RunResource> getHighScores() {
		return highScoresNew;
	}
	
	private String getScoreString(Tuple<String, int> value) {
		return value.Item1 +","+ value.Item2.ToString();
	}

	private RunResource loadScoreFromString(String value) {
		var json = new Json();
		var parseResult = json.Parse(value);
		if (parseResult != Error.Ok)
		{
			GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {value} at line {json.GetErrorLine()}");
			return null;
		}
		return RunResource.fromJson(json);
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
