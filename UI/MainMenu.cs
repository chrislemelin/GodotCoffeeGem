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
	[Export] Control gameplayCollectionTutorial;

	[Export] PackedScene gameScene;
	[Export] PackedScene deckSelectionScene;
	[Export] PackedScene metaProgressionScene;
	[Export] PackedScene creditsScene;
	
	[Export] SettingsMenu settingsMenu;
	[Export] Button settingsMenuButton;

	[Export] PackedScene beanScene;
	[Export] int starterBeans;
	[Export] int numberOfBeans;
	[Export] float beanWaitTime;

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

		if (!getGlobal().shownGameplayCollectionTutorial) {
			gameplayCollectionTutorial.Visible = true;
			setShownGameplayCollectionTutorial(true);
		}
		

		Timer timer = new Timer();
		AddChild(timer);
		timer.WaitTime = beanWaitTime;
		timer.OneShot = false;
		timer.Timeout += generateBean;
		timer.Start();
		generateBean();
		generateInitalBeans();

		//deckSelectionButton.Pressed += () => GetTree().ChangeSceneToPacked(deckSelectionScene);
		//metaProgressionButton.Pressed += () => GetTree().ChangeSceneToPacked(metaProgressionScene);
		settingsMenuButton.Pressed += () => settingsMenu.openWindow();
		// zenModButton.Pressed += () =>
		// {
		// 	setZenMode(true);
		// 	GetTree().ChangeSceneToPacked(gameScene);
		// };


		// for (int a = 0; a < numberOfBeans; a++)
		// {
		// 	generateBean();
		// }
		quitButton.Pressed += () => GetTree().Quit();
	}
	private void generateInitalBeans()
	{
		for (int a = 0; a < starterBeans; a++)
		{
			Rotate rotatingBean = (Rotate)beanScene.Instantiate();
			beanHolder.AddChild(rotatingBean);
			rotatingBean.Position = new Vector2(GD.RandRange(0, 1920), GD.RandRange(0,500));
		}

	}

	private void generateBean()
	{
		for (int a = 0; a < numberOfBeans; a++)
		{
			Rotate rotatingBean = (Rotate)beanScene.Instantiate();
			beanHolder.AddChild(rotatingBean);
			rotatingBean.Position = new Vector2(GD.RandRange(-100, 2020), GD.RandRange(-100, -150));
		}

		//rotatingBean.rotateSpeed = GD.RandRange(20, 70);
		//rotatingBean.RotationDegrees = GD.RandRange(0, 360);
	}
}
