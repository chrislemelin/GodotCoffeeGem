using Godot;
using System;

public partial class OpenBrowserButton : CustomButton
{

	[Export] string url = "res://mainScenes/MainMenu.tscn";

	public override void _Ready()
	{
		base._Ready();
		Pressed += () => OS.ShellOpen(url);
	}


}
