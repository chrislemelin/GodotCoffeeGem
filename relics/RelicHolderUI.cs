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

	public void setRelicList(List<RelicResource> relicResources)
	{
		this.relicResources = relicResources;
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
		foreach (RelicResource relicResource in relicResources)
		{
			RelicUI relicUI = (RelicUI)relicUIPackedScene.Instantiate();
			relicUI.showTitle = false;
			relicUI.showPrice = false;
			relicUI.setRelic(relicResource);
			relicResource.node = this;
			relicControlHolder.AddChild(relicUI);
		}
		
	}

	public void startUpRelics()
	{
		foreach (RelicResource relicResource in relicResources)
		{
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

	public void startNewTurn()
	{
		foreach (RelicResource relicResource in relicResources)
		{
			relicResource.newTurn();
			relicResource.incrementTurnCounter();
			foreach (EffectResource executablePassive in relicResource.getTurnStartEffects())
			{
				executablePassive.execute(this);
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
