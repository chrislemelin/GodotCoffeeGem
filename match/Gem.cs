using Godot;
using System;
using System.Collections;

public partial class Gem : lerp
{
	[Export] public Sprite2D sprite2D;

	[Export] public Sprite2D addonSprite;
	[Export] public RichTextLabel comboTextLabel;

	[Export] public Texture2D manaAddonTexture;
	[Export] public Texture2D cardAddonTexture;
	[Export] public ShaderMaterial rainbowMaterial;
	[Export] public Texture2D orbTexture;
	[Export] public bool useSprites;
	[Export] public Control control;
	[Export] AnimationPlayer animationPlayer;

	private int comboValue = 1;

	private Color? startingModulate = null;

	[Signal]
	public delegate void doneDyingSignalEventHandler(Gem gem);
	[Export]
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
		if (startingModulate == null)
		{
			startingModulate = Modulate;
		}
		Type = type;
		if (useSprites && !(type == GemType.Black || type == GemType.Rainbow))
		{
			Modulate = startingModulate.Value;
			sprite2D.Texture = getTexture(type);
		}
		else
		{
			sprite2D.Texture = orbTexture;
			Modulate = type.GetColor();
		}
		if (type == GemType.Rainbow)
		{
			sprite2D.Material = rainbowMaterial;
		}
		else
		{
			sprite2D.Material = null;
		}

	}
	private Texture2D getTexture(GemType type)
	{
		switch (type)
		{
			case GemType.Coffee:
			case GemType.Milk:
			case GemType.Leaf:
			case GemType.Vanilla:
			case GemType.Sugar:
				return type.getTexture2D();
			default:
				return sprite2D.Texture;
		}
	}

	public void doAddonEffect()
	{
		switch (AddonType)
		{
			case GemAddonType.Mana:
				FindObjectHelper.getMana(this).modifyMana(1);
				break;
			case GemAddonType.Card:
				FindObjectHelper.getHand(this).drawCards(1);
				break;
		}
	}

	public void startDying()
	{
		animationPlayer.Play("PopAnimation");
	}

	public void doneDying()
	{
		QueueFree();
		EmitSignal(SignalName.doneDyingSignal, this);
	}

	public void addCombo(int value)
	{
		if (AddonType == GemAddonType.Combo)
		{
			comboValue += value;
			comboTextLabel.Text = "+" + comboValue;
		}
	}

	public int getCombo()
	{
		if (AddonType == GemAddonType.Combo)
		{
			return comboValue;
		}
		return 0;
	}

	public void setAddonType(GemAddonType gemAddonType)
	{
		AddonType = gemAddonType;
		switch (gemAddonType)
		{
			case GemAddonType.None:
				addonSprite.Texture = null;
				control.TooltipText = "";
				break;
			case GemAddonType.Mana:
				addonSprite.Visible = true;
				addonSprite.Texture = manaAddonTexture;
				addonSprite.Scale = new Vector2(.3f, .3f);
				control.TooltipText = "Gain a mana when this ingredient is matched";
				break;
			case GemAddonType.Card:
				addonSprite.Visible = true;
				addonSprite.Texture = cardAddonTexture;
				addonSprite.Scale = new Vector2(.5f, .5f);
				control.TooltipText = "Draw a card when this ingredient is matched";
				break;
			case GemAddonType.Combo:
				comboTextLabel.Visible = true;
				comboValue = 1;
				comboTextLabel.Text = "+" + comboValue;
				control.TooltipText = "Doesnt disapear on match and gives extra points everytime this is matched";
				break;
		}
	}
}
