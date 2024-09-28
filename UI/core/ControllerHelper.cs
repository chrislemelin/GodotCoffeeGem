using Godot;
using System;

public partial class ControllerHelper : Node
{
	[Export ]private bool usingController = false;
	[Signal] public delegate void UsingControllerChangedEventHandler(bool usingController);
	[Signal] public delegate void RelaseFocusEventHandler();

	public override void _Input(InputEvent @event)
	{
		bool oldValue = usingController;
		if (@event is InputEventJoypadButton) {
			usingController = true;
		}
		if (@event is InputEventMouse)
		{
			usingController = false;
		}
		 if (@event is InputEventMouseMotion)
		{
			usingController = false;
		}
		if (oldValue != usingController) {
			GetTree().CreateTimer(.1).Timeout+= () => EmitSignal(SignalName.UsingControllerChanged, usingController);
			if (usingController) {
				Input.MouseMode = Input.MouseModeEnum.Hidden;
			}
			else {
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}
	}

	public void refreshFocus(){
		EmitSignal(SignalName.UsingControllerChanged, usingController);
	}

	public void forceDeselection(){
		EmitSignal(SignalName.UsingControllerChanged, false);
	}

	public bool isUsingController(){
		return usingController;
	}
}
