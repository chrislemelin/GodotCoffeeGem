using Godot;
using Godot.Collections;
using System;

public partial class SettingsMenu : Node
{
	[Export] public HSlider sfxSlider;
	[Export] public HSlider musicSlider;
	[Export] CheckBox dataCollectionCheckBox;
	[Export] Button quitButton;
	[Export] CustomButton closeButton;
	[Export] Button mainMenuButton;
	[Export] Button visibleButton;
	[Export] CheckBox fullScreenCheckBox;
	[Export] OptionButton resolutionsOptionButton;
	[Export] Panel panel;
	[Signal] public delegate void windowClosedEventHandler();
	[Signal] public delegate void windowOpenedEventHandler();
	[Export] private TextureRect controllerTexture;



	private Dictionary<String, Vector2> resolutionDictionary =  new Dictionary<String, Vector2> () {
		{"1280 × 720", new Vector2(1280,720)},
		{"1920 × 1080 (Recommended)", new Vector2(1920,1080)},
		{"2560 x 1440", new Vector2(2560,1440)},
		{"3840 × 2160", new Vector2(3840,2160)}
	};

	public override void _Ready()
	{
		base._Ready();
		ControllerHelper controllerHelper = FindObjectHelper.getControllerHelper(this);
		setVisibilityOnControllerTexture(controllerHelper.isUsingController());
		controllerHelper.UsingControllerChanged += setVisibilityOnControllerTexture;


		setUpResolutions();
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		panel.Visible = false;
		quitButton.Pressed += () => quitGame();
		visibleButton.Pressed += () => openWindow();
		closeButton.Pressed += () => closeWindow();
		
		sfxSlider.Value = gameManagerIF.getSFXVolume();
		sfxSlider.ValueChanged += (value) => gameManagerIF.setSFXVolume((float)value); 
		musicSlider.Value = gameManagerIF.getMusicVolume();
		musicSlider.ValueChanged += (value) => gameManagerIF.setMusicVolume((float)value); 

		FindObjectHelper.GetMusicPlayer(this).setUp();

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
		resolutionsOptionButton.Selected = 1;

		dataCollectionCheckBox.ButtonPressed = gameManagerIF.getCollectData();
		dataCollectionCheckBox.Pressed += () => gameManagerIF.setCollectData(dataCollectionCheckBox.ButtonPressed);
	}

	private void setUpResolutions() {
		foreach(String resolution in resolutionDictionary.Keys) {
			resolutionsOptionButton.AddItem(resolution);
		}
	}

	private void setVisibilityOnControllerTexture(bool usingController){
		controllerTexture.Visible = FindObjectHelper.getControllerHelper(this).isUsingController();
	}

	private void quitGame () {
		FindObjectHelper.getFormSubmitter(this).submitData("quit", FindObjectHelper.getGameManager(this), () =>  GetTree().Quit());
	}

	public bool isVisible() {
		return panel.Visible;
	}


	private void openWindow() {
		panel.Visible = true;
		EmitSignal(SignalName.windowOpened);
		FindObjectHelper.getControllerHelper(this).forceDeselection();

	}
	private void closeWindow() {
		panel.Visible = false;
		EmitSignal(SignalName.windowOpened);
		FindObjectHelper.getControllerHelper(this).refreshFocus();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_settings"))
		{
			if(!panel.Visible) {
				openWindow();
				closeButton.checkForFocus();
			} else {
				closeWindow();
			}
		}
	}
	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= setVisibilityOnControllerTexture;
		base._ExitTree();
	}
}
