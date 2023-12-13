using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RelicHolderUI : Control
{
	[Export] PackedScene relicUIPackedScene;
	[Export] Control relicControlHolder;
	List<RelicResource> relicResources;

	public void setRelicList(List<RelicResource> relicResources) {
		this.relicResources = relicResources;
		deleteRelics();
		setUpRelics();
	}

	private void deleteRelics() {
		foreach (Node node in relicControlHolder.GetChildren()) {
			node.QueueFree();
		}
	}

	private void setUpRelics() {
		foreach(RelicResource relicResource in relicResources) {
			RelicUI relicUI = (RelicUI)relicUIPackedScene.Instantiate();
			relicUI.setRelic(relicResource);
			relicControlHolder.AddChild(relicUI);
		}
	}

	public void startUpRelics() {
		foreach(RelicResource relicResource in relicResources) {
			relicResource.resetCounter();
			foreach (EffectResource executablePassive in relicResource.getGameStartExePassives())
			{
				executablePassive.execute(this);
			}
			foreach (EffectResource executablePassive in relicResource.getTurnStartEffects())
			{
				executablePassive.execute(this);
			}
		}
	}

	public void startNewTurn() {
		foreach(RelicResource relicResource in relicResources) {
			relicResource.incrementTurnCounter();
			foreach (EffectResource executablePassive in relicResource.getTurnStartEffects())
			{
				executablePassive.execute(this);
			}
		}
	}

	public override void _Ready()
	{
		base._Ready();
		FindObjectHelper.getNewTurnButton(this).StartNewTurn += () => startNewTurn();
	}
}
