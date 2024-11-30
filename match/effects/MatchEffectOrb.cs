using Godot;
using System;

public partial class MatchEffectOrb : Node
{
	[Export] public PathFollow2D pathFollow2D;
	[Export] public Path2D path;
	[Export] public MatchOrb orb;
	[Export] public Node2D placementTracker;
	[Export] public Vector2 randomOffsetRange;


	public float generatePathToCenter(Vector2 globalPosition) {
		Vector2 endPosition = new Vector2((float)GD.RandRange(-randomOffsetRange.X, randomOffsetRange.X),(float)GD.RandRange(-randomOffsetRange.Y, randomOffsetRange.Y));
		
		placementTracker.GlobalPosition = globalPosition;
		Vector2 startingPosition = placementTracker.Position;
		path.Curve.ClearPoints();
		path.Curve = (Curve2D)path.Curve.Duplicate();
		Vector2 normal = (startingPosition * new Vector2(1,-1)).Normalized();
		path.Curve.AddPoint(startingPosition);
		path.Curve.AddPoint((startingPosition *.5f) + (normal * 100));
		path.Curve.AddPoint(endPosition);

		Vector2 inOutVector = startingPosition * .33f;
  
		path.Curve.SetPointIn(1,inOutVector);
		path.Curve.SetPointOut(1,-inOutVector);

		pathFollow2D.ProgressRatio = 0.0f;

		//tweenMovement.TweenProperty(matchOrb.pathFollow2D, "progress_ratio", 1.0f, Math.Clamp(length /500.0f, 1.0f, 2.0f));.

		orb.start();

		return startingPosition.DistanceTo(endPosition);

	}

	public void destroy() {
		orb.destroy();
	}
}
