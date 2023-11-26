using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public partial class CardUI : CardIF
{
	[Export]
	public Control control;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		control.GuiInput += (inputEvent) => EmitSignal(SignalName.clickedSignal, inputEvent);	}

	public Delegate getClickedSignal()
	{
		return control._GuiInput;
	}
	
	public override void listenToMouseEnter(Action function)
	{
		control.MouseEntered += function;
	}

	public override void listenToMouseExit(Action function)
	{
		control.MouseExited += function;
	}

}
