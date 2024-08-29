using Godot;
using System;
using System.Collections.Generic;

public partial class HighScoreDisplay : ToggleVisibilityOnButtonPress
{
	[Export] PackedScene scoreText;
	[Export] PackedScene scoreInput;

	[Export] Control scoreHolder;
	List<Tuple<String, int>> scores;

	private LineEdit nameInput;
	private int scoreNaming;


	public override void _Ready() {
		base._Ready();
		button.Visible = false;
		scores = FindObjectHelper.getGameManager(this).getHighScores();
		renderScore();
		//addHighScore(100);
	}	

	private void renderScore() {
		foreach(Node node in scoreHolder.GetChildren()) {
			node.QueueFree();
		}

		foreach(Tuple<String, int> score in scores) {
			Control control = (Control)scoreText.Instantiate();
			((RichTextLabel)control.GetChild(0)).Text = score.Item1;
			((RichTextLabel)control.GetChild(1)).Text = score.Item2.ToString("N0");
			scoreHolder.AddChild(control);
		}
	}

	public void addHighScore(int score) {
		scoreNaming = score;
   		for(int scoreCount = 0; scoreCount < scores.Count; scoreCount++) {
			if (score > scores[scoreCount].Item2) {
				Control control = (Control)scoreInput.Instantiate();
				((RichTextLabel)control.GetChild(1)).Text = score.ToString("N0");
				nameInput = (LineEdit)control.GetChild(0);
				nameInput.TextSubmitted+= enteredName;
				scoreHolder.AddChild(control);
				scoreHolder.MoveChild(control, scoreCount);
				return;
			}
		}
		Control control2 = (Control)scoreInput.Instantiate();
		((RichTextLabel)control2.GetChild(1)).Text = score.ToString("N0");
		nameInput = (LineEdit)control2.GetChild(0);
		nameInput.TextSubmitted+= enteredName;
		scoreHolder.AddChild(control2);
	}

	public void enteredName(String text) {
		FindObjectHelper.getGameManager(this).submitScore(new Tuple<string, int>(text, scoreNaming));
		renderScore();
		button.Visible = true;
	}
}
