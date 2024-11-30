using Godot;
using System;
using System.Drawing;

public partial class lerp : Node2D
{
	Vector2 startPosition;
	Vector2 endPosition;
	[Export] protected float speed;
	float constantTimeSpeed = 0.0f;
	float timeStartedMovement;
	float timeAfterMovement;


	[Signal]
	public delegate void doneMovingSignalEventHandler();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timeStartedMovement = -1;
	}

	public void moveToPostion(Vector2 newPosition)
	{
		endPosition = newPosition;
		startPosition = Position;
		timeStartedMovement = Time.GetTicksMsec() / 1000.0f;
		timeAfterMovement = timeStartedMovement;
	}

	public void moveToPostionConstTime(Vector2 newPosition, float time)
	{
		moveToPostion(newPosition);
		constantTimeSpeed = endPosition.DistanceTo(startPosition) / time;
	}

	public void moveToGlobalPostion(Vector2 newPosition)
	{
		moveToPostion(newPosition);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (timeStartedMovement > 0)
		{
			float currentSpeed = constantTimeSpeed == 0.0 ? speed : constantTimeSpeed;
			float distance = endPosition.DistanceTo(startPosition);
			timeAfterMovement += (float)delta;
			float lerpValue = (timeAfterMovement - timeStartedMovement) * currentSpeed / distance;
			Vector2 finalPosition = startPosition.Lerp(endPosition, lerpValue);
			if (lerpValue > 1)
			{
				finalPosition = endPosition;
				timeStartedMovement = -1;
				doneMoving();
			}
			Position = finalPosition;
		}
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
		{
			//moveToPostion(GetGlobalMousePosition());
		}
	}


	protected virtual void doneMoving()
	{
		EmitSignal(SignalName.doneMovingSignal);
		constantTimeSpeed = 0.0f;
		//doneMovingSignal =-
	}

}
