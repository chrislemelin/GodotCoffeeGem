using Godot;
using System;

public partial class MatchScoreText : Control
{
	[Export] AnimationPlayer animationPlayer;
	[Export] RichTextLabel textLabel;


	public override void _Ready()
	{
		animationPlayer.Play("RiseAndFade");
	}

	public void setText(String text) {
		textLabel.Text = text;
	}
}
