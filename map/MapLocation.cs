using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot.Collections;

public partial class MapLocation : Control
{
	[Export] TextureRect image;
	[Export] RichTextLabel label;
	[Export] MapEventType type;
	[Export] HighlightOnHover highlight;
	[Export] Color inActiveColor;
	[Export] public bool active { get; private set; }
	[Export] Array<MapEventType> mapTypes = new Array<MapEventType>();
	[Export] Array<Texture2D> mapTextures = new Array<Texture2D>();

	[Export] CardResource vertSwitchCard;
	[Export] CardResource horizSwitchCard;
	[Export] CardResource upgradedSwitchCard;
	[Export] public MapLocation pairMapLocation;
	private bool done = false;
	private bool callbackDone = false;

	//private GameManagerIF gameManager;

	public override void _Ready()
	{
		base._Ready();
		setEventType(type);
		setActive(active);
		FindObjectHelper.getGameManager(this);
		MouseEntered += () => {
			if (pairMapLocation!=null) {
				pairMapLocation.highlight.setForceHighlight(true);
			}
		};
		MouseExited += () => {
			if (pairMapLocation!=null) {
				pairMapLocation.highlight.setForceHighlight(false);
			}};
	}

	public void setPair(MapLocation mapLocation) {
		pairMapLocation = mapLocation;
	}

	public void setEventType(MapEventType eventType)
	{
		type = eventType;
		label.Text = TextHelper.centered(type.getString());
		setImage();
	}

	private void setImage()
	{
		for (int typeIndex = 0; typeIndex < mapTypes.Count; typeIndex++)
		{
			if (mapTypes[typeIndex] == type)
			{
				image.Texture = mapTextures[typeIndex];
			}
		}
	}

	public void setActive(bool value)
	{
		active = value;
		if (active)
		{
			Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			highlight.setForceHighlightOff(false);
		}
		else
	{
			Modulate = inActiveColor;
			highlight.setForceHighlightOff(true);
		}
	}

	
	public void setHighlight(bool value)
	{
		highlight.setForceHighlight(value);
	}

	public void resolveEvent(MapEventResolveUI mapEventResolveUI, Action callback)
	{
		if (type == MapEventType.Money)
		{
			mapEventResolveUI.setUp("Found Money", "You found some money on the ground! It's almost as much as a full day's work. Gain 15 coins.");
			mapEventResolveUI.button.Pressed += () => addActionToContinueButton(() => FindObjectHelper.getGameManager(this).addCoins(15),callback);
		}
		else if (type == MapEventType.UpgradeCard)
		{
			mapEventResolveUI.setUp("Upgrade a Card", "You walk through a park on your way back home. Reconnecting with nature relieves some stress of the workday. " +
			"A random horizontal or vertical switch card has been upgraded!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				upgradeCard()
			,callback);
		}
		else if (type == MapEventType.GainCard)
		{
			mapEventResolveUI.setUp("Gain a New Card", "You see an old friend on the street and stop to say hello. " +
			"They have really risen up the corporate ladder since the last time you saw them. They give you a few tips to help you out!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() => {
				NewCardSelection cardSelection = FindObjectHelper.getCardSelection(this);
				cardSelection.windowClosed += (card) => addCallBackAction(callback);
				cardSelection.getRandomCardsToSelectFrom();
			}
			,null);
		}
		else if (type == MapEventType.RemoveCard)
		{
			mapEventResolveUI.setUp("Remove a Card", "You decide to stop at the bar on the way home for a quick drink." +
			"You drank a bit too much and end up losing something, although you can't remember what it was. Remove a card from your Deck!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				FindObjectHelper.getDeckView(this).setUp(FindObjectHelper.getGameManager(this).getDeckList(),
				 (CardResource cardResource) =>  {
					FindObjectHelper.getGameManager(this).removeCardFromDeckList(cardResource);
					callback.Invoke();
				 },
				 TextHelper.centered("Remove Card From Deck"), true
			), null);
		}
		else if (type == MapEventType.Heal)
		{
			GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
			mapEventResolveUI.setUp("Get your head fixed", "You stop by a free clinic on your way home, you have been feeling sick but cant afford to take off work." +
			"You wait about 3 hours before the doctor sees you for about 5 minutes. She writes you a prescription and sends you on your way. Heal 1 heart!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				gameManagerIF.setHealth(gameManagerIF.getHealth() + 1)
			,callback);
		}
		else if (type == MapEventType.Mechanic)
		{
			GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
			if (gameManagerIF.getCoins() >= 50)
			{
				bool currentBoardGooed = gameManagerIF.getGooRightRow();
				gameManagerIF.addCoins(-50);
				if (currentBoardGooed)
				{
					mapEventResolveUI.setUp("Upgrade Coffee Machine", "The mechanic takes a look at your coffee machine and tweaks a few things. Goo has been removed from the new column of the machine");
					mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
						gameManagerIF.setGooRightRow(false)
					,callback);
				}
				else
				{
					mapEventResolveUI.setUp("Upgrade Coffee Machine", "The mechanic takes a look at your coffee machine. He adds an additional column to the machine, " +
						"but that column will start with Goo");
					mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
					{
						gameManagerIF.addGridUpgrade();
						gameManagerIF.setGooRightRow(true);
					},callback);
				}
			}
			else
			{
				mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() => { }, callback);
				mapEventResolveUI.setUp("You Broke", "You are too broke to visit the mechanic.");
			}
		}
		else if (type == MapEventType.RelicShop)
		{
			GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
			if (gameManagerIF.getCoins() >= 40)
			{
				gameManagerIF.addCoins(-40);
				mapEventResolveUI.setUp("Relic shop", "You stumble into the relic shop to see what mysterious new artifacts they've gathered");
				mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				{
					List<RelicResource> relicResources = gameManagerIF.getRelicPool();
					RandomHelper.Shuffle(relicResources);
					List<RelicResource> relicsInSelection = relicResources.GetRange(0, Math.Min(3, relicResources.Count));
					RelicSelection relicSelection = FindObjectHelper.getRelicSelection(this);
					relicSelection.setRelics(relicsInSelection, true);
					relicSelection.WindowClosed += () => addCallBackAction(callback);
				},null);


			}
			else
			{
				mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() => { }, callback);
				mapEventResolveUI.setUp("You Broke", "You are too broke to visit the relic shop.");
			}
		}
		else if (type == MapEventType.Home)
		{
			mapEventResolveUI.setUp("Gain a New Card", "The wizened old downstairs neighbor offers you one of his signature pieces of wisdom as you return home. " +
			"It may have come from a guy yelling from his porch, but you feel like you could put this to use!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				FindObjectHelper.getCardSelection(this).getRandomCardsToSelectFrom(),callback
			);
			FindObjectHelper.getCardSelection(this).windowClosed += (card) => FindObjectHelper.getGameManager(this).advanceLevel();
		}
	}

	private void addActionToContinueButton(Action action, Action callback)
	{
		if (!done)
		{
			done = true;
			action.Invoke();
			callback?.Invoke();
		}
	}

	private void addCallBackAction(Action callback)
	{
		if (!callbackDone)
		{
			callbackDone = true;
			callback.Invoke();
		}
	}

	private void upgradeCard()
	{
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
		CardResource cardToReplace = gameManager.getDeckList().ToList().Find(cardResource => cardResource.equalToCard(vertSwitchCard) || cardResource.equalToCard(horizSwitchCard));
		if (cardToReplace != null)
		{
			gameManager.removeCardFromDeckList(cardToReplace);
			gameManager.addCardToDeckList(upgradedSwitchCard);
		}
	}



}
