using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

public partial class Score : Node
{
	[Export] RichTextLabel scoreLabel;
	[Export] RichTextLabel multLabel;
	[Export] RichTextLabel moneyLabel;

	[Export] RichTextLabel turnsRemainingLabel;
	[Export] RichTextLabel moneyNeededLabel;
	[Export] RichTextLabel levelLabel;
	[Export] AudioStreamPlayer2D audioStreamPlayer2D;
	[Export] PackedScene matchScoreTextPackedScene;
	[Export] private float multIncrement = .25f;

	[Export] GameManager gameManager;
	
	private float muiltResetValue = 1.0f;

	private int money;
	private int score;
	private float mult = 1;
	private int turnsRemaining = 5;
	private int level = 1;
	private int moneyNeeded = 500;

	public override void _Ready()
	{
		GetNode<Button>("%NewTurnButton").Pressed += () => newturn();
		setMoney(money);
		setMult(mult);
		setScore(score);
		setMoneyNeeded(moneyNeeded);
		setLevel(turnsRemaining);
		setTurnsRemaining(turnsRemaining);

	}

	public void newturn() {		
		setMoney(score + score);
		if (score != 0) {
			audioStreamPlayer2D.Play();
		}
		setScore(0);
		setMult(muiltResetValue);
		setTurnsRemaining(turnsRemaining-1);
		if (turnsRemaining == 0 || money >= moneyNeeded) {
			gameManager.advanceOrGameOver(money);
			//gameManager.nextLevel();
		}
	}

	public void setTurnsRemaining(int newValue) {
		turnsRemaining = newValue;
		turnsRemainingLabel.Text = "TURNS REMAINING:" + newValue;
	}

	public void setMoneyNeeded(int newValue) {
		moneyNeeded = newValue;
		moneyNeededLabel.Text = "MONEY NEEDED:" + (newValue / 100.0).ToString("C2");
	}

	public void setLevel(int newValue) {
		level = newValue;
		levelLabel.Text = "LEVEL:" + newValue;
	}

	public void setMoney(int newMoney) {
		money = newMoney;
		moneyLabel.Text = "TOTAL MONEY:" + (newMoney / 100.0).ToString("C2");
	}

	public void setScore(int newScore) {
		score = newScore;
		scoreLabel.Text = "MONEY: " + (newScore / 100.0).ToString("C2");
	}

	public void setMult(float newMult) {
		mult = newMult;
		multLabel.Text = "MULT:" + newMult.ToString("0.##") + "x";
	}

	public void scoreMatches(HashSet<HashSet<Tile>> matches) {  
		int totalPoints = 0;
		foreach(HashSet<Tile> match in matches) {
			GemType gemType = match.First().Gem.Type;
			int pointValue = 0;
			if (GemTypeHelper.getsPointsFromMatching(gemType)) {
				pointValue = evaluateMatch(match);
				pointValue = (int)(pointValue * mult);
				setMult(mult + multIncrement);
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

	private int evaluateMatch(HashSet<Tile> match) {
		if (match.Count == 3) {
			return 100;
		} else {
			return 100 + (match.Count - 3) * 5;
		}
	}
}
