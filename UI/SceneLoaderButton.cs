using Godot;
using System;

public partial class SceneLoaderButton : Button
{
	[Export] string sceneString;
	public override void _Ready() {
		Pressed += () => GetTree().ChangeSceneToFile(sceneString);
	}
}
