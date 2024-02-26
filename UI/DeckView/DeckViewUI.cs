using Godot;
using System;
using System.Collections.Generic;

public partial class DeckViewUI : ToggleVisibilityOnButtonPress
{
	[Export] PackedScene cardScene;
	[Export] PackedScene marginContainerScene;
	[Export] GridContainer gridContainer;
	[Export] RichTextLabel title;
	[Export] float deletingFadeOutDelay = .25f;

	private bool deleting = false;

	private List<CardInfoLoader> cardInfoLoaders = new List<CardInfoLoader>();

	private Action<CardResource> cardCallBack;
	private CardInfoLoader cardSelected;

	public override void _Ready()
	{
		base._Ready();
		resetAnimation();
		button.Pressed += () => {
			if (cardCallBack != null) {
				if (deleting) {
					cardSelected.destroyCard();
					GetTree().CreateTimer(.5f).Timeout += () => cardCallBack(cardSelected.cardResource);
				} else {
					cardCallBack(cardSelected.cardResource);
				}
			}
		};
	}

	private void setUpInternal(List<CardResource> cards) {
		cardInfoLoaders.Clear();
		Godot.Collections.Array<Node> nodes = gridContainer.GetChildren();
		foreach (Node node in nodes)
		{
			gridContainer.RemoveChild(node);
			node.QueueFree();
		}

		foreach(CardResource cardResource in cards) {
			CardInfoLoader cardInfoLoader = (CardInfoLoader)cardScene.Instantiate();
			cardInfoLoaders.Add(cardInfoLoader);
			cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource, cardInfoLoader);
			cardInfoLoader.setForceHighlightOff(true);
			cardInfoLoader.setUpCard(cardResource);
			MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			marginContainer.AddChild(cardInfoLoader);
			gridContainer.AddChild(marginContainer);
		}
		setVisible(true);
		GetTree().CreateTimer(.25f).Timeout += () => {
			foreach(CardInfoLoader cardInfoLoader in cardInfoLoaders) {
				cardInfoLoader.setForceHighlightOff(false);
			}
		};
	}

	public void setUp(List<CardResource> cards, Action<CardResource> cardCallBack, string title) {
		
		setUp(cards, cardCallBack, title, false);
	}

	public void setUp(List<CardResource> cards, Action<CardResource> cardCallBack, string title, bool deleting) {
		
		this.cardCallBack = cardCallBack;
		if (cardCallBack == null) {
			button.Text = "close";
			button.Disabled = false;
		} else {
			button.Text = "confirm";
			button.Disabled = true;
		}
		this.title.Text = title;
		this.deleting = deleting;
		if (deleting) {
			fadeOutDelay = deletingFadeOutDelay;
		} else {
			fadeOutDelay = 0;
		}
		setUpInternal(cards);
	}


	public void setUp(List<CardResource> cards) {
		setUp(cards, null, "");
	}

	private void cardClicked(InputEvent inputEvent, CardResource cardResource, CardInfoLoader cardInfoLoader) {
		if (inputEvent.IsActionPressed("click"))
		{
			if(cardCallBack != null) {
				cardSelected = cardInfoLoader;
				button.Disabled = false;
				foreach(CardInfoLoader currentCardInfoLoader in cardInfoLoaders) {
					if(currentCardInfoLoader == cardInfoLoader) {
						currentCardInfoLoader.setForceHighlight(true);
					} else {
						currentCardInfoLoader.setForceHighlight(false);
					}
				}
			}
		};
	}

}
