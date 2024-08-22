using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Schema;

public partial class Score : Node2D
{
	[Export] RichTextLabel scoreLabel;
	[Export] RichTextLabel multLabel;
	[Export] RichTextLabel turnsRemainingLabel;
	[Export] RichTextLabel moneyNeededLabel;
	[Export] RichTextLabel levelLabel;
	[Export] RichTextLabel coinsLabel;
	[Export] RichTextLabel metaCoinsLabel;
	
	[Export] PackedScene explosion;

	[Export] HBoxContainer colorUpgradePreviewBox;
	[Export] Node2D progressBarLocation;
	[Export] Mult multUI;
	[Export] Color turnColor;
	[Export] Color heartColor;
	[Export] AudioPlayer audioStreamPlayer2D;
	[Export] AudioStream oofAudio;
	[Export] AudioStream coinAudio;
	[Export] PackedScene matchScoreTextPackedScene;
	[Export] private float multIncrement = .25f;
	[Export] TextureProgressBar progressBar;
	[Export] HBoxContainer turnsContainer;
	[Export] HBoxContainer heartsContainer;
	[Export] GameManager gameManager;
	[Export] PackedScene colorUpgradeUI;
	[Export] private RecipeUI bossRecipeUI;

	[Export] PackedScene heartUI;
	[Export] Texture2D heartFull;
	[Export] Texture2D heartEmpty;

	[Export] Texture2D turnFull;
	[Export] Texture2D turnEmpty;

	[Signal]
	public delegate void scoreChangeEventHandler(int score, int maxScore);

	[Signal]
	public delegate void multChangeEventHandler(float mult);
	[Signal]
	public delegate void multGainedEventHandler(float mult);

	private float muiltResetValue = 1.0f;
	private int score;
	private float mult = 1;
	private int turnsRemaining = 3;
	private int heartsRemaining = 2;
	private int maxHearts = 2;

	private int level = 1;
	private int scoreNeeded = 500;
	private int coins;
	private List<ColorUpgrade> colorUpgrades = new List<ColorUpgrade>();
	private Dictionary<GemType, ColorUpgrade> colorUpgradesCompressed = new Dictionary<GemType, ColorUpgrade>();

	private bool levelCleared = false;
	private bool levelLost = false;


	public override void _Ready()
	{
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		gameManagerIF.coinsChanged += (int coins) =>
		{
			setCoins(coins);
			playCoinSound();
		};
		gameManagerIF.metaCoinsChanged += (int coins) =>
		{
			setMetaCoins(coins);
			playCoinSound();
		};
		gameManagerIF.healthChanged += (int value) => setHeartsRemaining(value);


		FindObjectHelper.getNewTurnButton(this).TurnCleanUp += () => newturn();
		setMult(mult);
		setScore(score);
		setScoreNeeded(scoreNeeded);
		setLevel(level);
		setTurnsRemaining(turnsRemaining);
		setCoins(coins);
		setMetaCoins(gameManagerIF.getMetaCoins());

		maxHearts = gameManager.getMaxHealth();
		initHeartsContainer();
		setHeartsRemaining(gameManagerIF.getHealth());
	}

	public void newturn()
	{
		setMult(muiltResetValue);
		if (scoreReached())
		{
			setTurnsRemaining(turnsRemaining - 1);
			gameManager.evaluateLevel();
			return;
		}
		colorUpgrades = colorUpgrades.Where(colorUpgrade => !colorUpgrade.temporary).ToList();
		renderColorUpgradePreviews();

		if (turnsRemaining >= 1 && turnsRemaining > 0)
		{
			setTurnsRemaining(turnsRemaining - 1);
		}
		else
		{
			gameManager.setHealth(heartsRemaining - 1);
			playOofSound();
		}
	}

	public bool scoreReached()
	{
		return score >= scoreNeeded;
	}

	public int getScoreNeeded()
	{
		return scoreNeeded;
	}

	public void addToTurnsRemaining(int value)
	{

	}

	public void modifyTurnsRemaining(int modifyValue)
	{
		if (modifyValue > 0)
		{
			setTurnsRemaining(turnsRemaining + modifyValue);
		}
		if (modifyValue < 0)
		{
			if (turnsRemaining >= 1 && turnsRemaining > 0)
			{
				setTurnsRemaining(turnsRemaining - 1);
			}
			else
			{
				setHeartsRemaining(heartsRemaining - 1);
				playOofSound();
			}
			if (turnsRemaining == 0 && heartsRemaining == 0)
			{
				gameManager.evaluateLevel();
			}
		}
	}

