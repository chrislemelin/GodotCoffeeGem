using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class RelicList : Resource
{
	[Export] bool removeDuplicates = true;

	[Export] public Array<RelicResource> allRelics = new Array<RelicResource>();

	[Export] public Array<ActivatableRelicResource> allActivatableRelics = new Array<ActivatableRelicResource>();


	public RelicList () {
	
	}
	public List<RelicResource> getRelics() {
		List<RelicResource> returnCards = allRelics.ToList().Where(card => card != null).ToList();
		if (removeDuplicates) {
			List<RelicResource> dups = returnCards.GroupBy(x => x.title)
			.Where(g => g.Count() > 1)
			.Select(y => y.First())
			.ToList();
			List<RelicResource> returnCardsNoDups = returnCards.GroupBy(x => x.title).Select(
				y => {
					return y.First();
			}).ToList();
			if (dups.Count > 0) {
				GD.Print("found dups: ");
				foreach(RelicResource cardResource in dups) {
					GD.Print(cardResource.title);
				}
			}
			returnCards = returnCardsNoDups;
		}
		return returnCards;
	}

	public List<ActivatableRelicResource> getActivatableRelics() {
		List<ActivatableRelicResource> returnRelics = allActivatableRelics.ToList().Where(card => card != null).ToList();
		if (removeDuplicates) {
			List<ActivatableRelicResource> dups = returnRelics.GroupBy(x => x.title)
			.Where(g => g.Count() > 1)
			.Select(y => y.First())
			.ToList();
			List<ActivatableRelicResource> returnRelicsNoDups = returnRelics.GroupBy(x => x.title).Select(
				y => {
					return y.First();
			}).ToList();
			if (dups.Count > 0) {
				GD.Print("found dups: ");
				foreach(ActivatableRelicResource cardResource in dups) {
					GD.Print(cardResource.title);
				}
			}
			returnRelics = returnRelicsNoDups;
		}
		return returnRelics;
	}
}
