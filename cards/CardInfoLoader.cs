using Godot;
using System;

public partial class CardInfoLoader : Control
{
	[Export] protected RichTextLabel titleLabel;
	[Export] protected RichTextLabel descriptionLabel;
	[Export] protected RichTextLabel costLabel;
	[Export] protected CardResource cardResource;
	[Export] protected TextureRect picture;
	[Export] protected TextureRect titleSprite;
	[Export] protected HighlightOnHover highlightOnHover;



	public override void _Ready()
	{
		base._Ready();
		//setUpCard(cardResource);
	}

	public void setUpCard(CardResource cardResource)
	{
		this.cardResource = cardResource;

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
		costLabel.Text = TextHelper.centered(cardResource.getCost().ToString());
		titleSprite.Modulate = cardResource.rarity.getColor();
	}

	public void setForceHighlight(bool value) {
		highlightOnHover.setForceHighlight(value);
	}

}
