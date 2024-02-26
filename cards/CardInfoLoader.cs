using Godot;
using System;

public partial class CardInfoLoader : Control
{
	[Export] protected RichTextLabel titleLabel;
	[Export] protected RichTextLabel descriptionLabel;
	[Export] protected RichTextLabel costLabel;
	[Export] public CardResource cardResource;
	[Export] protected TextureRect picture;
	[Export] protected TextureRect titleSprite;
	[Export] protected HighlightOnHover highlightOnHover;
	[Export] protected Control coinCostControl;
	[Export] protected RichTextLabel coinCostText;
	[Export] protected Color disabledColor;
	private bool disabled = false;
	public bool wiggleEnabled {get; private set;} = true;
	[Export] AnimationPlayer animationPlayer;



	public override void _Ready()
	{
		base._Ready();
		Material = (Material)Material.Duplicate();

		GrowHorizontal = GrowDirection.Both;
		GrowVertical = GrowDirection.Both;
		MouseEntered += () => wiggleCard();
	}

	public void wiggleCard() {
		if (!disabled && wiggleEnabled) {
			animationPlayer.Play("CardWiggle");
		}
	}

	public void flipCard() {
		animationPlayer.Play("CardFlip");
		wiggleEnabled = false;
		GetTree().CreateTimer(.5f).Timeout += () => wiggleEnabled = true;
	}

	public void destroyCard() {
		wiggleEnabled = false;
		setShowCoinCost(false);
		animationPlayer.Play("Destroy");
	}


	public void setDisabled()
	{
		Modulate = disabledColor;
		disabled = true;
		highlightOnHover.setForceHighlightOff(true);
	}
	public void setEnabled()
	{
		Modulate = new Color(1, 1, 1);
		disabled = false;
		highlightOnHover.setForceHighlightOff(false);
	}

	public void resetAnimation() {
		animationPlayer.Play("RESET");
	}

	public void setUpCard(CardResource cardResource)
	{
		animationPlayer.Play("RESET");
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
		costLabel.Text = TextHelper.centered(cardResource.getEnergyCost().ToString());
		titleSprite.Modulate = cardResource.rarity.getColor();
		coinCostText.Text = cardResource.getCoinCost().ToString();
		if (cardResource.cardEffect.consume) {
			highlightOnHover.TooltipText += "Consume cards will go away when played untill the end of the level";
		}
		if (cardResource.cardEffect.retain) {
			highlightOnHover.TooltipText += "Retain cards dont get discard at the end of turn";
		}
	}

	public void setShowCoinCost(bool visible) {
		coinCostControl.Visible = visible;
	}

	public void setForceHighlight(bool value) {
		highlightOnHover.setForceHighlight(value);
	}

	public void setForceHighlightOff(bool value) {
		highlightOnHover.setForceHighlightOff(value);
	}
	

}
