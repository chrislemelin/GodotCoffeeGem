using Godot;
using System;

public partial class Background : Node
{
	private bool visible = true;
	[Export] AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		base._Ready();
		//playTransitionDelayed();
	}

	private void playTransitionDelayed() {
		if(visible) {
			animationPlayer.Play("Transition");
		} else {
			animationPlayer.PlayBackwards("Transition");
		}
		visible = !visible;
		GetTree().CreateTimer(5).Timeout+= playTransitionDelayed;
	}
}
