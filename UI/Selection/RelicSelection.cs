using Godot;
using System;
using System.Collections.Generic;

public partial class RelicSelection : Control
{
	[Export] Control relicHolder;
	[Export] PackedScene relicUIPackedScene;
	[Export] CustomButton skipButton;
	[Export] public bool purchasable;
	[Signal] public delegate void WindowClosedEventHandler();
	public bool hasSetUpRelics = false;

	HashSet<RelicUI> relicUIs= new HashSet<RelicUI>();

	public override void _Ready()
	{
		skipButton.Pressed += () => {
			Visible = false;
			EmitSignal(SignalName.WindowClosed);
		};
		FindObjectHelper.getGameManager(this).coinsChanged += (coins) => updateRelicDisabled(); 
	}

	public void setToVisible() {
		Visible = true;
		skipButton.checkForFocus();
	}

	public void setRelics(List<RelicResource> relicResources, bool free) {
		Visible = true;
		hasSetUpRelics = true;
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
			relicUIs.Add(relicUI);

			MarginContainer marginContainer = new MarginContainer();
			marginContainer.AddThemeConstantOverride("margin_left", 25);
 			marginContainer.AddThemeConstantOverride("margin_right", 25);
			marginContainer.MouseFilter = MouseFilterEnum.Ignore;
			marginContainer.AddChild(relicUI);

			if (!free) {
				relicUI.setShowPrice(!free);
				relicUI.setDisable(!(gameManager.getCoins() >= relicResource.cost));
				relicUI.GuiInput += (inputEvent) => {
					relicBoughtClicked(inputEvent,marginContainer, relicUI);
				};
				relicUI.hitBox.GuiInput += (inputEvent) => {
					relicBoughtClicked(inputEvent,marginContainer, relicUI);
				};
				skipButton.Text = "Return";
			} else {
				// this hit box is just too big
				// relicUI.GuiInput += (inputEvent) => {
				// 	relicClicked(inputEvent, relicUI);
				// };
				relicUI.hitBox.GuiInput += (inputEvent) => {
					relicClicked(inputEvent, relicUI);
				};
			}
			relicHolder.AddChild(marginContainer);
		}
		skipButton.checkForFocus();
	}

	private void updateRelicDisabled() {
		GameManagerIF gameManager = FindObjectHelper.getGameManager(this);
		foreach(RelicUI relicUI in relicUIs) {
			relicUI.setDisable(!(gameManager.getCoins() >= relicUI.relicResource.cost));
		}

	}

	private void relicClicked(InputEvent inputEvent, RelicUI relicUI)
	{
		if (InputHelper.isSelectedAction(inputEvent))
		{
			FindObjectHelper.getGameManager(this).addRelic(relicUI.relicResource);
			Visible = false;
			EmitSignal(SignalName.WindowClosed);
		};
	}
	private void relicBoughtClicked(InputEvent inputEvent, MarginContainer marginContainer, RelicUI relicUI)
	{
		if (InputHelper.isSelectedAction(inputEvent) && !relicUI.getDisabled()) {
			FindObjectHelper.getGameManager(this).addCoins(-1 * relicUI.relicResource.cost);
			FindObjectHelper.getGameManager(this).addRelic(relicUI.relicResource);
			relicUIs.Remove(relicUI);
			marginContainer.RemoveChild(relicUI);
			relicHolder.RemoveChild(marginContainer);
			relicUI.QueueFree();
			skipButton.checkForFocus();
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
	// private void setUIFocus() {
	// 	setUIFocus(true);
	// }

	// private void setUIFocus(bool focus){

	// }

	public override void _ExitTree()
	{
		//FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= setUIFocus;
	}
}
