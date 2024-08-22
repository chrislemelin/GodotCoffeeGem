using Godot;
using System;

public partial class WelcomeScreen : Control
{
	[Export] private CustomButton button2;
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
		FindObjectHelper.getControllerHelper(this).forceDeselection();
		button2.checkForFocus();
	}

	private void advanceScreen() {
		screenNumber++;
		if (screenNumber > screens.GetChildCount()){
			Visible = false;
			FindObjectHelper.getHand(this).setUIFocus(true);
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
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_tutorial"))
		{
			startUp();
		}
	}


}
