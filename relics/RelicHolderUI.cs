using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RelicHolderUI : Control
{
	[Export] PackedScene relicUIPackedScene;
	[Export] Control relicControlHolder;
	List<RelicResource> relicResources = new List<RelicResource>();
	List<RelicUI> relicUIs = new List<RelicUI>();


	public void setRelicList(List<RelicResource> relicResources)
	{
		this.relicResources = relicResources;
		deleteRelics();
		setUpRelics();

	}

	public void addRelic(RelicResource relicResource)
	{
		relicResources.Add(relicResource);
		deleteRelics();
		setUpRelics();
	}

	public void removeRelic(RelicResource relicResource)
	{
		relicResources.Remove(relicResource);
		deleteRelics();
		setUpRelics();
	}

	public List<RelicResource> getRelics()
	{
		return relicResources;
	}

	private void deleteRelics()
	{
		foreach (Node node in relicControlHolder.GetChildren())
		{
			node.QueueFree();
		}
	}

	private void setUpRelics()
	{
		relicUIs.Clear();
		foreach (RelicResource relicResource in relicResources)
		{
			RelicUI relicUI = (RelicUI)relicUIPackedScene.Instantiate();
			relicUI.showTitle = false;
			relicUI.showPrice = false;
			relicUIs.Add(relicUI);
			relicUI.setRelic(relicResource);
			relicResource.node = relicUI;
			relicControlHolder.AddChild(relicUI);
		}
	}

	public void startUpRelics()
	{
		foreach (RelicUI relicUI in relicUIs)
		{
			foreach (EffectResource executablePassive in relicUI.relicResource.getGameStartExePassives())
			{
				executablePassive.execute(relicUI);
				relicUI.activateAnimation();
			}
			relicUI.relicResource.startLevel();

			startNewTurn();
		}
	}


	public void startNewTurn()
	{
		foreach (RelicUI relicUI in relicUIs)
		{
			relicUI.relicResource.newTurn();
			relicUI.relicResource.incrementTurnCounter();
			List<EffectResource> executablePassives = relicUI.relicResource.getTurnStartEffects();
			foreach (EffectResource executablePassive in executablePassives)
			{
				executablePassive.execute(relicUI);
			}
			if (executablePassives.Count != 0)
			{
				relicUI.activateAnimation();
			}
		}
	}

	public void beforeTurnCleanUp()
	{
		foreach (RelicResource relicResource in relicResources)
		{
			relicResource.beforeTurnCleanUp();
		}
	}

	public void afterTurnCleanUp()
	{
		foreach (RelicResource relicResource in relicResources)
		{
			relicResource.afterTurnCleanUp();
		}
		List<RelicResource> relicsToRemove = relicResources.Where(relicResources => relicResources.lastForOneTurn).ToList();
		foreach (RelicResource relicResourceToRemove in relicsToRemove)
		{
			removeRelic(relicResourceToRemove);
		}
	}

	public void levelOver()
	{
		foreach (RelicResource relicResource in relicResources)
		{
			relicResource.levelOver();
		}
	}

	public void cardDrawn(CardResource cardResource)
	{
		foreach (RelicResource relicResource in relicResources)
		{
			relicResource.cardDrawn(cardResource);
		}
	}

	public void multChanged(float newMult)
	{
		foreach (RelicResource relicResource in relicResources)
		{
			relicResource.multChanged(newMult);
		}
	}

	public void ingredientsMatched(Match match)
	{
		foreach (RelicResource relicResource in relicResources)
		{
			relicResource.ingredientsMatched(match);
		}
	}

	public void ingredientDestroyed(Gem gem)
	{
		foreach (RelicResource relicResource in relicResources)
		{
			relicResource.ingredientDestroyed(gem);
		}
	}

	public override void _Ready()
	{
		base._Ready();
		NewTurnButton newTurnButton = FindObjectHelper.getNewTurnButton(this);
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(this);

		newTurnButton.StartNewTurn += () => startNewTurn();
		newTurnButton.BeforeTurnCleanUp += () => beforeTurnCleanUp();
		newTurnButton.AfterTurnCleanUp += () => afterTurnCleanUp();

		matchBoard.ingredientDestroyed += (gem) => ingredientDestroyed(gem);
		matchBoard.ingredientMatched += (match) => ingredientsMatched(match);
		((GameManager)FindObjectHelper.getGameManager(this)).levelOver += () => levelOver();
		FindObjectHelper.getHand(this).cardDrawn += (card) => cardDrawn(card);
		FindObjectHelper.getScore(this).multChange += (mult) => multChanged(mult);
	}


}
