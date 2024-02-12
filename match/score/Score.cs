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
	[Export] HBoxContainer colorUpgradePreviewBox;
	[Export] Node2D progressBarLocation;
	[Export] Mult multUI;
	[Export] Color turnColor;
	[Export] Color heartColor;

	[Export] AudioStreamPlayer2D audioStreamPlayer2D;
	[Export] AudioStream victoryAudio;
	[Export] AudioStream oofAudio;

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
	public delegate void multChangeEventHandler(float muilt);

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

	public override void _Ready()
	{
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		gameManagerIF.coinsChanged += (int coins) => setCoins(coins);

		FindObjectHelper.getNewTurnButton(this).TurnCleanUp += () => newturn();
		setMult(mult);
		setScore(score);
		setMoneyNeeded(scoreNeeded);
		setLevel(level);
		setTurnsRemaining(turnsRemaining);
		setCoins(coins);

		maxHearts = gameManager.getMaxHealth();
		initHeartsContainer();
		setHeartsRemaining(2);
	}

	public void newturn()
	{
		setMult(muiltResetValue);
		if (scoreReached())
		{
			setTurnsRemaining(turnsRemaining - 1);
			audioStreamPlayer2D.Stream = victoryAudio;
			audioStreamPlayer2D.Play();
			GetTree().CreateTimer(0).Timeout += () => gameManager.evaluateLevel();
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
			setHeartsRemaining(heartsRemaining - 1);
			audioStreamPlayer2D.Stream = oofAudio;
			audioStreamPlayer2D.Play();
			gameManager.setHealth(heartsRemaining);
		}
		if (turnsRemaining == 0 && heartsRemaining == 0)
		{
			gameManager.evaluateLevel();
			//gameManager.nextLevel();
		}
	}

	public bool scoreReached() {
		return score >= scoreNeeded;
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
				audioStreamPlayer2D.Stream = oofAudio;
				audioStreamPlayer2D.Play();
				gameManager.setHealth(heartsRemaining);
			}
			if (turnsRemaining == 0 && heartsRemaining == 0)
			{
				gameManager.evaluateLevel();
			}
		}
	}


	public void setTurnsRemaining(int newValue)
	{
		turnsRemaining = newValue;
		turnsRemainingLabel.Text = "TURNS REMAINING:" + newValue;
		Godot.Collections.Array<Node> nodes = turnsContainer.GetChildren();
		for (int count = 0; count < nodes.Count; count++)
		{
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
		heartsRemaining = newValue;
		Godot.Collections.Array<Node> nodes = heartsContainer.GetChildren();
		for (int count = 0; count < nodes.Count; count++)
		{
			if (count + 1 <= heartsRemaining)
			{
				((TextureRect)nodes[count]).Texture = heartFull;
			}
			else
			{
				((TextureRect)nodes[count]).Texture = heartEmpty;
			}
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

	public void setMoneyNeeded(int newValue)
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

	public void setLevel(int newValue)
	{
		level = newValue;
		levelLabel.Text = "Level:" + newValue;
	}

	public void setScore(int newScore)
	{
		score = newScore;
		scoreLabel.Text = "Score: " + score + "/" + scoreNeeded;
		progressBar.Value = (float)score / scoreNeeded * 100;
		EmitSignal(SignalName.scoreChange, score, scoreNeeded);
	}

	public void setMult(float newMult)
	{
		mult = newMult;
		multLabel.Text = "Score Multiplier: " + getMultText(newMult);
		if (multUI != null) {
			multUI.setMult(newMult);
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

		if (colorUpgradePreviewBox != null) {
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

	public void addScore(int scoreValue)
	{
		int pointValue = (int)(scoreValue * mult);
		setScore(score + pointValue);
	}

	public void scoreMatches(HashSet<HashSet<Tile>> matches)
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
				if(bossRecipeUI.Visible && !bossRecipeUI.complete()) {
					pointValueAfterMult = 0;
				}
				totalPoints += pointValueAfterMult;
			
			}

			MatchScoreText matchScore = (MatchScoreText)matchScoreTextPackedScene.Instantiate();
			AddChild(matchScore);

			if (match.Count > 0)
			{
				matchScore.GlobalPosition = match.First().GlobalPosition;
				matchScore.setText(pointValueAfterMult.ToString());
				//matchScore.init(pointValue, pointValueAfterMult, newMultValue, ToLocal(multUI.GlobalPosition), progressBarLocation.Position, this, multUI);
			}
		}
		setScore(score + totalPoints);
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
			//addMult(.25f);
			//GetTree().Paused = true;

		}
	}

	public String getMultText(float mult) {
		String waveText = "";
		if (mult >= 1.5) {
			waveText = "[wave amp=25.0 freq=2.0 connected=1]";
		} if (mult >= 2) {
			waveText = "[wave amp=50.0 freq=3.0 connected=1]";
		} if (mult >= 3) {
			waveText = "[wave amp=50.0 freq=6.0 connected=1]";
		}
		String rainbowText = "";
		if (mult >= 2) {
			rainbowText = "[rainbow freq=.5 sat=0.4 val=0.8]";
		}
		if (mult >= 3) {
			rainbowText = "[rainbow freq=.5 sat=0.8 val=0.8]";
		}
		return waveText + rainbowText +"x" + mult;
	}
}
