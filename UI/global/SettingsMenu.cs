using Godot;
using System;

public partial class SettingsMenu : Node
{
	[Export] HSlider sfxSlider;
	[Export] HSlider musicSluder;
	[Export] CheckBox dataCollectionCheckBox;
	[Export] Button quitButton;
	[Export] Button closeButton;

	[Export] Button visibleButton;
	[Export] Panel panel;


	public override void _Ready()
	{
		base._Ready();
		panel.Visible = false;
		quitButton.Pressed += () => quitGame();
		visibleButton.Pressed += () => panel.Visible = !panel.Visible;
		closeButton.Pressed += () => panel.Visible = !panel.Visible;
		closeButton.Disabled = false;

		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);

		dataCollectionCheckBox.ButtonPressed = gameManagerIF.getCollectData();
		dataCollectionCheckBox.Pressed += () => gameManagerIF.setCollectData(dataCollectionCheckBox.ButtonPressed);
	}

	private void quitGame () {
		FindObjectHelper.getFormSubmitter(this).submitData("quit", FindObjectHelper.getGameManager(this), () =>  GetTree().Quit());
	}
}
