using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Schema;

public partial class Score : Node
{
	[Export] RichTextLabel scoreLabel;
	[Export] RichTextLabel multLabel;
	[Export] RichTextLabel turnsRemainingLabel;
	[Export] RichTextLabel moneyNeededLabel;
	[Export] RichTextLabel levelLabel;
	[Export] RichTextLabel coinsLabel;
	[Export] HBoxContainer colorUpgradePreviewBox;


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
	[Export] public ColorUpgrade colorUpgrade;

	
	private float muiltResetValue = 1.0f;
	private int score;
	private float mult = 1;
	private int turnsRemaining = 3;
	private int heartsRemaining = 2;
	private int level = 1;
	private int moneyNeeded = 500;
	private int coins;
	private List<ColorUpgrade> colorUpgrades = new List<ColorUpgrade>();
	private Dictionary<GemType, ColorUpgrade> colorUpgradesCompressed = new Dictionary<GemType, ColorUpgrade>();

	public override void _Ready()
	{
		if(colorUpgrade != null) {
			addColorUpgrade(colorUpgrade);
		}

		FindObjectHelper.getNewTurnButton(this).SetUpNewTurn += () => newturn();
		setMult(mult);
		setScore(score);
		setMoneyNeeded(moneyNeeded);
		setLevel(level);
		setTurnsRemaining(turnsRemaining);
		setHeartsRemaining(heartsRemaining);
		setCoins(coins);
	}

	public void newturn() {		
		setMult(muiltResetValue);
		if (score >= moneyNeeded) {
			setTurnsRemaining(turnsRemaining-1);
			audioStreamPlayer2D.Stream = victoryAudio;
			audioStreamPlayer2D.Play();
			GetTree().CreateTimer(0).Timeout += () => gameManager.evaluateLevel();
			return;
		}

		if (turnsRemaining >= 1 && turnsRemaining > 0) {
			setTurnsRemaining(turnsRemaining-1);
		} else {
			setHeartsRemaining(heartsRemaining-1);
			audioStreamPlayer2D.Stream = oofAudio;
			audioStreamPlayer2D.Play();
			gameManager.setHealth(heartsRemaining);
		}
		if (turnsRemaining == 0 && heartsRemaining == 0) {
			gameManager.evaluateLevel();
			//gameManager.nextLevel();
		}
	}

	public void setTurnsRemaining(int newValue) {
		turnsRemaining = newValue;
		turnsRemainingLabel.Text = "TURNS REMAINING:" + newValue;
		Godot.Collections.Array<Node> nodes = turnsContainer.GetChildren();
		for(int count = 0; count < nodes.Count; count ++) {
			if (count + 1 <= turnsRemaining ) {
				((TextureRect)nodes[count]).Modulate = turnColor;
			} else {
				((TextureRect)nodes[count]).Modulate = new Color(1,1,1);
			}
		}
	}

	public void setHeartsRemaining(int newValue) {
		heartsRemaining = newValue;
		Godot.Collections.Array<Node> nodes = heartsContainer.GetChildren();
		for(int count = 0; count < nodes.Count; count ++) {
			if (count + 1 <= heartsRemaining) {
				((TextureRect)nodes[count]).Modulate = heartColor;
			} else {
				((TextureRect)nodes[count]).Modulate = new Color(1,1,1);
			}
		}
	}

	private void compressCurrentColorUpgrades() {
		Dictionary<GemType, ColorUpgrade> colorUpgradesCompressedNew = new Dictionary<GemType, ColorUpgrade>();
		foreach(ColorUpgrade colorUpgrade in colorUpgrades) {
			if (colorUpgradesCompressedNew.ContainsKey(colorUpgrade.gemType)) {
				colorUpgradesCompressedNew[colorUpgrade.gemType] = colorUpgrade.add(colorUpgradesCompressedNew[colorUpgrade.gemType]);
			} else {
				colorUpgradesCompressedNew.Add(colorUpgrade.gemType, colorUpgrade);
			}
		}
		colorUpgradesCompressed = colorUpgradesCompressedNew;
	}

