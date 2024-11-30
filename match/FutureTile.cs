using Godot;
using System;
using System.Diagnostics.Contracts;

public partial class FutureTile : Node2D
{
	[Export] public int x;
	[Export] public int y;
	[Export] public Sprite2D sprite2D;
	[Export] public Control control;
	[Export] public Sprite2D gem;
	[Export] Color hoverColor;


	Color normalColor;
	private bool isHovered = false;

	private GemType gemType;
	

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
	}

	public void setGemType(GemType gemType) {
		this.gemType = gemType;
		gem.Texture = gemType.getTexture2D();
	}

	public GemType getGemType() {
		return gemType;
	}

	public Vector2 getTilePosition() {
		return new Vector2(x,y);
	}



	public void mouseEnter() {
		isHovered = true;
		sprite2D.Modulate = getCurrentColor();

	}
	public void mouseExit() {
		isHovered = false;
		sprite2D.Modulate = getCurrentColor();
	}

	private Color getCurrentColor() {
		if (isHovered) {
			return hoverColor;
		}
		return normalColor;
	}

	public override void _ExitTree()
	{
		//FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= clearUIFocus;
	}
	

}
