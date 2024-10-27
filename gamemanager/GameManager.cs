using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : GameManagerIF
{
	int scoreNeededToPass;
	[Export] RelicResource testRelic;

	[Export] public NewCardSelection newCardSelection;
	[Export] public GameOverScreen gameOverScreen;
	[Export] public Score score;
	[Export] public RelicHolderUI relicHolderUI;
	[Export] public HighScoreDisplay highScoreDisplay;
	[Export] public Array<RelicResource> relicTestResources = new Array<RelicResource>();
	[Export] public ToggleVisibilityOnButtonPress bossRelicTutorial;
	[Export] public ToggleVisibilityOnButtonPress overTimeTutorial;
	[Export] public ToggleVisibilityOnButtonPress gooTutorial;
	[Export] public ToggleVisibilityOnButtonPress gameBeatenTutorial;
	[Export] public Resource gameBeatenBossDialouge;
	[Export] public Resource gameLostBossDialouge;
	[Export] public WelcomeScreen welcomeTutorial;
	[Export] public RelicSelection relicSelection;
	[Export] public LevelListResource levelList;
	[Export] public RelicList bossRelics;
	[Export] public LevelCompleteUI levelComplete;
	[Export] public ToggleVisibilityOnButtonPress debtDisplay;
	[Export] public AudioPlayer levelCompleteAudioPlayer;
	[Signal] public delegate void levelOverEventHandler();
	[Signal] public delegate void levelStartEventHandler();
	[Export] private bool debugMode;
	[Export] private bool endlessMode;
	[Export] private bool skipFirstLevel;
	[Export] private Godot.Collections.Dictionary<int, Resource> dialougeDict = new Godot.Collections.Dictionary<int, Resource>();
	[Export] Resource dialougeResource;
	[Export] Resource startingDialougeResource;
	[Export] Resource bossLevelDialougeResource;


	private int currentLevel;
	private LevelResource currentLevelResource;

	public override void _Ready()
	{
		base._Ready();

		GetViewport().GuiFocusChanged += (thing) => {
			GD.Print(thing.GetPath());
		};

		
		MatchBoard matchboard = FindObjectHelper.getMatchBoard(this);
		Hand hand = FindObjectHelper.getHand(this);


		currentLevel = global.currentLevel;
		currentLevelResource = levelList.levelResources[currentLevel - 1];
		if (currentLevel == 1)
		{
			resetGlobals();
			setStartTime();
		}
		RecipeResource bossRecipe = currentLevelResource.getBossRecipe();
		if (getNumberOfLevelsPlayed() == 0 && startingDialougeResource != null) {
			hand.forceNotHaveUIFocus = true;
			DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(startingDialougeResource);
			bool hasFired = false;
			DialogueManagerRuntime.DialogueManager.DialogueEnded += (resource) => {
				if (!hasFired) {
					hand.forceNotHaveUIFocus = false;
					hand.setUIFocus(true);
					hasFired = true;
				}
				
			};
			//DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(startingDialougeResource);
		}
		if (!getGlobal().shownGooTutorial && currentLevelResource.blockedTiles > 0)
		{
			gooTutorial.setVisible(true);
			getGlobal().shownGooTutorial = true;
			hand.setUIFocus(false);
			gooTutorial.WindowClosedSignal += () => hand.setUIFocus(true);
		}

		scoreNeededToPass = currentLevelResource.score;
		if (debugMode && currentLevel != 10)
		{
			scoreNeededToPass = 50;
			addCoins(200);
		}
		if (endlessMode || getGlobal().zenMode)
		{
			scoreNeededToPass = 9999999;
			score.setTurnsRemaining(999999999);
		}
		if (skipFirstLevel && currentLevel == 1)
		{
			nextLevel();
		}
		score.setScoreNeeded(scoreNeededToPass);
		score.setLevel(currentLevel);
		//score.setHeartsRemaining(global.currentHealth);
		score.setCoins(getCoins());
		score.setColorUpgrades(getColorUpgrades().ToList());
		if (relicTestResources.Count != 0 && getRelics().Count == 0)
		{
			foreach (RelicResource relicResource in relicTestResources)
			{
				//GD.Print("added relic");
				addRelic(relicResource);
			}
		}
		FindObjectHelper.getMatchBoard(this).addRandomBlockedTiles(currentLevelResource.blockedTiles);

		if (currentLevelResource.generateRandomBossRelic)
		{
			RelicResource bossRelic = getRandomBossRelic();
			addRelic(bossRelic);
			bossRelicTutorial.WindowClosedSignal += () => hand.setUIFocus(true);
			bossRelicTutorial.richTextLabel.Text += bossRelic.description;
			hand.setUIFocus(false);
			if (!getSeenBossDialouge()) {
				bool hasFired = false;
				DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(bossLevelDialougeResource);
				DialogueManagerRuntime.DialogueManager.DialogueEnded += (resource) => {
				if (resource == bossLevelDialougeResource && !hasFired) {
					bossRelicTutorial.setVisible(true);
					hasFired = true;
				}
				setSeenBossDialouge(true);
			};
			} else {
				bossRelicTutorial.setVisible(true);
			}

		}

		relicHolderUI.setRelicList(getRelics());
		relicHolderUI.startUpRelics();
		EmitSignal(SignalName.levelStart);

		score.levelPassed += checkForOverTimeTutorial;
		score.levelPassed += checkForGameOver;


		//EmitSignal(SignalName.levelStart);
		if (dialougeResource!= null) {
			hand.forceNotHaveUIFocus = true;
			DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(dialougeResource);
			DialogueManagerRuntime.DialogueManager.DialogueEnded += (resource) => {
				hand.forceNotHaveUIFocus = true;
				hand.setUIFocus(true);
			};
		}

	}

	private void checkForOverTimeTutorial () {
		if (currentLevel == 1 && !getGlobal().shownOvertimeTutorial) {
			overTimeTutorial.setVisible(true);
			getGlobal().shownOvertimeTutorial = true;
			getGlobal().save();
		}
	}

	private void checkForGameOver () {
		if (gameBeaten()) {
			gameBeatenTutorial.setVisible(true);
		}
	}

	private Optional<Resource> getDialouge()
	{
		int value = getNumberOfLevelsPlayed();
		if (dialougeDict.ContainsKey(value))
		{
			return Optional.Some(dialougeDict[value]);
		}
		else
		{
			return Optional.None<Resource>();
		}
	}

	public bool gameBeaten() {
		//return true;
		return currentLevel == levelList.levelResources.Count;
	}

	public void evaluateLevel()
	{
		getGlobal().totalScore += score.getScore();

		if (score.getScore() < scoreNeededToPass)
		{
			showGameOverScreen(false);
		}
		else
		{
			nextLevel();
		}
	}

	private bool checkForHighScore(bool gameCompleted) {
		if (getGlobal().isScoreAHighScore(getGlobal().totalScore)) {
			if (gameCompleted) {
				DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(gameBeatenBossDialouge);
			} else {
				DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(gameLostBossDialouge);
			}

			highScoreDisplay.Visible = true;
			highScoreDisplay.addHighScore(getGlobal().totalScore, gameCompleted);
			return true;
		}
		return false;
	}
	
	private Optional<UnlockableCardPack> getNewCardPack() {
		return Optional.None<UnlockableCardPack>();
		// List<UnlockableCardPack> lockedPacks = getLockedCardPacks().ToList();
		// if (lockedPacks.Count > 0) {
		// 	UnlockableCardPack unlockedPack = lockedPacks[0];
		// 	return Optional.Some<UnlockableCardPack>(unlockedPack);
		// 	//FindObjectHelper.getDeckView(this).setUp(unlockedPack.getCards(), null, TextHelper.centered("New Cards Unlocked"));
		// 	//unlockCardPack(unlockedPack);
		// }
		// return Optional.None<UnlockableCardPack>();
	}

	private int evaluateMetaCoins()
	{
		if (currentLevel == levelList.levelResources.Count)
		{
			return 20;
		}
		return currentLevel + Math.Max(0, currentLevel - 5);
	}

	private int evaluateDebtProgress()
	{
		if (currentLevel == levelList.levelResources.Count || debugMode)
		{
			return 500;
		}
		return currentLevel * 25 + Math.Max(0, currentLevel - 5) * 25;
	}

	private void showGameOverScreen(bool gameBeaten) {
		string formSubmitterText = gameBeaten?"win":"lose";
		FindObjectHelper.getFormSubmitter(this).submitData(formSubmitterText, this);

		if (gameBeaten) {
			gameOverScreen.label.Text = TextHelper.centered("You win!!!");
		}
		
		Optional<UnlockableCardPack> unlockableCardPack = getNewCardPack();
		if (unlockableCardPack.HasValue) {
			gameOverScreen.restartButton.Pressed+= () => {
			FindObjectHelper.getDeckView(this).setUp(unlockableCardPack.GetValue().getCards(), (card) => GetTree().ChangeSceneToFile("res://mainScenes/MainMenu.tscn"), TextHelper.centered("New Cards Unlocked"), false, false);
			unlockCardPack(unlockableCardPack.GetValue());
		};
		} else {
			gameOverScreen.setUpMainMenu();
		}

		bool highScore = checkForHighScore(gameBeaten);

		// int metaCoins = evaluateMetaCoins();
		// gameOverScreen.setMetaCoins(metaCoins);
		// addMetaCoins(metaCoins);

		int debtProgress = evaluateDebtProgress();
		int currentDebtProgress = getDebtProgress();
		if (highScore) {
			highScoreDisplay.WindowClosedSignal += () => {
				int segementsPassed = gameOverScreen.setDebt(currentDebtProgress, debtProgress);
				addMetaCoins(segementsPassed * 50);
			};
		} else {
			int segementsPassed = gameOverScreen.setDebt(currentDebtProgress, debtProgress);
			addMetaCoins(segementsPassed * 50);
		}
		changeDebt(-debtProgress);

		gameOverScreen.Visible = true;

		resetGlobals();

		return;
	}
	/*
	FindObjectHelper.getFormSubmitter(this).submitData("loss", this);
	Optional<UnlockableCardPack> unlockableCardPack = getNewCardPack();

	int metaCoins = evaluateMetaCoins();
	changeDebt(-metaCoins * 10);
	gameOverScreen.setMetaCoins(metaCoins);

	int debtProgress = evaluateDebtProgress();
	gameOverScreen.setDebt(getDebtProgress(), getDebtProgress());
	addMetaCoins(metaCoins);
	

	if (unlockableCardPack.HasValue) {
		gameOverScreen.restartButton.Pressed+= () => {
			FindObjectHelper.getDeckView(this).setUp(unlockableCardPack.GetValue().getCards(), (card) => GetTree().ChangeSceneToFile("res://mainScenes/MainMenu.tscn"), TextHelper.centered("New Cards Unlocked"), false, false);
			unlockCardPack(unlockableCardPack.GetValue());
		};
	} else {
		gameOverScreen.setUpMainMenu();
	}
	checkForHighScore(false);
	gameOverScreen.setVisible(true);
	resetGlobals();
	*/

	public void nextLevel()
	{
		levelCompleteAudioPlayer.Play();
		EmitSignal(SignalName.levelOver);
		setRelics(getRelics().Where(relic => !relic.lastForOneLevel).ToList());

		if (gameBeaten())
		{
			showGameOverScreen(true);
		}

		if (currentLevelResource.getBossRecipe() != null || currentLevelResource.makeRandomBossRecipe)
		{
			levelComplete.WindowClosedSignal += () => selectRandomRelic();
		}
		else
		{
			levelComplete.WindowClosedSignal += () => advanceLevel();
		}
		int coinsGained = score.calculateCoinsGained();
		levelComplete.setCoinsGained(coinsGained);
		addCoins(coinsGained);
	}

	private RelicResource getRandomBossRelic()
	{
		List<RelicResource> relicList = bossRelics.allRelics.ToList();
		RandomHelper.Shuffle(relicList);
		return relicList[0];
	}

	private void selectRandomRelic()
	{
		List<RelicResource> relicResources = getRelicPool();
		RandomHelper.Shuffle(relicResources);
		List<RelicResource> relicsInSelection = relicResources.GetRange(0, Math.Min(3, relicResources.Count));
		relicsInSelection.Add(testRelic);
		relicSelection.setRelics(relicsInSelection, true);
		relicSelection.WindowClosed += () => advanceLevel();
		relicSelection.Visible = true;
	}
	public override void advanceLevel()
	{
		incrementNumberOfLevelsPlayed();
		// Used for dialoge at the end of the level
		// Optional<Resource> dialougeMaybe = getDialouge();
		// if (dialougeMaybe.HasValue)
		// {
		// 	
		// 	;
		// }
		// else
		// {
		// 	loadNewScene();
		// }
		loadNewScene();
	}
	private void loadNewScene()
	{
		global.currentLevel += 1;
		GetTree().ChangeSceneToFile("res://mainScenes/Map.tscn");
	}

	protected override void Dispose(bool disposing)
	{
		//DialogueManagerRuntime.DialogueManager.DialogueEnded -= loadNewScene;
		base.Dispose(disposing);
	}

}
