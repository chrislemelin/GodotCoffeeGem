using Godot;
using System;
using System.IO;

public partial class Rotate : Node2D
{
	[Export] public float rotateSpeed;
	public override void _Ready()
	{
		base._Ready();
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		this.RotationDegrees = (RotationDegrees + (float)delta * rotateSpeed) % 360;

	}
}