	public void playOofSound() {
		audioStreamPlayer2D.Stream = oofAudio;
		audioStreamPlayer2D.setBaseVolumeMult(.5f);
		audioStreamPlayer2D.Play();
	}

	public void playCoinSound() {
		audioStreamPlayer2D.Stream = coinAudio;
		audioStreamPlayer2D.setBaseVolumeMult(1.0f);
		audioStreamPlayer2D.Play();
	}


	public void setTurnsRemaining(int newValue)
	{		
		int indexTurnBlowUp = -1;
		if (newValue < turnsRemaining) {
			indexTurnBlowUp = turnsRemaining -1;
		}
		turnsRemaining = newValue;
		turnsRemainingLabel.Text = "TURNS REMAINING:" + newValue;
		Godot.Collections.Array<Node> nodes = turnsContainer.GetChildren();
		for (int count = 0; count < nodes.Count; count++)
		{
			if (count == indexTurnBlowUp) {
				Explosion explosionNode = (Explosion)explosion.Instantiate();
				explosionNode.Position = new Vector2(20,20);
				explosionNode.Scale = new Vector2(.25f,.25f);
				explosionNode.GetChild<GpuParticles2D>(0,false).Emitting = true;
				nodes[count].AddChild(explosionNode);
			}
			if (count + 1 <= turnsRemaining)
			{
				((TextureRect)nodes[count]).Texture = turnFull;
			}
			else
			{
				((TextureRect)nodes[count]).Texture = turnEmpty;
			}
		}
	}

	public void setHeartsRemaining(int newValue)
	{
		int indexHeartBlowUp = -1;
		if (newValue < heartsRemaining) {
			indexHeartBlowUp = heartsRemaining -1;
		}
		heartsRemaining = Math.Min(newValue, maxHearts);
		Godot.Collections.Array<Node> nodes = heartsContainer.GetChildren();
		for (int count = 0; count < nodes.Count; count++)
		{
			if (count == indexHeartBlowUp) {
				Explosion explosionNode = (Explosion)explosion.Instantiate();
				explosionNode.Position = new Vector2(20,20);
				explosionNode.Scale = new Vector2(.25f,.25f);
				explosionNode.GetChild<GpuParticles2D>(0,false).Emitting = true;
				nodes[count].AddChild(explosionNode);
			}
			if (count + 1 <= heartsRemaining)
			{
				((TextureRect)nodes[count]).Texture = heartFull;
			}
			else
			{
				((TextureRect)nodes[count]).Texture = heartEmpty;
			}
		}
		if (heartsRemaining <= 0)
		{
			levelLost = true;
			gameManager.evaluateLevel();
		}
	}

	public void initHeartsContainer()
	{
		Godot.Collections.Array<Node> nodes = heartsContainer.GetChildren();
		foreach (Node node in nodes)
		{
			heartsContainer.RemoveChild(node);
			node.QueueFree();
		}

		for (int a = 0; a < maxHearts; a++)
		{
			heartsContainer.AddChild(heartUI.Instantiate());
		}
	}

	private void compressCurrentColorUpgrades()
	{
		Dictionary<GemType, ColorUpgrade> colorUpgradesCompressedNew = new Dictionary<GemType, ColorUpgrade>();
		foreach (ColorUpgrade colorUpgrade in colorUpgrades)
		{
			if (colorUpgradesCompressedNew.ContainsKey(colorUpgrade.gemType))
			{
				colorUpgradesCompressedNew[colorUpgrade.gemType] = colorUpgrade.add(colorUpgradesCompressedNew[colorUpgrade.gemType]);
			}
			else
			{
				colorUpgradesCompressedNew.Add(colorUpgrade.gemType, colorUpgrade);
			}
		}
		colorUpgradesCompressed = colorUpgradesCompressedNew;
	}

	public void setScoreNeeded(int newValue)
	{
		scoreNeeded = newValue;
		moneyNeededLabel.Text = "MONEY NEEDED:" + (newValue / 100.0).ToString("C2");
	}

	public int getScore()
	{
		return score;
	}

	public int getTurnsRemaining()
	{
		return turnsRemaining;
	}

	public void addCoins(int value)
	{
		coins += value;
		gameManager.addCoins(value);
		coinsLabel.Text = coins.ToString();
	}


