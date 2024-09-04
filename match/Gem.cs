using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Gem : lerp
{
	[Export] public Sprite2D sprite2D;
	[Export] public Sprite2D addonSprite;
	[Export] public RichTextLabel comboTextLabel;
	[Export] public Texture2D manaAddonTexture;
	[Export] public Texture2D cardAddonTexture;
	[Export] public Texture2D lockAddonTexture;
	[Export] public Texture2D coinAddonTexture;
	[Export] public Texture2D metaCoinAddonTexture;
	[Export] public ShaderMaterial rainbowMaterial;
	[Export] public AudioPlayer audioPlayer;
	[Export] public Texture2D orbTexture;
	[Export] public bool useSprites;
	[Export] public Control control;
	[Export] AnimationPlayer animationPlayer;
	[Export] PackedScene explosion;
	[Export] PackedScene shimmer;


	List<Action<List<Gem>>> matchActions = new List<Action<List<Gem>>>();
	[Export] Texture2D questionMark;

	//List<Tuple<Action<List<GemType>>, Boolean>> matchCallBacks = new List<Tuple<Action<List<GemType>>, Boolean>>();

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

	public void clearMatchActions() {
		matchActions.Clear();
	}

	public void addMatchActions(Action<List<Gem>> action) {
		matchActions.Add(action);
	}

	public void executeMatchActions(List<Gem> gems) {
		foreach(Action<List<Gem>> action in matchActions) {
			action.Invoke(gems);
		}
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
		updateSprite();
	}

	private void updateSprite() {
		sprite2D.Material = (Material)sprite2D.Material.Duplicate();
		if (AddonType == GemAddonType.Hidden) {
			sprite2D.Texture = questionMark;
			sprite2D.Scale = new Vector2(.55f,.55f);
			//Modulate = new Color(0,0,0,1);
			return;
		}
		if (startingModulate == null)
		{
			startingModulate = Modulate;
		}
		if (useSprites && !(Type == GemType.Rainbow))
		{
			Modulate = startingModulate.Value;
			sprite2D.Texture = getTexture(Type);
		}
		else
		{
			sprite2D.Texture = getTexture(GemType.Sugar);
			Modulate = Type.GetColor();
		}
		if (Type == GemType.Rainbow)
		{
			sprite2D.Material = rainbowMaterial;
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
			case GemType.Black:
				return type.getTexture2D();
			default:
				return sprite2D.Texture;
		}
	}

	public void doMatchEffects(List<Gem> gems) {
		doAddonEffect();
		executeMatchActions(gems);
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
			case GemAddonType.Money:
				FindObjectHelper.getGameManager(this).addCoins(5);
				break;
			case GemAddonType.MetaCoin:
				FindObjectHelper.getGameManager(this).addMetaCoins(2);
				break;
		}
	}

	public void startDyingMatch()
	{
		animationPlayer.Play("PopAnimation");
		Node shimmerNode = shimmer.Instantiate();
		shimmerNode.GetChild<GpuParticles2D>(0,false).Emitting = true;
		this.AddChild(shimmerNode);
	}

	public void explode() {
		Explosion explode = (Explosion)explosion.Instantiate();
		explode.Position = new Vector2(125,125);
		this.AddChild(explode);
	}

	public void startDying()
	{
		animationPlayer.Play("PopAnimation");
	}

	public void shine()
	{
		resetAnimation();
		animationPlayer.Play("Shine");
	}

	public void shake()
	{
		resetAnimation();
		audioPlayer.Play();
		animationPlayer.Play("Shake");
	}

	public void resetAnimation() {
		animationPlayer.Play("RESET");
	}

	public void startDyingExplode()
	{
		animationPlayer.Play("PopAnimation");
		explode();
	}


	public void doneDying()
	{
		GetTree().CreateTimer(1).Timeout += () => QueueFree();
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
				addonSprite.Scale = new Vector2(13f, 13f);
				control.TooltipText = "Gain a mana when this ingredient is matched";
				break;
			case GemAddonType.Card:
				addonSprite.Visible = true;
				addonSprite.Texture = cardAddonTexture;
				addonSprite.Scale = new Vector2(10f, 10f);
				control.TooltipText = "Draw a card when this ingredient is matched";
				break;
			case GemAddonType.Combo:
				comboTextLabel.Visible = true;
				comboValue = 1;
				comboTextLabel.Text = "+" + comboValue;
				control.TooltipText = "Doesnt disapear on match and gives extra points everytime this is matched";
				break;
			case GemAddonType.Lock:
				addonSprite.Visible = true;
				addonSprite.Texture = lockAddonTexture;
				addonSprite.Scale = new Vector2(.3f, .3f);
				control.TooltipText = "Cannot move ingredient";
				break;
			case GemAddonType.Money:
				addonSprite.Visible = true;
				addonSprite.Texture = coinAddonTexture;
				addonSprite.Scale = new Vector2(1.25f, 1.25f);
				control.TooltipText = "Cannot move ingredient";
				break;
			case GemAddonType.MetaCoin:
				addonSprite.Visible = true;
				addonSprite.Texture = metaCoinAddonTexture;
				addonSprite.Scale = new Vector2(1.25f, 1.25f);
				control.TooltipText = "Cannot move ingredient";
				break;
		}
		updateSprite();
	}
}
