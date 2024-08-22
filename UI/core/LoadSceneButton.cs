using Godot;
using System;

public partial class LoadSceneButton : CustomButton
{
	[Export] string sceneString = "res://mainScenes/MainMenu.tscn";

	public override void _Ready()
	{
		base._Ready();
		Pressed += () => GetTree().ChangeSceneToFile(sceneString);
	}


}
