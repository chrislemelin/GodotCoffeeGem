using Godot;
using System;

public partial class GameOverScreen : Control
{
	[Export] Button restartButton;

	public override void _Ready()
	{
		restartButton.Pressed += () => GetTree().ReloadCurrentScene();
	}

}
