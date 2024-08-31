using Godot;
using System;
using System.Diagnostics.Contracts;

public partial class Tile : Node2D
{
	[Export] public int x;
	[Export] public int y;
	[Export] public Sprite2D sprite2D;
	[Export] public Sprite2D disableSprite2D;

	[Export] public Control control;
	[Export] public Node2D gemParent;
	[Export] Color highlightColor;
	[Export] Color hoverColor;
	[Export] Color selectColor;
	[Export] Color blockedColor;
	[Export] TextureRect goo;

	Color normalColor;

	bool isHighlighted = false;
	bool isSelected = false;
	bool isHovered = false;
	bool isDisabled = false;
	bool isBlocked = false;

	public Gem Gem
	{
		get; set;
	}

	// public void addGem(Gem Gem) {
	// 	this.Gem = Gem;
	// 	gemParent.AddChild(Gem);
	// }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		normalColor = sprite2D.Modulate;
		control.MouseEntered += () => mouseEnter();
		control.MouseExited += () => mouseExit();
		goo.Visible = false;
	}

	public void setBlocked(bool value) {
		isBlocked = value;
		goo.Visible = value;
		sprite2D.Modulate = getCurrentColor();
	}

	public Vector2 getTilePosition() {
		return new Vector2(x,y);
	}

	public void setDisabled(bool disabled) {
		isDisabled = disabled;
		if (isDisabled) {
			disableSprite2D.Visible = true;
		} else {
			disableSprite2D.Visible = false;
		}
	}

	public bool getIsBlocked() {
		return isBlocked;
	}

	public void turnHighlightOn() {
		isHighlighted = true;
		sprite2D.Modulate = getCurrentColor();
	}
	public void turnHighlightOff() {
		isHighlighted = false;
		sprite2D.Modulate = getCurrentColor();
	}
	public void turnSelectedOn() {
		isSelected = true;
		sprite2D.Modulate = getCurrentColor();
	}
	public void turnSelectedOff() {
		isSelected = false;
		sprite2D.Modulate = getCurrentColor();
	}

	public void mouseEnter() {
		isHovered = true;
		if(Gem != null) {
			Gem.shake();
		}
		sprite2D.Modulate = getCurrentColor();

	}
	public void mouseExit() {
		isHovered = false;
		sprite2D.Modulate = getCurrentColor();
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
		if (isBlocked) {
			return blockedColor;
		}
		return normalColor;
	}
	

}
