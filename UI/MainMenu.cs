using Godot;
using System;

public partial class MainMenu : GameManagerIF
{
	[Export] Button quickPlayButton;
	[Export] Button deckSelectionButton;

	[Export] Button zenModButton;
	[Export] Button quitButton;
	[Export] Button metaProgressionButton;
	[Export] Button creditsButton;
	[Export] Control tryTutorialUI;


	[Export] PackedScene gameScene;
	[Export] PackedScene deckSelectionScene;
	[Export] PackedScene metaProgressionScene;
	[Export] PackedScene creditsScene;
	
	[Export] SettingsMenu settingsMenu;
	[Export] Button settingsMenuButton;

	[Export] PackedScene beanScene;
	[Export] int numberOfBeans;
	[Export] Node2D beanHolder;

	public override void _Ready()
	{
		quickPlayButton.Pressed += () => {
			if (!getGlobal().shownWelcomeTutorial) {
				tryTutorialUI.Visible = true;
				setShownWelcomeTutorial(true);
			} else {
				GetTree().ChangeSceneToFile("res://mainScenes/DeckSelectionPicker.tscn");
			}
		};
		//deckSelectionButton.Pressed += () => GetTree().ChangeSceneToPacked(deckSelectionScene);
		//metaProgressionButton.Pressed += () => GetTree().ChangeSceneToPacked(metaProgressionScene);
		creditsButton.Pressed += () => GetTree().ChangeSceneToPacked(creditsScene);
		settingsMenuButton.Pressed += () => settingsMenu.openWindow();
		// zenModButton.Pressed += () =>
		// {
		// 	setZenMode(true);
		// 	GetTree().ChangeSceneToPacked(gameScene);
		// };


		for (int a = 0; a < numberOfBeans; a++)
		{
			generateBean();
		}
		quitButton.Pressed += () => GetTree().Quit();
	}

	private void generateBean()
	{
		Rotate rotatingBean = (Rotate)beanScene.Instantiate();
		beanHolder.AddChild(rotatingBean);
		rotatingBean.Position = new Vector2(GD.RandRange(0, 1920), GD.RandRange(0, 1080));
		rotatingBean.rotateSpeed = GD.RandRange(20, 70);
		rotatingBean.RotationDegrees = GD.RandRange(0, 360);
	}
}
