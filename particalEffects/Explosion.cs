using Godot;
using System;

public partial class Explosion : Node2D
{
	[Export] GpuParticles2D particles2D;
	[Export] float timeEmiting;
	[Export] float timeAlive;

	public override void _Ready()
	{
		base._Ready();
		GetTree().CreateTimer(timeEmiting).Timeout += () => particles2D.Emitting = false;
		GetTree().CreateTimer(timeAlive).Timeout += () => QueueFree();


	}
}
