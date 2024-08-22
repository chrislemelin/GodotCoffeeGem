using Godot;
using System;

public partial class ControllerInput : Node
{
	private bool horizontalDeadZoneHit = true;
	private bool verticalDeadZoneHit = true;



	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("left") )
		{
			if (@event is InputEventJoypadMotion){
				float actionStrength = @event.GetActionStrength("left");
				if (actionStrength < .8) {
					horizontalDeadZoneHit = true;
				}
				else if (horizontalDeadZoneHit) {
					controllerLeft();
					horizontalDeadZoneHit = false;
				}
			} else {
				controllerLeft();
			}
		
		}
		if (@event.IsActionPressed("right"))
		{
			if (@event is InputEventJoypadMotion){
				float actionStrength = @event.GetActionStrength("right");
				if (actionStrength < .8) {
					horizontalDeadZoneHit = true;
				}
				else if (horizontalDeadZoneHit) {
					controllerRight();
					horizontalDeadZoneHit = false;
				}
			} else {
				controllerRight();
			}
		}



		if (@event.IsActionPressed("up"))
		{
			if (@event is InputEventJoypadMotion){
				float actionStrength = @event.GetActionStrength("up");
				if (actionStrength < .8) {
					verticalDeadZoneHit = true;
				}
				else if (verticalDeadZoneHit) {
					controllerUp();
					verticalDeadZoneHit = false;
				}
			} else {
				controllerUp();
			}
		}
		if (@event.IsActionPressed("down"))
		{
			if (@event is InputEventJoypadMotion){
				float actionStrength = @event.GetActionStrength("down");
				if (actionStrength < .8) {
					verticalDeadZoneHit = true;
				}
				else if (verticalDeadZoneHit) {
					controllerDown();
					verticalDeadZoneHit = false;
				}
			} else {
				controllerDown();
			}
		}
	}

	protected  virtual void controllerRight(){

	}
	protected virtual void controllerLeft(){

	}
	protected  virtual void controllerUp(){

	}
	protected virtual void controllerDown(){

	}
}
