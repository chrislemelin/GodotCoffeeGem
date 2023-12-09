using Godot;
using System;

public partial class MainMenu : Node
{
	[Export] Button newGameButton;
	[Export] PackedScene gameScene;

	public override void _Ready()
	{
		newGameButton.Pressed += () => GetTree().ChangeSceneToPacked(gameScene);
	}
}
