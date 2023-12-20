using Godot;
using System;

public partial class MainMenu : Node
{
	[Export] Button newGameButton;
	[Export] PackedScene gameScene;
	[Export] PackedScene beanScene;
	[Export] int numberOfBeans;
	[Export] Node2D beanHolder;
	
	public override void _Ready()
	{
		newGameButton.Pressed += () => GetTree().ChangeSceneToPacked(gameScene);
		for(int a = 0; a < numberOfBeans; a ++) {
			generateBean();
		}
	}

	private void generateBean() {
		Rotate rotatingBean = (Rotate)beanScene.Instantiate();
		beanHolder.AddChild(rotatingBean);
		rotatingBean.Position = new Vector2(GD.RandRange(0,1920),GD.RandRange(0,1080));
		rotatingBean.rotateSpeed = GD.RandRange(20,70);
		rotatingBean.RotationDegrees = GD.RandRange(0,360);
	}
}
