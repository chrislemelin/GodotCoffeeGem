using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : GameManagerIF
{
	int scoreNeededToPass;

	[Export] public NewCardSelection newCardSelection;
	[Export] public RecipeUI recipeUI;
	[Export] Control UICover;

	[Export] public GameOverScreen gameOverScreen;
	[Export] public Score score;
	[Export] public RelicHolderUI relicHolderUI;
	[Export] public Array<RelicResource> relicTestResources = new Array<RelicResource>();
	[Export] public ToggleVisibilityOnButtonPress bossRelicTutorial;
	[Export] public Control bossRecipeTutorial;
	[Export] public Control gooTutorial;
	[Export] public WelcomeScreen welcomeTutorial;
	[Export] public RelicSelection relicSelection;
	[Export] public LevelListResource levelList;
	[Export] public RelicList bossRelics;
	[Export] public LevelCompleteUI levelComplete;
	[Export] public ToggleVisibilityOnButtonPress debtDisplay;
	[Signal] public delegate void levelOverEventHandler();
	[Signal] public delegate void levelStartEventHandler();
	[Export] private bool debugMode;
	[Export] private bool endlessMode;
	[Export] private bool skipFirstLevel;
	[Export] private Godot.Collections.Dictionary<int, Resource> dialougeDict = new Godot.Collections.Dictionary<int, Resource>();

	[Export] Resource dialougeResource;

	private int currentLevel;
	private LevelResource currentLevelResource;

	public override void _Ready()
	{
		base._Ready();
		currentLevel = global.currentLevel;
		currentLevelResource = levelList.levelResources[currentLevel - 1];
		if (currentLevel == 1)
		{
			setStartTime();
		}
		RecipeResource bossRecipe = currentLevelResource.getBossRecipe();
		if (bossRecipe != null)
		{
			recipeUI.loadRecipe(bossRecipe);
			bossRecipeTutorial.Visible = true;

		}
		if (!getGlobal().shownWelcomeTutorial && !debugMode)
		{
			welcomeTutorial.startUp();
			getGlobal().shownWelcomeTutorial = true;
			getGlobal().save();
		}
		if (!getGlobal().shownGooTutorial && currentLevelResource.blockedTiles > 0 && !debugMode)
		{
			gooTutorial.Visible = true;
			getGlobal().shownGooTutorial = true;
		}

		scoreNeededToPass = currentLevelResource.score;
		if (debugMode)
		{
			scoreNeededToPass = 50;
			addCoins(100);
		}
		if (endlessMode || getGlobal().zenMode)
		{
			scoreNeededToPass = 999999999;
			score.setTurnsRemaining(999999999);
		}
		if (skipFirstLevel && currentLevel == 1)
		{
			nextLevel();
		}
		score.setScoreNeeded(scoreNeededToPass);
		score.setLevel(currentLevel);
		score.setHeartsRemaining(global.currentHealth);
		score.setCoins(getCoins());
		score.setColorUpgrades(getColorUpgrades().ToList());
		if (relicTestResources.Count != 0)
		{
			foreach (RelicResource relicResource in relicTestResources)
			{
				addRelic(relicResource);
			}
		}
		FindObjectHelper.getMatchBoard(this).addRandomBlockedTiles(currentLevelResource.blockedTiles);
		relicHolderUI.setRelicList(getRelics());

		if (currentLevelResource.generateRandomBossRelic)
		{
			RelicResource bossRelic = getRandomBossRelic();
			relicHolderUI.addRelic(bossRelic);
			bossRelicTutorial.richTextLabel.Text += bossRelic.description;
			bossRelicTutorial.Visible = true;
		}
		EmitSignal(SignalName.levelStart);
		relicHolderUI.startUpRelics();
		debtDisplay.richTextLabel.Text += "$" + getDebt();

		//EmitSignal(SignalName.levelStart);
		//startDialouge(dialougeResource);

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

	public void startDialouge(Resource dialougeResource)
	{
		DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(dialougeResource);
	}

	public void evaluateLevel()
	{
		if (score.getScore() < scoreNeededToPass)
		{
			FindObjectHelper.getFormSubmitter(this).submitData("loss", this);
			unlockNewCards();

			// its joever
			resetGlobals();
			int metaCoins = evaluateMetaCoins();
			changeDebt(-metaCoins);
			gameOverScreen.setMetaCoins(metaCoins);
			addMetaCoins(metaCoins);
			gameOverScreen.Visible = true;
		}
		else
		{
			nextLevel();
		}
	}
	
	private void unlockNewCards() {
		List<UnlockableCardPack> lockedPacks = getLockedCardPacks().ToList();
		if (lockedPacks.Count > 0) {
			UnlockableCardPack unlockedPack = lockedPacks[0];
			GD.Print(unlockedPack.title);
			FindObjectHelper.getDeckView(this).setUp(unlockedPack.getCards(), null, TextHelper.centered("New Cards Unlocked"));
			unlockCardPack(unlockedPack);
		}
	}

	private int evaluateMetaCoins()
	{
		if (currentLevel == levelList.levelResources.Count)
		{
			return 20;
		}
		return currentLevel + Math.Max(0, currentLevel - 5);
	}

	public void nextLevel()
	{
		EmitSignal(SignalName.levelOver);
		setRelics(getRelics().Where(relic => !relic.lastForOneLevel).ToList());
		//newCardSelection.windowClosed += (CardResource) => advanceLevel();
		if (currentLevel == levelList.levelResources.Count)
		{
			FindObjectHelper.getFormSubmitter(this).submitData("win", this);
			resetGlobals();
			gameOverScreen.label.Text = "You win!!!";
			int metaCoins = evaluateMetaCoins();
			gameOverScreen.setMetaCoins(metaCoins);
			addMetaCoins(metaCoins);
			gameOverScreen.Visible = true;
			return;
		}

		if (currentLevelResource.getBossRecipe() != null || currentLevelResource.makeRandomBossRecipe)
		{
			levelComplete.WindowClosedSignal += () => selectRandomRelic();
		}
		else
		{
			levelComplete.WindowClosedSignal += () => advanceLevel();
		}
		int coinsGained = 20 + Math.Max(0, score.getTurnsRemaining()) * 10;
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
		relicSelection.setRelics(relicsInSelection);
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
		// 	DialogueManagerRuntime.DialogueManager.DialogueEnded += loadNewScene;
		// 	DialogueManagerRuntime.DialogueManager.ShowDialogueBalloon(dialougeMaybe.GetValue());
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
