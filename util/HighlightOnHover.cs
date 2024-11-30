using Godot;
using System;

public partial class HighlightOnHover : TextureRect
{
	[Export] public bool makeBiggerBool;
	[Export] Control area2D;
	[Export] bool forceHighlightOn = false;
	[Export] bool forceHighlightOff = false;
	bool forceHighlightOnAltColor= false;

	[Export] bool highlightOnSelf = true;
	[Export] Color forceHighlightColor;
	private Color normalHighlightColor;
	[Export] AudioPlayer hoverAudioPlayer;
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
		base._Ready();
		Material = (Material)Material.Duplicate();
		normalHighlightColor = (Color)Material.Get("shader_parameter/line_color");
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
		isHovered = value;
		//renderEnlarge();
		renderHighlight();
		if(value) {
			if (hoverAudioPlayer != null) {
				hoverAudioPlayer.Play();
			}
		}
		if (makeBiggerBool) {
			if (value) {
				enlargeTween();
			} else {
				shrinkTween();
			}
		}

	}

	private void enlargeTween() {
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(makeBiggerControl, "scale", new Vector2(1.05f, 1.05f), .1f);
	}

	private void shrinkTween() {
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(makeBiggerControl, "scale", new Vector2(1f, 1f), .1f);
	}
	private void renderEnlarge() {
		if (startingSize == null) {
			if (makeBigger != null) {
				startingSize = makeBigger.Scale;
			}
			if (makeBiggerControl != null) {
				startingSize = makeBiggerControl.Scale;
			}
		}

		if (makeBigger != null) {
			if (isHighlighted()) {
				makeBigger.Scale = (Vector2)startingSize * makeBiggerScale;
			} else {
				makeBigger.Scale = (Vector2)startingSize;
			}
		}
		if (makeBiggerControl != null) {
			if (isHighlighted()) {
				makeBiggerControl.Scale = (Vector2)startingSize * makeBiggerScale;
			} else {
				makeBiggerControl.Scale = (Vector2)startingSize;
			}
		}
	}
	

	public void setForceHighlight(bool forceHighlightValue)
	{
		forceHighlightOn = forceHighlightValue;
		Material.Set("shader_parameter/line_color", normalHighlightColor);
		renderHighlight();
	}

	public void setForceHighlightAltColor(bool forceHighlightValue)
	{
		forceHighlightOnAltColor = forceHighlightValue;
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
		base._Process(delta);
		renderHighlight();
		//renderEnlarge();
	}

	private bool isHighlighted() {
		if (forceHighlightOnAltColor) {
			return true;
		}
		if(forceHighlightOn) {
			return true;
		} else if (forceHighlightOff) {
			return false;	
		}
		else if (isHovered) {
			return true;
		} else {
			return false;
		}
	}

	private void renderHighlight() {
		if(isHighlighted()) {
			setHighlightOn();
		} else {
			setHighlightOff();	
		}
	}


	private void setHighlightOn()
	{
		Color color = getColor();
		Color newColor = new Color(color.R, color.G, color.B, 1.0f);
		Material.Set("shader_parameter/line_color", newColor);

	}
	private void setHighlightOff()
	{
		Color color = getColor();
		Color newColor = new Color(color.R, color.G, color.B, 0.0f);
		Material.Set("shader_parameter/line_color", newColor);
	}

	private Color getColor() {
		if(forceHighlightOnAltColor) {
			return forceHighlightColor;
		} else {
			return normalHighlightColor;
		}
	}
}
