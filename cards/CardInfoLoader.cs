using Godot;
using System;

public partial class CardInfoLoader : Control
{
	[Export] protected RichTextLabel titleLabel;
	[Export] protected RichTextLabel descriptionLabel;
	[Export] protected RichTextLabel costLabel;
	[Export] protected CardResource cardResource;
	[Export] protected TextureRect picture;


	public override void _Ready()
	{
		base._Ready();
		setUpCard(cardResource);
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

}