	public void setMoneyNeeded(int newValue) {
		moneyNeeded = newValue;
		moneyNeededLabel.Text = "MONEY NEEDED:" + (newValue / 100.0).ToString("C2");
	}

	public int getScore() { 
		return score;
	}

	public int getTurnsRemaining() {
		return turnsRemaining;
	}

	public void setCoins(int newValue) {
		coins = newValue;
		coinsLabel.Text = newValue.ToString();
	}

	public void setLevel(int newValue) {
		level = newValue;
		levelLabel.Text = "LEVEL:" + newValue;
	}

	public void setScore(int newScore) {
		score = newScore;
		scoreLabel.Text = "SCORE: " + (newScore / 100.0).ToString("C2");
		progressBar.Value = (float)score/moneyNeeded * 100;
	}

	public void setMult(float newMult) {
		mult = newMult;
		multLabel.Text = "Score Multiplier: " + newMult.ToString("0.##") + "x";
	}
	public void setColorUpgrades(List<ColorUpgrade> colorUpgrades) {
		this.colorUpgrades = colorUpgrades;
		compressCurrentColorUpgrades();
		renderColorUpgradePreviews();
	}

	public void addColorUpgrade(ColorUpgrade colorUpgrade) {
		colorUpgrades.Add(colorUpgrade);
		compressCurrentColorUpgrades();
		renderColorUpgradePreviews();

	}

	private void renderColorUpgradePreviews() {
		foreach(Node node in colorUpgradePreviewBox.GetChildren()) {
			node.QueueFree();
		}
		foreach(ColorUpgrade colorUpgrade in colorUpgradesCompressed.Values) {
			TextureRect texture2D = (TextureRect)colorUpgradeUI.Instantiate();
			texture2D.Modulate = colorUpgrade.gemType.GetColor();
			texture2D.TooltipText = "bonus score: " + colorUpgrade.baseIncrease + "\nbonus mult: " + colorUpgrade.multIncrease;
			GD.Print("making color upgrade preview " + colorUpgrade);
			colorUpgradePreviewBox.AddChild(texture2D);
		}
	}

	public void scoreMatches(HashSet<HashSet<Tile>> matches) {  
		int totalPoints = 0;
		foreach(HashSet<Tile> match in matches) {
			GemType gemType = extractGemTypeFromMatch(match);
			int pointValue = 0;
			if (gemType.matchable()) {
				pointValue = evaluatePoints(match, gemType);
				pointValue = (int)(pointValue * mult);
				setMult(mult + evaluateMultIncrease(gemType));
				totalPoints += pointValue;
			}
			
			MatchScoreText matchScore = (MatchScoreText)matchScoreTextPackedScene.Instantiate();
			matchScore.setText(pointValue.ToString());
			AddChild(matchScore);
			if (match.Count > 0) {
				matchScore.GlobalPosition = match.First().GlobalPosition;
			}
		}
		setScore(score + totalPoints);
	}

	private GemType extractGemTypeFromMatch(HashSet<Tile> tiles) {
		foreach(Tile tile in tiles) {
			if (tile.Gem.Type != GemType.Rainbow) {
				return tile.Gem.Type;
			}
		}
		return GemType.Rainbow;
	}

	private int evaluatePoints(HashSet<Tile> match, GemType gemType) {
		List<ColorUpgrade> upgradesForCurrentColor = colorUpgrades.Where(colorUpgrade => colorUpgrade.gemType == gemType).ToList();
		int pointUpgrade = upgradesForCurrentColor.Sum(colorUpgrade => colorUpgrade.baseIncrease);
		if (match.Count == 3) {
			return 100 + pointUpgrade;
		} else {
			return 100 + (match.Count - 3) * 50 + pointUpgrade;
		}
	}

	private float evaluateMultIncrease(GemType gemType) {
		List<ColorUpgrade> upgradesForCurrentColor = colorUpgrades.Where(colorUpgrade => colorUpgrade.gemType == gemType).ToList();
		return upgradesForCurrentColor.Sum(colorUpgrade => colorUpgrade.multIncrease) + multIncrement;
	}
}