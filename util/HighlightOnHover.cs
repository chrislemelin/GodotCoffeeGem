using Godot;
using System;

public partial class HighlightOnHover : TextureRect
{
	[Export] Control area2D;
	[Export] bool forceHighlightOn = false;
	[Export] bool forceHighlightOff = false;
	[Export] bool highlightOnSelf = true;

	[Export] Node2D makeBigger;
	[Export] Control makeBiggerControl;

	Vector2? startingSize = null;
	[Export] float makeBiggerScale;

	bool isHovered = false;
	public HighlightOnHover() {

	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = (Material)Material.Duplicate();
		if (highlightOnSelf) {
			MouseEntered += () =>  setHighlightFromMouse(true);
			MouseExited += () =>  setHighlightFromMouse(false);
		}
		if (area2D != null) {
			area2D.MouseEntered += () => setHighlightFromMouse(true);
			area2D.MouseExited += () => setHighlightFromMouse(false);
		}
	}

	private void setHighlightFromMouse(bool value) {
		if (startingSize == null) {
			if (makeBigger != null) {
				startingSize = makeBigger.Scale;
			}
			if (makeBiggerControl != null) {
				startingSize = makeBiggerControl.Scale;
			}
		}

		isHovered = value;
		if (makeBigger != null) {
			if (isHovered) {
				makeBigger.Scale = (Vector2)startingSize * makeBiggerScale;
			} else {
				makeBigger.Scale = (Vector2)startingSize;
			}
		}
		if (makeBiggerControl != null) {
			if (isHovered) {
				makeBiggerControl.Scale = (Vector2)startingSize * makeBiggerScale;
			} else {
				makeBiggerControl.Scale = (Vector2)startingSize;
			}
		}
		renderHighlight();
	}

	public void setForceHighlight(bool forceHighlightValue)
	{
		forceHighlightOn = forceHighlightValue;
		renderHighlight();
	}

	public void setForceHighlightOff(bool forceHighlightValue)
	{
		forceHighlightOff = forceHighlightValue;
		renderHighlight();
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		renderHighlight();
	}

	private void renderHighlight() {
		if(forceHighlightOn) {
			setHighlightOn();
		} else if (forceHighlightOff) {
			setHighlightOff();	
		}
		else if (isHovered) {
			setHighlightOn();
		} else {
			setHighlightOff();
		}
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
