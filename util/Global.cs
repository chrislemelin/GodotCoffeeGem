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
	private const String TUTORIAL_OVERTIME_SAVE_NAME = "seenOvertimeTutorial";

	private const String NUMBER_OF_LEVELS_PLAYED = "numberOfLevelsPlayed";
	private const String LEADER_BOARD_SCORE = "leaderboardScore";

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
	public int totalScore = 0;
	public int numberOfCardsToChoose = 3;
	public bool shownBossTutorial = false;
	public bool shownWelcomeTutorial = false;
	public bool shownOvertimeTutorial = false;
	public bool shownGooTutorial = false;
	public bool gooRightRow = false;
	public bool collectData = true;
	public int userId = -1;
	public float musicVolume = .5f;
	public float sfXvolume = .5f;

	private int highScoreMax = 8;
	private List<Tuple<String,int>> highScores = new List<Tuple<string, int>>() 
	{new Tuple<string, int>("Chris L", 10_000_000),
	 new Tuple<string, int>("HAP", 5_000_000),
	 new Tuple<string, int>("Casper", 1_000_000),
	 new Tuple<string, int>("Clyde", 100_000)};


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
		//addNewHighScore(new Tuple<string, int>("your mom", 1));
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
		saveDict.Add(TUTORIAL_OVERTIME_SAVE_NAME, shownOvertimeTutorial.ToString());
		for(int scoreCount = 0; scoreCount < highScores.Count; scoreCount++) {
			saveDict.Add(LEADER_BOARD_SCORE + scoreCount,getScoreString(highScores[scoreCount]));
		} 

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
		shownOvertimeTutorial = tryLoadBool(nodeData, TUTORIAL_OVERTIME_SAVE_NAME, shownOvertimeTutorial);
		numberOfLevelsPlayed = tryLoadInt(nodeData, NUMBER_OF_LEVELS_PLAYED, numberOfLevelsPlayed);
		
		int scoreCount = 0;
		highScores.Clear();
		while (true) {
			String key = LEADER_BOARD_SCORE + scoreCount++;
			if (nodeData.ContainsKey(key)) {
				highScores.Add(loadScoreFromString(nodeData[key]));
			} else {
				break;
			}
		} 
	}

	private void saveHighScore() {
		
	}

	public void addNewHighScore(Tuple<String, int> newHighScore) {
		newHighScore = new Tuple<string, int>(newHighScore.Item1.Replace(",",""), newHighScore.Item2);
		bool addedToScore = false;
		for(int scoreCount = 0; scoreCount < highScores.Count; scoreCount++) {
			if (newHighScore.Item2 > highScores[scoreCount].Item2) {
				highScores.Insert(scoreCount, newHighScore);
				addedToScore = true;
				break;
			}
		}
		if (!addedToScore) {
			highScores.Add(newHighScore);
		}
		if (highScores.Count > highScoreMax) {
			highScores.RemoveAt(highScores.Count -1);
		}
		save(); 
	}

	public bool isScoreAHighScore(int value) {
		if (highScores.Count < highScoreMax) {
			return true;
		}
		if (value > highScores[highScores.Count-1].Item2) {
			return true;
		}
		return false;
	}

	public List<Tuple<String,int>> getHighScores() {
		return highScores;
	}
	
	private String getScoreString(Tuple<String, int> value) {
		return value.Item1 +","+ value.Item2.ToString();
	}

	private Tuple<String,int> loadScoreFromString(String value) {
		String [] stringArray = value.Split(",");
		return new Tuple<String, int>(stringArray[0], int.Parse(stringArray[1]));
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
