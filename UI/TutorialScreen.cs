using Godot;
using System;

public partial class TutorialScreen : Control
{
	[Export] private TextureButton textureButton;

	public override void _Ready()
	{
		base._Ready();
		textureButton.Pressed += () => Visible = !Visible;
	}
}
