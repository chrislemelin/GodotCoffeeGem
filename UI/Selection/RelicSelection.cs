using Godot;
using System;
using System.Collections.Generic;

public partial class RelicSelection : Control
{
	[Export] Control relicHolder;
	[Export] PackedScene relicUIPackedScene;
	[Export] Button skipButton;
	[Export] public bool purchasable;
	[Signal] public delegate void WindowClosedEventHandler();

	HashSet<RelicUI> relicUIs= new HashSet<RelicUI>();

	public override void _Ready()
	{
		skipButton.Pressed += () => {
			Visible = false;
			EmitSignal(SignalName.WindowClosed);
		};
	}
	public void setRelics(List<RelicResource> relicResources) {
		foreach (Node child in relicHolder.GetChildren())
		{
			child.QueueFree();
		};		
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
		gameManager.coinsChanged += (coinValue) => updatePurchasableRelicButtons(coinValue);
		foreach(RelicResource relicResource in relicResources) {
			RelicUI relicUI = (RelicUI)relicUIPackedScene.Instantiate();
			relicUI.showTitle = true;
			relicUI.setRelic(relicResource);
			relicUI.setShowPrice(purchasable);
			relicUIs.Add(relicUI);
			relicUI.buyButton.Disabled = !(gameManager.getCoins() >= relicResource.cost);

			MarginContainer marginContainer = new MarginContainer();
			marginContainer.AddThemeConstantOverride("margin_left", 10);
 			marginContainer.AddThemeConstantOverride("margin_right", 10);
			marginContainer.MouseFilter = MouseFilterEnum.Ignore;
			marginContainer.AddChild(relicUI);

			if (purchasable) {
				relicUI.buyButton.Pressed += () => {
					relicUIs.Remove(relicUI);
					relicUI.QueueFree();
					gameManager.addCoins(-1 * relicResource.cost);
				};
			} else {
				relicUI.hitBox.GuiInput += (inputEvent) => {
					if (inputEvent.IsActionPressed("click")) {
						FindObjectHelper.getGameManager(this).addRelic(relicResource);
						Visible = false;
						EmitSignal(SignalName.WindowClosed);
					}
				};
			}
			relicHolder.AddChild(marginContainer);
		}
	}

	private void updatePurchasableRelicButtons(int coinValue) {
		if (purchasable) {
			foreach(RelicUI relicUI in relicUIs) {
				RelicResource relicResource = relicUI.relicResource;
				relicUI.buyButton.Disabled = !(FindObjectHelper.getGameManager(this).getCoins() >= relicResource.cost);
			}
		}
	}
}
