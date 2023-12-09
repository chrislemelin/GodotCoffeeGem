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
	[Export] protected Color disabledColor;




	[Signal]
	public delegate void clickedSignalEventHandler(InputEvent inputEvent);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public void setDisabled() {
		Modulate = disabledColor;
	}
	public void setEnabled(){
		Modulate = new Color(1,1,1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public void setCardResource(CardResource cardResource) {
		if (this.cardResource != null) {
			cardResource.cardEffect.CardPassivesChanged -= setUpCard;
		}
		this.cardResource = cardResource;
		setUpCard();
		cardResource.cardEffect.CardPassivesChanged += setUpCard;
	}

	private void setUpCard()
	{
//		if (GetTree().)
		if (cardResource.cardEffect.effectGemType != CardEffectGemType.None) {
			picture.Modulate = cardResource.cardEffect.effectGemType.GetGemType().GetColor();
		}


		titleLabel.Text = TextHelper.centered(cardResource.Title);
		descriptionLabel.Text = cardResource.getDescription();
		costLabel.Text = TextHelper.centered(cardResource.getCost().ToString());
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

	public void delete() {
		//cardResource.cardEffect.CardPassivesChanged -= setUpCard;
		QueueFree();
	}
	public CardResource getCardResource()
	{
		return cardResource;
	}
	protected override void Dispose(bool disposing) {
		//if (cardResource )
		cardResource.cardEffect.CardPassivesChanged -= setUpCard;
		base.Dispose(disposing);
	}

}
