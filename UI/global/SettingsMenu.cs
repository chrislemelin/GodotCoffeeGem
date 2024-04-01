using Godot;
using System;

public partial class SettingsMenu : Node
{
	[Export] public HSlider sfxSlider;
	[Export] public HSlider musicSlider;
	[Export] CheckBox dataCollectionCheckBox;
	[Export] Button quitButton;
	[Export] Button closeButton;
	[Export] Button mainMenuButton;
	[Export] Button visibleButton;
	[Export] Panel panel;


	public override void _Ready()
	{
		base._Ready();
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		panel.Visible = false;
		quitButton.Pressed += () => quitGame();
		visibleButton.Pressed += () => panel.Visible = !panel.Visible;
		closeButton.Pressed += () => panel.Visible = !panel.Visible;
		
		sfxSlider.Value = gameManagerIF.getSFXVolume();
		sfxSlider.ValueChanged += (value) => gameManagerIF.setSFXVolume((float)value); 
		musicSlider.Value = gameManagerIF.getMusicVolume();
		musicSlider.ValueChanged += (value) => gameManagerIF.setMusicVolume((float)value); 

		mainMenuButton.Pressed += () => FindObjectHelper.getFormSubmitter(this).submitData("quit to main menu", gameManagerIF,() => {
			gameManagerIF.resetGlobals();
			GetTree().ChangeSceneToFile("res://mainScenes/MainMenu.tscn");
		});
		closeButton.Disabled = false;


		dataCollectionCheckBox.ButtonPressed = gameManagerIF.getCollectData();
		dataCollectionCheckBox.Pressed += () => gameManagerIF.setCollectData(dataCollectionCheckBox.ButtonPressed);
	}

	private void quitGame () {
		FindObjectHelper.getFormSubmitter(this).submitData("quit", FindObjectHelper.getGameManager(this), () =>  GetTree().Quit());
	}
}
