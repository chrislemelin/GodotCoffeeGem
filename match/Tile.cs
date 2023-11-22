using Godot;
using System;
using System.Diagnostics.Contracts;

public partial class Tile : Node2D
{
	[Export] public int x;
	[Export] public int y;
	[Export] public Sprite2D sprite2D;
	[Export] public Area2D area2D;

	[Export] Color highlightColor;
	[Export] Color hoverColor;
	[Export] Color selectColor;

	Color normalColor;

	bool isHighlighted = false;
	bool isSelected = false;
	bool isHovered = false;

	public Gem Gem
	{
		get; set;
	}

		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		normalColor = sprite2D.SelfModulate;
		area2D.MouseEntered += () => mouseEnter();
		area2D.MouseExited += () => mouseExit();
	}

	public Vector2 getPosition() {
		return new Vector2(x,y);
	}

	public void turnHighlightOn() {
		isHighlighted = true;
		sprite2D.SelfModulate = getCurrentColor();
	}
	public void turnHighlightOff() {
		isHighlighted = false;
		sprite2D.SelfModulate = getCurrentColor();
	}
	public void turnSelectedOn() {
		isSelected = true;
		sprite2D.SelfModulate = getCurrentColor();
	}
	public void turnSelectedOff() {
		isSelected = false;
		sprite2D.SelfModulate = getCurrentColor();
	}

	public void mouseEnter() {
		isHovered = true;
		sprite2D.SelfModulate = getCurrentColor();

	}
	public void mouseExit() {
		isHovered = false;
		sprite2D.SelfModulate = getCurrentColor();
	}

	private Color getCurrentColor() {
		if(isSelected) {
			return selectColor;
		}
		if (isHovered) {
			return hoverColor;
		}
		if (isHighlighted) {
			return highlightColor;
		}

		return normalColor;
	}
	

}
