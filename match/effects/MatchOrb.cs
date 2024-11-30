using Godot;
using System;

public partial class MatchOrb : Node2D
{
	[Export] CpuParticles2D particalEffect;
	[Export] Sprite2D sprite;


	public void start() {
		particalEffect.Emitting = true;
	}

	public void destroy() {
		sprite.Visible = false;
		particalEffect.Emitting = false;
		GetTree().CreateTimer(2.0).Timeout += () => QueueFree();
	}

}
