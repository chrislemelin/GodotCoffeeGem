using Godot;
using System;

public partial class Gem : lerp
{
	[Export] public Sprite2D sprite2D;
	[Export] AnimationPlayer animationPlayer;

	[Signal]
	public delegate void doneDyingSignalEventHandler(Gem gem);
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

	public void startDying() {
		animationPlayer.Play("PopAnimation");
	}

	public void doneDying() {
		QueueFree();
		EmitSignal(SignalName.doneDyingSignal, this);
	}
}
