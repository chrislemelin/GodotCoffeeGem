using Godot;
using Godot.Collections;
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
	[Export] CheckBox fullScreenCheckBox;
	[Export] OptionButton resolutionsOptionButton;
	[Export] Panel panel;

	private Dictionary<String, Vector2> resolutionDictionary =  new Dictionary<String, Vector2> () {
		{"1366 × 768", new Vector2(1366,768)},
		{"1440 × 900", new Vector2(1440,900)},
		{"1920 × 1080", new Vector2(1920,1080)},
		{"3840 × 2160", new Vector2(3840,2160)}

	};

	public override void _Ready()
	{
		base._Ready();
		setUpResolutions();
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

		fullScreenCheckBox.Pressed += () =>  {
			if (fullScreenCheckBox.ButtonPressed) {
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
			} else {
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			} 
		};

		resolutionsOptionButton.ItemSelected += (value) => DisplayServer.WindowSetSize((Vector2I)resolutionDictionary[resolutionsOptionButton.GetItemText((int)value)]);
		resolutionsOptionButton.Selected = 2;

		dataCollectionCheckBox.ButtonPressed = gameManagerIF.getCollectData();
		dataCollectionCheckBox.Pressed += () => gameManagerIF.setCollectData(dataCollectionCheckBox.ButtonPressed);
	}

	private void setUpResolutions() {
		foreach(String resolution in resolutionDictionary.Keys) {
			resolutionsOptionButton.AddItem(resolution);
		}
	}

	private void quitGame () {
		FindObjectHelper.getFormSubmitter(this).submitData("quit", FindObjectHelper.getGameManager(this), () =>  GetTree().Quit());
	}
}
