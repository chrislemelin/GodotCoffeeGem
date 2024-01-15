using Godot;
using System;

public partial class TutorialScreen : Control
{
	[Export] private TextureButton textureButton;
	[Export] private Button button2;

	public override void _Ready()
	{
		base._Ready();
		textureButton.Pressed += () => Visible = !Visible;
		button2.Pressed += () => Visible = !Visible;
	}
}
