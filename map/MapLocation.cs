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
	private bool done = false;
	//private GameManagerIF gameManager;

	public override void _Ready()
	{
		base._Ready();
		setEventType(type);
		setActive(active);
		FindObjectHelper.getGameManager(this);
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

	public void resolveEvent(MapEventResolveUI mapEventResolveUI)
	{
		if (type == MapEventType.Money)
		{
			mapEventResolveUI.setUp("Found Money", "You found some money on the ground! Its almost as much as a full days work. Gain 15 coins");
			mapEventResolveUI.button.Pressed += () => addActionToContinueButton(() => FindObjectHelper.getGameManager(this).addCoins(15));
		}
		else if (type == MapEventType.UpgradeCard)
		{
			mapEventResolveUI.setUp("Upgrade a Card", "You walk through a park on your way back home. Reconnecting with nature relieves some stress of the workday. " +
			"A random horizontal or vertical switch card has been upgraded!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				upgradeCard()
			);
		}
		else if (type == MapEventType.GainCard)
		{
			mapEventResolveUI.setUp("Gain a New Card", "You see an old friend on the street and stop to say hello. " +
			"They have really risen up the corporate ladder since you last time you saw them. They give you a few tips to help you out!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				FindObjectHelper.getCardSelection(this).getRandomCardsToSelectFrom()
			);
		}
		else if (type == MapEventType.RemoveCard)
		{
			mapEventResolveUI.setUp("Remove a Card", "You decide to stop at the bar on the way from home to drown out your sorrows. " +
			"You can feel the trama leaving your soul as you take your first sip. Remove a card from your Deck!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				FindObjectHelper.getDeckView(this).setUp(FindObjectHelper.getGameManager(this).getDeckList(),
				 (CardResource cardResource) => FindObjectHelper.getGameManager(this).removeCardFromDeckList(cardResource),
				 TextHelper.centered("Remove Card From Deck"), true
			));
		}
		else if (type == MapEventType.Heal)
		{
			GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
			mapEventResolveUI.setUp("Get your head fixed", "You stop by a free clinic on your way home, you have been feeling sick but cant afford to take off work." +
			"You wait about 3 hours before the doctor sees you for about 5 minutes. She writes you a prescription and sends you on your way. Heal 1 heart!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				gameManagerIF.setHealth(gameManagerIF.getHealth() + 1)
			);
		}
		else if (type == MapEventType.Mechanic)
		{
			GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
			if (gameManagerIF.getCoins() >= 40)
			{
				bool currentBoardGooed = gameManagerIF.getGooRightRow();
				gameManagerIF.addCoins(-40);
				if (currentBoardGooed)
				{
					mapEventResolveUI.setUp("Upgrade Coffee Machine", "The mechanic takes a look at your coffee machine, he removes the starting Goo from the machine");
					mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
						gameManagerIF.setGooRightRow(false)
					);
				}
				else
				{
					mapEventResolveUI.setUp("Upgrade Coffee Machine", "The mechanic takes a look at your coffee machine, he adds an additional row to the machine, " +
						"but that row will start with Goo");
					mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
					{
						gameManagerIF.changeGridSize(gameManagerIF.getGridSize() + new Vector2(1, 0));
						gameManagerIF.setGooRightRow(true);
					});
				}
			}
			else
			{
				mapEventResolveUI.setUp("You Broke", "You are too broke to visit the mechanic. RIP BROKIE");
			}
		}
		else if (type == MapEventType.RelicShop)
		{
			GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
			if (gameManagerIF.getCoins() >= 20)
			{
				gameManagerIF.addCoins(-20);
				mapEventResolveUI.setUp("Relic shop", "You stumble into a the relic shop, they have items that should help your run");
				mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				{
					List<RelicResource> relicResources = gameManagerIF.getRelicPool();
					RandomHelper.Shuffle(relicResources);
					List<RelicResource> relicsInSelection = relicResources.GetRange(0, Math.Min(3, relicResources.Count));
					RelicSelection relicSelection = FindObjectHelper.getRelicSelection(this);
					relicSelection.setRelics(relicsInSelection);
				});


			}
			else
			{
				mapEventResolveUI.setUp("You Broke", "You are too broke to visit the relic shop. RIP BROKIE");
			}
		}
		else if (type == MapEventType.Home)
		{
			mapEventResolveUI.setUp("Gain a New Card", "You see an old friend on the street right before you are about to head home. " +
			"They have really risen up the corporate ladder since you last time you saw them. They give you a few tips to help you out!");
			mapEventResolveUI.WindowClosedSignal += () => addActionToContinueButton(() =>
				FindObjectHelper.getCardSelection(this).getRandomCardsToSelectFrom()
			);
			FindObjectHelper.getCardSelection(this).windowClosed += (card) => FindObjectHelper.getGameManager(this).advanceLevel();
		}
	}

	private void addActionToContinueButton(Action action)
	{
		if (!done)
		{
			done = true;
			action.Invoke();
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
