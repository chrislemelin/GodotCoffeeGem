using Godot;
using System;

public partial class CoffeeRandomMovement : lerp
{
	[Export] Vector2 minValue;
	[Export] Vector2 maxValue;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		doneMovingSignal+= moveToRandomSpot;
		moveToRandomSpot();
	}

	private void moveToRandomSpot() {
		Tween tween = GetTree().CreateTween();
		speed = 5.0f;
		//tween.TweenProperty(this, "speed", 10.0, 3.0f);
		//tween.SetTrans(Tween.TransitionType.Quad);
		moveToPostion(new Vector2((float)GD.RandRange(minValue.X, maxValue.X),(float)GD.RandRange(minValue.Y, maxValue.Y)));
	}


}
