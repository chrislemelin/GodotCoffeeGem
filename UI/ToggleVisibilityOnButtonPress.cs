using Godot;
using System;

public partial class ToggleVisibilityOnButtonPress : Control
{
	[Export] public Button button;
	[Export] AnimationPlayer animationPlayer;
	[Export] public RichTextLabel richTextLabel;

	[Signal] public delegate void WindowClosedSignalEventHandler();

	[Export] protected float fadeOutDelay = 0.0f;

	public override void _Ready()
	{
		base._Ready();
		button.Pressed += () => setVisible(!Visible); 
	}

	public void setVisible(bool value) {
		if (animationPlayer != null) {
			if (value) {
				animationPlayer.Play("FadeIn");
				Visible = value;
			}
			else {
				GetTree().CreateTimer(fadeOutDelay).Timeout += () =>  {
					animationPlayer.PlayBackwards("FadeIn");
				};

				GetTree().CreateTimer(.25f + fadeOutDelay).Timeout += () =>  {
					EmitSignal(SignalName.WindowClosedSignal);
					Visible = value;
				};
			}
		} else {
			Visible = value;
		}
	}

	public void resetAnimation() {
		animationPlayer.Play("RESET");
	}
}
