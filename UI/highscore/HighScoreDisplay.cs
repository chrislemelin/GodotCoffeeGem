using Godot;
using System;
using System.Collections.Generic;

public partial class HighScoreDisplay : ToggleVisibilityOnButtonPress
{
	[Export] PackedScene scoreText;
	[Export] PackedScene scoreInput;

	[Export] Control scoreHolder;
	List<RunResource> scores;

	private LineEdit nameInput;
	private int scoreValue;
	private bool scoreCompleted;


	public override void _Ready() {
		base._Ready();
		button.Visible = false;
		scores = FindObjectHelper.getGameManager(this).getHighScores();
		renderScore();
	}	

	private void renderScore() {
		foreach(Node node in scoreHolder.GetChildren()) {
			node.QueueFree();
		}

		foreach(RunResource score in scores) {
			Control control = (Control)scoreText.Instantiate();
			((RichTextLabel)control.GetChild(0)).Text = score.name;
			((RichTextLabel)control.GetChild(1)).Text = getScoreText(score.score, score.completed);

			scoreHolder.AddChild(control);
		}
	}



	public void addHighScore(int score, bool completed) {
		scoreValue = score;
		scoreCompleted = completed;
   		for(int scoreCount = 0; scoreCount < scores.Count; scoreCount++) {
			if (score > scores[scoreCount].score) {
				Control control = (Control)scoreInput.Instantiate();
				((RichTextLabel)control.GetChild(1)).Text = getScoreText(score,completed);
				nameInput = (LineEdit)control.GetChild(0);
				nameInput.TextSubmitted+= enteredName;
				scoreHolder.AddChild(control);
				scoreHolder.MoveChild(control, scoreCount);
				return;
			}
		}
		Control control2 = (Control)scoreInput.Instantiate();
		((RichTextLabel)control2.GetChild(1)).Text = getScoreText(score,completed);
		nameInput = (LineEdit)control2.GetChild(0);
		nameInput.TextSubmitted+= enteredName;
		scoreHolder.AddChild(control2);
	}

	public void enteredName(String text) {
		FindObjectHelper.getGameManager(this).submitScore(new RunResource(text, scoreValue, scoreCompleted));
		renderScore();
		button.Visible = true;
	}

	private string getScoreText(int score, bool completed) {
		string starText = "";
		if (completed) {
			starText = "[img with=32 height=32]res://UI/highscore/starSingle.png[/img]";
		} else {
			starText = "[img with=32 height=32]res://UI/highscore/blank32.png[/img]";
		}
		return TextHelper.right(score.ToString("N0")+starText);

	}
}
