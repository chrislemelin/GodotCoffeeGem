using Godot;
using System;
using System.Collections;

public partial class Gem : lerp
{
	[Export] public Sprite2D sprite2D;
	[Export] public Sprite2D addonSprite;

	[Export] public Texture2D manaAddonTexture;
	[Export] public Texture2D cardAddonTexture;
	[Export] public ShaderMaterial rainbowMaterial;

	[Export] AnimationPlayer animationPlayer;

	[Signal]
	public delegate void doneDyingSignalEventHandler(Gem gem);
	public GemType Type
	{
		get;
		private set;
	}

	public GemAddonType AddonType
	{
		get;
		private set;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Modulate = Type.GetColor();
		setAddonType(GemAddonType.None);
	}

	public void setType(GemType type)
	{
		Type = type;
		Modulate = type.GetColor();
		if (type == GemType.Rainbow) {
			this.sprite2D.Material = rainbowMaterial;
		} else {
			sprite2D.Material = null;
		}
	}

	public void startDying() {
		animationPlayer.Play("PopAnimation");
	}

	public void doneDying() {
		QueueFree();
		EmitSignal(SignalName.doneDyingSignal, this);
	}

	public void setAddonType(GemAddonType gemAddonType) {
		AddonType = gemAddonType;
		switch(gemAddonType) {
			case GemAddonType.None:
				addonSprite.Texture = null;
				break;
			case GemAddonType.Mana:
				addonSprite.Texture = manaAddonTexture;
				addonSprite.Scale = new Vector2(.4f,.4f);
				break;
			case GemAddonType.Card:
				addonSprite.Texture = cardAddonTexture;
				addonSprite.Scale = new Vector2(.2f,.2f);
				break;
		}
	}
}
