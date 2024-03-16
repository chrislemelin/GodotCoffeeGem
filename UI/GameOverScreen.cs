using Godot;
using System;

public partial class GameOverScreen : Control
{
	[Export] Button restartButton;
	[Export] public RichTextLabel label;

	public override void _Ready()
	{
		restartButton.Pressed += () => GetTree().ChangeSceneToFile("res://mainScenes/MainMenu.tscn");
	}
}