	public void setCoins(int newValue)
	{
		coins = newValue;
		coinsLabel.Text = newValue.ToString();
	}


	public void setMetaCoins(int value)
	{
		metaCoinsLabel.Text = value.ToString();
	}


	public void setLevel(int newValue)
	{
		level = newValue;
		levelLabel.Text = "Level:" + newValue + "/10";
	}

	public void setScore(int newScore)
	{
		score = newScore;
		scoreLabel.Text = "Score: " + score + "/" + scoreNeeded;
		progressBar.Value = (float)score / scoreNeeded * 100;
		EmitSignal(SignalName.scoreChange, score, scoreNeeded);
		checkForLevelCleared();
	}

	public void setMult(float newMult)
	{
		float difference = newMult - mult;
		mult = newMult;
		multLabel.Text = "Score Multiplier: " + getMultText(newMult);
		if (multUI != null)
		{
			multUI.setMult(newMult);
		}
		if (difference > 0 ){
			EmitSignal(SignalName.multGained, difference);
		}
		EmitSignal(SignalName.multChange, mult);
	}

	public void addMult(float value)
	{
		setMult(mult + value);
	}

	public float getMult()
	{
		return mult;
	}

	public void setColorUpgrades(List<ColorUpgrade> colorUpgrades)
	{
		this.colorUpgrades = colorUpgrades;
		renderColorUpgradePreviews();
	}

	public void addColorUpgrade(ColorUpgrade colorUpgrade)
	{
		colorUpgrades.Add(colorUpgrade);
		renderColorUpgradePreviews();
	}

	private void renderColorUpgradePreviews()
	{
		compressCurrentColorUpgrades();

		if (colorUpgradePreviewBox != null)
		{
			foreach (Node node in colorUpgradePreviewBox.GetChildren())
			{
				node.QueueFree();
			}
		}
		foreach (ColorUpgrade colorUpgrade in colorUpgradesCompressed.Values)
		{
			TextureRect texture2D = (TextureRect)colorUpgradeUI.Instantiate();
			texture2D.Modulate = colorUpgrade.gemType.GetColor();

			texture2D.TooltipText = colorUpgrade.gemType.ToString();
			if (!colorUpgrade.baseIncrease.Equals(0) && !colorUpgrade.finalMult.Equals(0.0f))
			{
				texture2D.TooltipText += "\nbonus score: " + colorUpgrade.baseIncrease;
			}
			if (!colorUpgrade.multIncrease.Equals(0.0f))
			{
				texture2D.TooltipText += "\nbonus mult increase per match: " + colorUpgrade.multIncrease;
			}
			if (colorUpgrade.finalMult.Equals(0.0f))
			{
				texture2D.TooltipText += "\nno points given";
			}
			else if (!colorUpgrade.finalMult.Equals(1.0f))
			{
				texture2D.TooltipText = texture2D.TooltipText + "\nfinal mult: " + colorUpgrade.finalMult;
			}

			colorUpgradePreviewBox.AddChild(texture2D);
		}
	}


	public void addScoreFromNode(int scoreValue, Node node)
	{
		int pointValue = (int)(scoreValue * mult);
		MatchBoard board = FindObjectHelper.getMatchBoard(this);
		board.playMatchSound();
		if (node is Control control) {
			makeMatchText(control.GetScreenPosition(), scoreValue, pointValue);
		} else if (node is Node2D node2D) {
			makeMatchText(node2D.GlobalPosition, scoreValue, pointValue);
		}
		setScore(score + pointValue);
	}

	// public void addScoreFromNode(int scoreValue, Node2D card)
	// {
	// 	int pointValue = (int)(scoreValue * mult);
	// 	MatchBoard board = FindObjectHelper.getMatchBoard(this);
	// 	board.playMatchSound();
	// 	makeMatchText(card.GlobalPosition, scoreValue, pointValue);
	// 	setScore(score + pointValue);
	// }

	public void checkForLevelCleared()
	{
		if (scoreReached() && levelCleared == false)
		{
			gameManager.evaluateLevel();
			levelCleared = true;
		}
	}

	public bool isLevelOver() {
		if(levelCleared || levelLost){
			return true;
		}
		return false;
	}

