using Godot;
using System;

public partial class GameOverScreen : Control
{
	[Export] public Button restartButton;
	[Export] public RichTextLabel label;
	[Export] public RichTextLabel metaCoinLabel;
	[Export] public RichTextLabel debtLabel;


	public override void _Ready()
	{
	}

	public void setUpMainMenu() {
		restartButton.Pressed += () => GetTree().ChangeSceneToFile("res://mainScenes/MainMenu.tscn");
	}

	public void setMetaCoins (int metaCoins) {
		metaCoinLabel.Text = "Gained " + metaCoins;
		debtLabel.Text = "Debt reduced by $"+ metaCoins;
	}
}
