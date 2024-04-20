using Godot;
using System;

public partial class GameOverScreen : Control
{
	[Export] Button restartButton;
	[Export] public RichTextLabel label;
	[Export] private RichTextLabel metaCoinLabel;

	public override void _Ready()
	{
		restartButton.Pressed += () => GetTree().ChangeSceneToFile("res://mainScenes/MainMenu.tscn");
	}

	public void setMetaCoins (int metaCoins) {
		metaCoinLabel.Text = "Gained " + metaCoins;
	}
}
