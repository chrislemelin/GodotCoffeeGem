using Godot;
using System;

public partial class CustomButton : Button
{
	[Export] AnimationPlayer animationPlayer;
	[Export] AudioPlayer hoverAudioplayer;
	[Export] AudioPlayer clickAudioplayer;
	[Export] bool grabFocus = false;
	[Export] bool isInSettingsMenu = false;


	public void setGrabFocus(bool value) {
		grabFocus = value;
	}

	public override void _Ready()
	{
		if (grabFocus) {
			GD.Print("trying to grab focus for "+ Text);
			checkForFocus(true);
			FindObjectHelper.getSettingsMenu(this).windowClosed+= () => checkForFocus(true);	
		}
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged += checkForFocus;
		ButtonDown += () => clickAudioplayer.Play();
		MouseEntered += () => {
			if (!Disabled) {
				hoverAudioplayer.Play();
				animationPlayer.Play("Hover");
			}
		};
		//PivotOffset = new Vector2(Size.X/2, Size.Y);
	}

	public override void _Process(Double delta)
	{
		base._Process(delta);
		PivotOffset = new Vector2(Size.X/2, Size.Y);
	}


	
	public void checkForFocus() {
		checkForFocus(true);
	}

	public void checkForFocus(bool value) {
		GD.Print("trying to grab focus for "+ Text);
		if (grabFocus && IsVisibleInTree() && FindObjectHelper.getControllerHelper(this).isUsingController()) {
			SettingsMenu settingsMenu = FindObjectHelper.getSettingsMenu(this);
			if (settingsMenu == null || !settingsMenu.isVisible() || isInSettingsMenu) {
				GrabFocus();
				GD.Print("grabing focus for "+ Text);
			}	
		} else {
			GD.Print("relasing focus for "+ Text);
			ReleaseFocus();
		}
	}

	public void loseFocus() {
		
	}

	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= checkForFocus;
	}

	
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		//FindObjectHelper.GetControllerHelper(this).UsingControllerChanged -= checkForFocus;
	}



}
