using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class RecipeResource : Resource
{
	[Export]
	public Array<GemType> ingredientList = new Array<GemType>();
	public List<bool> completedComponents = new List<bool>();
	[Signal]
	public delegate void finishedRecipeEventHandler();
	[Signal]
	public delegate void statusChangedEventHandler();
	private ActivatableRelicResource activatableRelicResource;
	public void setUp(Node node, ActivatableRelicResource activatableRelicResource)
	{
		if (completedComponents.Count == 0) {
			reset();
		}
		this.activatableRelicResource = activatableRelicResource;
		MatchBoard matchBoard = FindObjectHelper.getMatchBoard(node);
		if (matchBoard != null)
		{
			matchBoard.ingredientAllMatches += evaulateMatches;
		}
	}

	public void evaulateMatches(Array<Match> matches)
	{
		if (activatableRelicResource.atMaxCapacity())
		{
			return;
		}
		bool changed = false;
		foreach (Match match in matches)
		{
			for (int count = 0; count < completedComponents.Count; count++)
			{
				if (!completedComponents[count] && ingredientList[count] == match.GetGemType())
				{
					completedComponents[count] = true;
					changed = true;
					break;
				}
			}
		}
		if (completed())
		{
			EmitSignal(SignalName.finishedRecipe);
			reset();
		}
		if (changed)
		{
			EmitSignal(SignalName.statusChanged);
		}
	}

	public List<Tuple<GemType, bool>> getCompletionStatus()
	{
		List<Tuple<GemType, bool>> returnList = new List<Tuple<GemType, bool>>();
		for (int count = 0; count < ingredientList.Count; count++)
		{
			returnList.Add(new Tuple<GemType, bool>(ingredientList[count], completedComponents[count]));
		}
		return returnList;
	}

	public void reset()
	{
		completedComponents.Clear();
		for (int count = 0; count < ingredientList.Count; count++)
		{
			completedComponents.Add(false);
		}
	}

	public bool completed()
	{
		return completedComponents.All(value => value);
	}

}
