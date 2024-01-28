using Godot;
using System;

public partial class ToggleVisibilityOnButtonPress : Control
{
	[Export] public Button button;


	public override void _Ready()
	{
		base._Ready();
		button.Pressed += () => Visible = !Visible; 
	}
}
