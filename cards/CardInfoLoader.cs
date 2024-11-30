using Godot;
using System;

public partial class CardInfoLoader : CustomToolTip
{
	[Export] public ScrollContainer cardScrollContainer;
	[Export] protected RichTextLabel titleLabel;
	[Export] protected RichTextLabel descriptionLabel;
	[Export] protected RichTextLabel costLabel;
	[Export] public CardResource cardResource;
	[Export] protected TextureRect picture;
	[Export] public TextureRect background;
	[Export] public TextureRect cardBack;
	[Export] public Control hitBox;


	[Export] protected TextureRect titleSprite;
	[Export] protected HighlightOnHover highlightOnHover;
	[Export] protected Control coinCostControl;
	[Export] protected RichTextLabel coinCostText;
	[Export] protected Color disabledColor;
	[Export] protected Color lockedColor;

	private bool disabled = false;
	public bool wiggleEnabled { get; private set; } = true;
	[Export] AnimationPlayer animationPlayer;
	private bool isLocked = false;


	public override void _Ready()
	{
		base._Ready();
		GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
		if (gameManagerIF.getDeckSelection() != null) {
			background.Texture = gameManagerIF.getDeckSelection().faceCardFront;
			if (cardBack != null) {
				cardBack.Texture = gameManagerIF.getDeckSelection().faceCard;

			}
		}
		Material = (Material)Material.Duplicate();

		FocusEntered += () => highlightOnHover.setForceHighlight(true);
		FocusEntered += () =>
		{
			if (cardScrollContainer != null)
			{
				cardScrollContainer.EnsureControlVisible(this);
			}
		};
		FocusExited += () => highlightOnHover.setForceHighlight(false);

		GrowHorizontal = GrowDirection.Both;
		GrowVertical = GrowDirection.Both;
		hitBox.MouseEntered += () => wiggleCard();
		setShowCoinCost(false);
	}

	public void wiggleCard()
	{
		if (!disabled && wiggleEnabled)
		{
			animationPlayer.Play("CardWiggle");
		}
	}

	public void flipCard()
	{
		animationPlayer.Play("CardFlip");
		wiggleEnabled = false;
		GetTree().CreateTimer(.6f).Timeout += () => animationPlayer.Play("Shine");
		GetTree().CreateTimer(1f).Timeout += () =>  {
			wiggleEnabled = true;
			highlightOnHover.makeBiggerBool = true;
		};
	}

	public void destroyCard()
	{
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

	public void resetAnimation()
	{
		animationPlayer.Play("RESET");
	}

	public void setUpLockedCard()
	{
		animationPlayer.Play("RESET");

		titleLabel.Text = "";
		descriptionLabel.Text = "";
		costLabel.Text = TextHelper.centered("");
		titleSprite.Modulate = CardRarity.Common.getColor();
		coinCostText.Text = "";
		isLocked = true;
		Modulate = lockedColor;
		toolTipText.Text = "This card is locked, play the game to unlock";
		return;
	}

	public void setUpCard(CardResource cardResource, bool locked = false)
	{
		animationPlayer.Play("RESET");

		this.cardResource = cardResource;
		this.cardResource.node = this;
		this.cardResource.cardEffect.node = this;

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
		descriptionLabel.Text = TextHelper.centered(cardResource.getDescription());
		costLabel.Text = TextHelper.centered(cardResource.getEnergyCostString());
		titleSprite.Modulate = cardResource.rarity.getColor();
		coinCostText.Text = cardResource.getCoinCost().ToString();
		//highlightOnHover.TooltipText = cardResource.getToolTip();
		toolTipText.Text = cardResource.getToolTip();
	}

	public void setShowCoinCost(bool visible)
	{
		coinCostControl.Visible = visible;
	}

	public void setForceHighlight(bool value)
	{
		highlightOnHover.setForceHighlight(value);
	}

	public void setForceHighlightOff(bool value)
	{
		highlightOnHover.setForceHighlightOff(value);
	}


}
