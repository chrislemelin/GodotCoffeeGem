using Godot;
using System;

public partial class HighlightOnHover : Sprite2D
{
	bool forceHighlightOn = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = (Material)Material.Duplicate();
	}

	public void setForceHighlight(bool forceHighlightValue)
	{
		if (forceHighlightOn)
		{
			setHighlightOn();
		}
		else
		{
			checkForHighlight();
		}
		forceHighlightOn = forceHighlightValue;
	}

	private void checkForHighlight()
	{
		if (forceHighlightOn)
		{
			return;
		}
		if (GetRect().HasPoint(GetLocalMousePosition()))
		{
			setHighlightOn();
		}
		else
		{
			setHighlightOff();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		checkForHighlight();
	}

	private void setHighlightOn()
	{
		Color color = (Color)Material.Get("shader_parameter/line_color");
		Color newColor = new Color(color.R, color.G, color.B, 1.0f);
		Material.Set("shader_parameter/line_color", newColor);

	}
	private void setHighlightOff()
	{
		Color color = (Color)Material.Get("shader_parameter/line_color");
		Color newColor = new Color(color.R, color.G, color.B, 0.0f);
		Material.Set("shader_parameter/line_color", newColor);
	}
}
