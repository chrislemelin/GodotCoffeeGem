using Godot;
using System;

public partial class HandLine : Line2D
{

	private Vector2 startPoint;


	public void turnOn(Vector2 startPoint)
	{   
		this.startPoint = startPoint;
		Visible = true;
	}
	 public void turnOff()
	{   
		Visible = false;
	}

	public override void _Process(double delta) {
		if (Visible) {
			ClearPoints();
			AddPoint(startPoint);
			AddPoint(GetLocalMousePosition());
		}
	}

	
	

}
