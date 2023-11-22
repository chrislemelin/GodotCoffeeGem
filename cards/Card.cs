using Godot;
using System;
using System.Collections.Generic;

public partial class Card : lerp
{

	[Export] RichTextLabel titleLabel;
	[Export] RichTextLabel descriptionLabel;
	[Export] RichTextLabel costLabel;
	[Export] public Area2D area2D;
	[Export]
	public HighlightOnHover highlightOnHover;
	[Export] CardResource cardResource;


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

		titleLabel.Text = TextHelper.centered(cardResource.Title);
		descriptionLabel.Text = cardResource.Description;
		costLabel.Text = TextHelper.centered(cardResource.Cost.ToString());
	}

	public void playCard(MatchBoard matchboard, Mana mana, List<Vector2> tiles)
	{
		cardResource.cardEffect.doEffect(matchboard, mana, tiles);
	}

	public CardResource getCardResource()
	{
		return cardResource;
	}
}
