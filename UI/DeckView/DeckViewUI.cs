using Godot;
using System;
using System.Collections.Generic;

public partial class DeckViewUI : ToggleVisibilityOnButtonPress
{
	[Export] PackedScene cardScene;
	[Export] PackedScene marginContainerScene;
	[Export] GridContainer gridContainer;
	[Export] ScrollContainer scrollContainer;
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
		button.Pressed += () => closeWindow();
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged += checkForFocus;
	}


	private void closeWindow() {
		if (cardCallBack != null) {
			if (deleting && cardSelected != null) {
				cardSelected.destroyCard();
				GetTree().CreateTimer(.5f).Timeout += () => cardCallBack(cardSelected.cardResource);
			} else {
				fadeOutDelay = 0.0f;
				if (cardSelected == null) {
					cardCallBack(null);
				} else {
					cardCallBack(cardSelected.cardResource);
				}
			}
		}
		setVisible(false);

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
			MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			marginContainer.AddChild(cardInfoLoader);
			gridContainer.AddChild(marginContainer);
			cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource, cardInfoLoader);
			cardInfoLoader.setForceHighlightOff(true);
			cardInfoLoader.setUpCard(cardResource);
			cardInfoLoader.cardScrollContainer = scrollContainer;
			cardInfoLoaders.Add(cardInfoLoader);
		}

		GD.Print("showing cards "+ cards.Count);
		setVisible(true);
		GetTree().CreateTimer(.25f).Timeout += () => {
			foreach(CardInfoLoader cardInfoLoader in cardInfoLoaders) {
				cardInfoLoader.setForceHighlightOff(false);
			}
			checkForFocus();
		};
	}

	private void checkForFocus(bool value) {
		if (value) {
			checkForFocus();
		}
	}

	private void checkForFocus() {
		if (FindObjectHelper.getControllerHelper(this).isUsingController() && cardInfoLoaders.Count > 0 && IsVisibleInTree()) {
			SettingsMenu settingsMenu = FindObjectHelper.getSettingsMenu(this);
			if (settingsMenu == null || !settingsMenu.isVisible()) {
				cardInfoLoaders[0].GrabFocus();
			}

		}
	}

	// public void setUp(List<CardResource> cards, Action<CardResource> cardCallBack, string title) {
		
	// 	setUp(cards, cardCallBack, title);
	// }

	public void setUp(List<CardResource> cards, Action<CardResource> cardCallBack, string title, bool deleting = false) {
		
		setUp(cards, cardCallBack, title, deleting, cardCallBack != null);
	}


	public void setUp(List<CardResource> cards, Action<CardResource> cardCallBack, string title, bool deleting, bool needToSelectCard) {
		
		this.cardCallBack = cardCallBack;
		if (!needToSelectCard) {
			button.Text = "Close";
			button.Disabled = false;
		} else {
			button.Text = "Confirm";
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
		if (InputHelper.isClick(inputEvent))
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
		if (InputHelper.isControllerAccept(inputEvent))
		{
			if(cardCallBack != null) {
				cardSelected = cardInfoLoader;
				closeWindow();
			}
		}
	}
	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= checkForFocus;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			if (Visible) {
				closeWindow();
			}
		}
	}

}
