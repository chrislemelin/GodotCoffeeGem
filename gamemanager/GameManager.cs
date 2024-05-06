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

	[Export] public GameOverScreen gameOverScreen;
	[Export] public Score score;
	[Export] public RelicHolderUI relicHolderUI;
	[Export] public Array<RelicResource> relicTestResources = new Array<RelicResource>();
	[Export] public Control bossRecipeTutorial;
	[Export] public Control gooTutorial;
	[Export] public WelcomeScreen welcomeTutorial;
	[Export] public RelicSelection relicSelection;
	[Export] public LevelListResource levelList;
	[Export] public RelicList bossRelics;
	[Export] public LevelCompleteUI levelComplete;
	[Signal] public delegate void levelOverEventHandler();
	[Signal] public delegate void levelStartEventHandler();
	[Export] private bool debugMode;
	[Export] private bool endlessMode;
	[Export] private bool skipFirstLevel;

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
			relicHolderUI.setRelicList(getRelics());
		}
		relicHolderUI.addRelic(getRandomBossRelic());
		relicHolderUI.startUpRelics();

		EmitSignal(SignalName.levelStart);
	}

	public void evaluateLevel()
	{
		if (score.getScore() < scoreNeededToPass)
		{
			FindObjectHelper.getFormSubmitter(this).submitData("loss", this);
			// its joever
			resetGlobals();
			int metaCoins = evaluateMetaCoins();
			gameOverScreen.setMetaCoins(metaCoins);
			addMetaCoins(metaCoins);
			gameOverScreen.Visible = true;
		}
		else
		{
			nextLevel();
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
		return bossRelics.allRelics[0];
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
		global.currentLevel += 1;
		GetTree().ChangeSceneToFile("res://mainScenes/Map.tscn");
	}
}
