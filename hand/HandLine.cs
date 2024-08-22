using Godot;
using System;

public partial class HandLine : Line2D
{
	private bool enabled = true;

	private ControllerHelper controllerHelper;
	public override void _Ready(){
		controllerHelper = FindObjectHelper.getControllerHelper(this);
		controllerHelper.UsingControllerChanged += setInvisible;
		setInvisible(controllerHelper.isUsingController());
	}
	private Vector2 startPoint;


	public void turnOn(Vector2 startPoint)
	{   
		this.startPoint = startPoint;
		this.enabled = true;
	}
	public void turnOff()
	{   
		enabled = false;
	}

	public override void _Process(double delta) {
		if (!controllerHelper.isUsingController()){
			if (enabled) {
				ClearPoints();
				AddPoint(startPoint);
				AddPoint(GetLocalMousePosition());
				Visible = true;
			}
		}
		if(!enabled) {
			Visible = false;
		}

	}

	public void setPosition(Vector2 globalPosition) {
		if (enabled) {
			ClearPoints();
			AddPoint(startPoint);
			AddPoint(ToLocal(globalPosition));
			Visible = true;
		}
	}

	public void setInvisible(bool usingController) {
		enabled = false;
	}
}