	public void scoreMatchesWithPointOverride(HashSet<HashSet<Tile>> matches, Optional<int> pointValueOverride)
	{
		int totalPoints = 0;
		foreach (HashSet<Tile> match in matches)
		{
			GemType gemType = extractGemTypeFromMatch(match);
			int pointValue = 0;
			int pointValueAfterMult = 0;
			float newMultValue = mult;
			if (gemType.matchable())
			{
				bossRecipeUI.evaulateMatch(gemType);
				pointValue = evaluatePoints(match, gemType);

				pointValueAfterMult = (int)(pointValue * mult);
				newMultValue += evaluateMultIncrease(gemType);
				setMult(newMultValue);
				if (bossRecipeUI.Visible && !bossRecipeUI.complete())
				{
					pointValueAfterMult = 0;
				}
			}
			if (pointValueOverride.HasValue)
			{
				pointValueAfterMult = pointValueOverride.GetValue();
			}
			totalPoints += pointValueAfterMult;

			if (match.Count > 0)
			{
				makeMatchText(match.First().GlobalPosition, pointValue, pointValueAfterMult);
			}
		}
		setScore(score + totalPoints);
	}

	private void makeMatchText(Vector2 position, int pointValue, int pointValueAfterMult)
	{
		MatchScoreText matchScore = (MatchScoreText)matchScoreTextPackedScene.Instantiate();
		AddChild(matchScore);
		matchScore.GlobalPosition = position;
		matchScore.init(pointValue, pointValueAfterMult);
	}


	public void scoreMatches(HashSet<HashSet<Tile>> matches)
	{
		scoreMatchesWithPointOverride(matches, Optional.None<int>());
	}

	private GemType extractGemTypeFromMatch(HashSet<Tile> tiles)
	{
		foreach (Tile tile in tiles)
		{
			if (tile.Gem.Type != GemType.Rainbow)
			{
				return tile.Gem.Type;
			}
		}
		return GemType.Rainbow;
	}

	private int evaluatePoints(HashSet<Tile> match, GemType gemType)
	{
		// List<ColorUpgrade> upgradesForCurrentColor = colorUpgrades.Where(colorUpgrade => colorUpgrade.gemType == gemType).ToList();
		// int pointUpgrade = upgradesForCurrentColor.Sum(colorUpgrade => colorUpgrade.baseIncrease);
		// float finalMult = upgradesForCurrentColor.Sum (colorUpgrade => colorUpgrade.baseIncrease);
		int combo = 0;
		foreach (Tile tile in match)
		{
			combo += tile.Gem.getCombo();
		}
		combo = Math.Max(0, combo - 1);
		Optional<ColorUpgrade> colorUpgrade = getColorUpgrade(gemType);
		int pointUpgrade = 0;
		float finalMult = 1.0f;
		if (colorUpgrade.HasValue)
		{
			pointUpgrade = colorUpgrade.GetValue().baseIncrease;
			finalMult = colorUpgrade.GetValue().finalMult;
		}

		if (match.Count == 3)
		{
			return (int)((100 + pointUpgrade + combo * 50) * finalMult);
		}
		else
		{
			return (int)((100 + (match.Count - 3 + combo) * 50 + pointUpgrade) * finalMult);
		}
	}

	private float evaluateMultIncrease(GemType gemType)
	{
		Optional<ColorUpgrade> colorUpgrade = getColorUpgrade(gemType);
		if (colorUpgrade.HasValue)
		{
			return colorUpgrade.GetValue().multIncrease + multIncrement;
		}
		else
		{
			return multIncrement;
		}
	}

	public Optional<ColorUpgrade> getColorUpgrade(GemType gemType)
	{
		if (!colorUpgradesCompressed.ContainsKey(gemType))
		{
			return Optional.None<ColorUpgrade>();
		}
		return Optional.Some<ColorUpgrade>(colorUpgradesCompressed[gemType]);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("space"))
		{
			addMult(.25f);
			//GetTree().Paused = true;
		}
	}

	public String getMultText(float mult)
	{
		String waveText = "";
		if (mult >= 1.5)
		{
			waveText = "[wave amp=25.0 freq=2.0 connected=1]";
		}
		if (mult >= 2)
		{
			waveText = "[wave amp=50.0 freq=3.0 connected=1]";
		}
		if (mult >= 3)
		{
			waveText = "[wave amp=50.0 freq=6.0 connected=1]";
		}
		String rainbowText = "";
		if (mult >= 2)
		{
			rainbowText = "[rainbow freq=.5 sat=0.4 val=0.8]";
		}
		if (mult >= 3)
		{
			rainbowText = "[rainbow freq=.5 sat=0.8 val=0.8]";
		}
		return waveText + rainbowText + "x" + mult;
	}
}
