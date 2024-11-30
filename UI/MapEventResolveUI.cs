using Godot;
using System;

public partial class MapEventResolveUI : ToggleVisibilityOnButtonPress
{
	[Export] RichTextLabel title;
	[Export] RichTextLabel description;
	[Export] Button continueButton;
	[Export] Button declineButton;
	[Signal] public delegate void AcceptableEventHandler(bool accepted);


	
	public override void _Ready()
	{
		base._Ready();
		declineButton.Pressed += () => {
			EmitSignal(SignalName.Acceptable, false);
			setVisible(false);
		};
		continueButton.Pressed += () => {
			EmitSignal(SignalName.Acceptable, true);
		};
		//playDistort();
	}
	

	public void setUp(string title, string description, bool acceptablePrompt = false, bool continueEnabled = true) {
		this.title.Text = TextHelper.centered(title);
		this.description.Text = description;
		resetAnimation();
		setVisible(true);
		continueButton.Disabled = !continueEnabled;
		if(acceptablePrompt) {
			continueButton.Text = "Accept";
			declineButton.Visible = true;
		} else {
			declineButton.Visible = false;
			continueButton.Text = "Continue";
		}
	}

}
