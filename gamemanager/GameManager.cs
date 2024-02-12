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
	[Export] public Array<LevelResource> levels;
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
		int currentLevel = global.currentLevel;
		if (currentLevel > levels.Count) {
			// you win
			resetGlobals();
			gameOverScreen.label.Text = "You win!!!";
			gameOverScreen.Visible = true;
		}
		currentLevelResource = levels[currentLevel-1];
		RecipeResource bossRecipe = currentLevelResource.getBossRecipe();
		if (bossRecipe != null) {
			recipeUI.loadRecipe(bossRecipe);
			bossRecipeTutorial.Visible = true;
			
		}
		if (!getGlobal().shownWelcomeTutorial && !debugMode) {
			welcomeTutorial.startUp();
			getGlobal().shownWelcomeTutorial = true;
		}
		if (!getGlobal().shownGooTutorial && currentLevelResource.blockedTiles > 0 && !debugMode) {
			gooTutorial.Visible = true;
			getGlobal().shownGooTutorial = true;
		}


		scoreNeededToPass = currentLevelResource.score;
		if (debugMode)
		{
			scoreNeededToPass = 50;
			addCoins(100);
		}
		if (endlessMode) {
			scoreNeededToPass = 50000;
			score.setTurnsRemaining(100);
		}
		if(skipFirstLevel && currentLevel == 1) {
			nextLevel();
		}
		score.setMoneyNeeded(scoreNeededToPass);
		score.setLevel(currentLevel);
		score.setHeartsRemaining(global.currentHealth);
		score.setCoins(getCoins());
		score.setColorUpgrades(global.colorUpgrades.ToList());
		if (relicTestResources.Count != 0)
		{
			foreach (RelicResource relicResource in relicTestResources)
			{
				addRelic(relicResource);
			}
		}
		FindObjectHelper.getMatchBoard(this).addRandomBlockedTiles(currentLevelResource.blockedTiles);
		relicHolderUI.setRelicList(getRelics());
		relicHolderUI.startUpRelics();

		EmitSignal(SignalName.levelStart);
	}

	

	public void evaluateLevel()
	{
		if (score.getScore() < scoreNeededToPass)
		{
			// its joever
			resetGlobals();
			gameOverScreen.Visible = true;
		}
		else
		{
			nextLevel();
		}
	}

	public void nextLevel()
	{
		EmitSignal(SignalName.levelOver);
		newCardSelection.windowClosed += (CardResource) => advanceLevel();

		int coinsGained = 20 + Math.Max(0, score.getTurnsRemaining()) * 10;
		levelComplete.setCoinsGained(coinsGained);
		if (currentLevel == levels.Count - 1) {
			resetGlobals();
			gameOverScreen.label.Text = "You win!!!";
			gameOverScreen.Visible = true;
		}

		if (currentLevelResource.getBossRecipe() != null || currentLevelResource.makeRandomBossRecipe) {
			levelComplete.continueButton.Pressed += () => selectRandomRelic();
		} else {
			levelComplete.continueButton.Pressed += () => advanceLevel();
		}
		levelComplete.Visible = true;
		addCoins(coinsGained);
	}

	private void selectRandomRelic() {
		List<RelicResource> relicResources = getRelicPool();
		RandomHelper.Shuffle(relicResources);
		List<RelicResource> relicsInSelection = relicResources.GetRange(0,Math.Min(3,relicResources.Count));
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
