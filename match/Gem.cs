using Godot;
using System;

public partial class Gem : lerp
{
	[Export] public Sprite2D sprite2D;
	[Export]
	public GemType Type
	{
		get;
		private set;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite2D.Modulate = Type.GetColor();
	}

	public void setType(GemType type)
	{
		Type = type;
		sprite2D.Modulate = type.GetColor();
	}
}
