using Godot;
using System;

public partial class GameCamera : Camera2D
{
	[Export] AnimationPlayer animationPlayer;
	[Export] NoiseTexture2D noise;
	[Export] float shakeIntensity;
	[Export] float shakeDecay;
	[Export] float shakeSpeed;


	Vector2 startPosition;
	float currentShakeIntensity = 0 ; 
	float time;
	
	public void shake() {
		//animationPlayer.Play("Shake");
		currentShakeIntensity = shakeIntensity;
	}

	public override void _Ready()
	{
		base._Ready();
		startPosition = Position;
	}

	

	public override void _Process(double delta)
	{
		base._Process(delta);
		currentShakeIntensity = Lerp(currentShakeIntensity, 0, (float)delta * shakeDecay);
		Position = getPosition((float)delta);
	}

	private Vector2 getPosition(float delta) {
		time += delta * shakeSpeed;
		return startPosition + new Vector2(
			noise.Noise.GetNoise2D(0,time) * currentShakeIntensity,
			noise.Noise.GetNoise2D(100,time) * currentShakeIntensity);
	}

	float Lerp(float firstFloat, float secondFloat, float by)
	{
		return firstFloat * (1 - by) + secondFloat * by;
	}
}
