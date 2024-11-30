using Godot;
using System;
using System.IO;

public partial class Rotate : Node2D
{
	[Export] public float rotateSpeed;
	[Export] VisibleOnScreenNotifier2D visibleOnScreen;

	public override void _Ready()
	{
		base._Ready();
		Timer timer = new Timer();
		AddChild(timer);
		timer.WaitTime = 10.0f;
		timer.OneShot = false;
		timer.Timeout += checkForCleanup;
		timer.Start();
	}

	private void checkForCleanup()
	{
		if(!visibleOnScreen.IsOnScreen()) {
			QueueFree();
		}
	}
}
