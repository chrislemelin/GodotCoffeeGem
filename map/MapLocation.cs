using Godot;
using System;
using System.Linq;

public partial class MapLocation : Control
{
	[Export] TextureRect image;
	[Export] RichTextLabel label;
	[Export] MapEventType type;
	[Export] HighlightOnHover highlight;
	[Export] Color inActiveColor;
	[Export] public bool active {get; private set;}

	[Export] CardResource vertSwitchCard;
	[Export] CardResource horizSwitchCard;
	[Export] CardResource upgradedSwitchCard;
	private bool done = false;
	//private GameManagerIF gameManager;

	public override void _Ready() 
	{
		base._Ready();
		setEventType(type);
		setActive(active);
		FindObjectHelper.getGameManager(this);
	}

	public void setEventType(MapEventType eventType) {
		type = eventType;
		label.Text = TextHelper.centered(type.getString());
	} 

	public void setActive(bool value) {
		active = value;
		if(active) {
			Modulate = new Color(1.0f, 1.0f,1.0f, 1.0f);
			highlight.setForceHighlightOff(false);
		} else {
			Modulate = inActiveColor;
			highlight.setForceHighlightOff(true);
		}
	}

	public void resolveEvent(MapEventResolveUI mapEventResolveUI) {
		if (type == MapEventType.Money) {
			mapEventResolveUI.setUp("Found Money", "You found some money on the ground! Its almost as much as a full days work. Gain 15 coins");
			mapEventResolveUI.button.Pressed += () => addActionToContinueButton(()=> FindObjectHelper.getGameManager(this).addCoins(15));
		} else if (type == MapEventType.UpgradeCard) {
			mapEventResolveUI.setUp("Upgrade a Card", "You walk through a park on your way back home. Reconnecting with nature relieves some stress of the workday. " +
			"A random horizontal or vertical switch card has been upgraded!");
			mapEventResolveUI.button.Pressed += () => addActionToContinueButton(()=> 
				upgradeCard()
			);
		}
		else if (type == MapEventType.GainCard) {
			mapEventResolveUI.setUp("Gain a New Card", "You see an old friend on the street and stop to say hello. " +
			"They have really rissen up the coperate ladder since you last saw. They give you a few tips to help you out!");
			mapEventResolveUI.button.Pressed += () => addActionToContinueButton(()=> 
				FindObjectHelper.getCardSelection(this).getRandomCardsToSelectFrom()
			);
		}
		else if (type == MapEventType.RemoveCard) {
			mapEventResolveUI.setUp("Remove a Card", "You decide to stop at the bar on the way from home to drown out your sorrows. " +
			"You can feel the trama leaving your soul as you take your first sip. Remove a card from your Deck!");
			mapEventResolveUI.button.Pressed += () => addActionToContinueButton(()=> 
				FindObjectHelper.getDeckView(this).setUp(FindObjectHelper.getGameManager(this).getDeckList(),
				 (CardResource cardResource) => FindObjectHelper.getGameManager(this).removeCardFromDeckList(cardResource), 
				 TextHelper.centered("Remove Card From Deck")
			));
		}
		else if (type == MapEventType.Home) {
			FindObjectHelper.getGameManager(this).advanceLevel();
		}
	}

	private void addActionToContinueButton(Action action) {
		if (!done) {
			done = true;
			action.Invoke();
		}
	}

	private void upgradeCard() {
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
		CardResource cardToReplace = gameManager.getDeckList().ToList().Find(cardResource => cardResource.equalToCard(vertSwitchCard) || cardResource.equalToCard(horizSwitchCard));
		if (cardToReplace != null) {
			gameManager.removeCardFromDeckList(cardToReplace);
			gameManager.addCardToDeckList(upgradedSwitchCard);
		}
	}



}