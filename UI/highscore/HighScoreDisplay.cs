using Godot;
using System;
using System.Collections.Generic;

public partial class HighScoreDisplay : Control
{
	[Export] PackedScene scoreText;
	[Export] PackedScene scoreInput;

	[Export] Control scoreHolder;
	List<Tuple<String, int>> scores;

	private LineEdit nameInput;
	private int scoreNaming;


	public override void _Ready() {
		scores = FindObjectHelper.getGameManager(this).getHighScores();
		renderScore();
		addHighScore(100);
	}

	private void renderScore() {
		foreach(Node node in scoreHolder.GetChildren()) {
			node.QueueFree();
		}

		foreach(Tuple<String, int> score in scores) {
			Control control = (Control)scoreText.Instantiate();
			((RichTextLabel)control.GetChild(0)).Text = score.Item1;
			((RichTextLabel)control.GetChild(1)).Text = score.Item2 + "";
			scoreHolder.AddChild(control);
		}
	}

	public void addHighScore(int score) {
		scoreNaming = score;
   		for(int scoreCount = 0; scoreCount < scores.Count; scoreCount++) {
			if (score > scores[scoreCount].Item2) {
				Control control = (Control)scoreInput.Instantiate();
				((RichTextLabel)control.GetChild(1)).Text = score + "";
				nameInput = (LineEdit)control.GetChild(0);
				nameInput.TextSubmitted+= enteredName;
				scoreHolder.AddChild(control);
				scoreHolder.MoveChild(control, scoreCount);
				break;
			}
		}

	}

	public void enteredName(String text) {
		FindObjectHelper.getGameManager(this).submitScore(new Tuple<string, int>(text, scoreNaming));
		renderScore();
		GetTree().CreateTimer(3).Timeout += () =>  Visible = false;
	}
}
