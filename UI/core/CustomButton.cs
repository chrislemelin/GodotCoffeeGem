using Godot;
using System;

public partial class CustomButton : Button
{
	[Export] AnimationPlayer animationPlayer;
	[Export] AudioPlayer hoverAudioplayer;
	[Export] AudioPlayer clickAudioplayer;
	[Export] public bool grabFocus = false;
	[Export] bool isInSettingsMenu = false;


	public void setGrabFocus(bool value) {
		grabFocus = value;
	}

	public override void _Ready()
	{
		setFocusMode();
		if (grabFocus) {
			checkForFocus(true);
			FindObjectHelper.getSettingsMenu(this).windowClosed+= () => checkForFocus(true);
		}
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged += checkForFocus;
		ButtonDown += () =>  {
			clickAudioplayer.Play();
			if (!FindObjectHelper.getControllerHelper(this).isUsingController()) {
				//ReleaseFocus();
			}
		};
		MouseEntered += () => {
			if (!Disabled) {
				hoverAudioplayer.Play();
				animationPlayer.Play("Hover");
			}
		};
	}

	public override void _Process(Double delta)
	{
		base._Process(delta);
		PivotOffset = new Vector2(Size.X/2, Size.Y);
	}


	
	public void checkForFocus() {
		checkForFocus(true);
	}

	private void setFocusMode() {
		if (FindObjectHelper.getControllerHelper(this).isUsingController()) {
			FocusMode = FocusModeEnum.All;
		} else {
			FocusMode = FocusModeEnum.None;
		}
	}

	public void checkForFocus(bool value) {

		setFocusMode();
		if (grabFocus && IsVisibleInTree() && FindObjectHelper.getControllerHelper(this).isUsingController()) {
			SettingsMenu settingsMenu = FindObjectHelper.getSettingsMenu(this);
			if (settingsMenu == null || !settingsMenu.isVisible() || isInSettingsMenu) {
				GrabFocus();
			}	
		} else {
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
