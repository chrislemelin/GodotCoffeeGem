using Godot;
using System;
using System.Collections.Generic;
using System.Security.Authentication;

public partial class CardIF : lerp
{
	[Export] protected RichTextLabel titleLabel;
	[Export] protected RichTextLabel descriptionLabel;
	[Export] protected RichTextLabel costLabel;
	[Export] protected TextureRect picture;
	[Export] public HighlightOnHover highlightOnHover;
	[Export] protected CardResource cardResource;
	[Export] protected Color disabledColor;
	[Export] protected TextureRect titleSprite;

	private bool enabled = true;


	[Signal]
	public delegate void clickedSignalEventHandler(InputEvent inputEvent);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public bool getEnabled() {
		return enabled || cardResource.playable;
	}

	public void setDisabled()
	{
		enabled = false;
		Modulate = disabledColor;
	}
	public void setEnabled()
	{
		enabled = true;
		Modulate = new Color(1, 1, 1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public void setCardResource(CardResource cardResource)
	{
		if (this.cardResource != null)
		{
			cardResource.cardEffect.CardPassivesChanged -= setUpCard;
		}
		this.cardResource = cardResource;
		setUpCard();
		cardResource.cardEffect.CardPassivesChanged += setUpCard;
	}

	private void setUpCard()
	{
		if (cardResource.cardEffect.effectGemType != CardEffectGemType.None)
		{
			picture.Modulate = cardResource.cardEffect.effectGemType.GetGemType().GetColor();
		}
		if (cardResource.Picture != null)
		{
			picture.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			picture.Texture = cardResource.Picture;
		}

		titleLabel.Text = TextHelper.centered(cardResource.Title);
		descriptionLabel.Text = cardResource.getDescription();
		costLabel.Text = TextHelper.centered(cardResource.getEnergyCostString());
		titleSprite.Modulate = cardResource.rarity.getColor();
		if (cardResource.cardEffect.consume) {
			highlightOnHover.TooltipText += "Consume cards will go away when played untill the end of the level";
		}
		if (cardResource.cardEffect.retain) {
			highlightOnHover.TooltipText += "Retain cards dont get discard at the end of turn";
		}
	}

	public void playCard(MatchBoard matchboard, Hand hand, Mana mana, List<Vector2> tiles)
	{
		cardResource.cardEffect.doEffect(matchboard, hand, mana, tiles, Optional.None<CardIF>());
	}

	public virtual void listenToMouseEnter(Action function)
	{
	}

	public virtual void listenToMouseExit(Action function)
	{
	}

	public void delete()
	{
		//cardResource.cardEffect.CardPassivesChanged -= setUpCard;
		QueueFree();
	}
	public CardResource getCardResource()
	{
		return cardResource;
	}
	protected override void Dispose(bool disposing)
	{
		if (cardResource != null ) {
			cardResource.cardEffect.CardPassivesChanged -= setUpCard;
		}
		base.Dispose(disposing);
	}

}
