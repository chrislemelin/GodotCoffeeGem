using Godot;
using System;

public partial class WelcomeScreen : Control
{
	[Export] private Button button2;
	[Export] private Control screens;
	[Export] private TextureButton textureButton;
	int screenNumber = 1;

	public override void _Ready()
	{
		base._Ready();
		textureButton.Pressed += () => startUp();
		button2.Pressed += () => advanceScreen();
	}

	public void startUp() {
		Visible = true;
		screenNumber = 0;
		advanceScreen();
	}

	private void advanceScreen() {
		screenNumber++;
		if (screenNumber > screens.GetChildCount()){
			Visible = false;
		} else {
			Godot.Collections.Array<Node> children = screens.GetChildren();
			for (int index = 0; index < children.Count; index ++) {
				if (screenNumber -1 == index) {
					((Control)children[index]).Visible = true;
				} else {
					((Control)children[index]).Visible = false;
				}
			}
		}
	}
}
