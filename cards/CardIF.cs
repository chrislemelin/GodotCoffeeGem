using Godot;
using System;
using System.Collections.Generic;

public partial class CardIF : lerp{

	
	[Export] protected RichTextLabel titleLabel;
	[Export] protected RichTextLabel descriptionLabel;
	[Export] protected RichTextLabel costLabel;
	[Export] protected TextureRect picture;
	[Export] public HighlightOnHover highlightOnHover;
	[Export] protected CardResource cardResource;

	[Signal]
	public delegate void clickedSignalEventHandler(InputEvent inputEvent);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		setUpCard(cardResource);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public void setUpCard(CardResource cardResource)
	{
		this.cardResource = cardResource;

		if (cardResource.cardEffect.effectGemType != CardEffectGemType.None) {
			picture.Modulate = cardResource.cardEffect.effectGemType.GetGemType().GetColor();
		}


		titleLabel.Text = TextHelper.centered(cardResource.Title);
		descriptionLabel.Text = cardResource.Description;
		costLabel.Text = TextHelper.centered(cardResource.Cost.ToString());
	}

	public void playCard(MatchBoard matchboard, Hand hand, Mana mana, List<Vector2> tiles)
	{
		cardResource.cardEffect.doEffect(matchboard, hand, mana, tiles);
	}

	public virtual void listenToMouseEnter(Action function)
	{
	}

	public virtual void listenToMouseExit(Action function)
	{
	}


	public CardResource getCardResource()
	{
		return cardResource;
	}
}
