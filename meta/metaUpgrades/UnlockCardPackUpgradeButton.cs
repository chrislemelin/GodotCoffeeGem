using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UnlockCardPackUpgradeButton : MetaUpgradeButton
{
	//[Export] DeckViewUI deckViewUI;
	public override void _Ready() {
		base._Ready();
	}

	protected override void doUpgrade() {
		Optional<UnlockableCardPack> pack = getNewCardPack();
		gameManagerIF.unlockCardPack(pack.GetValue());
		FindObjectHelper.getDeckView(this).setUp(pack.GetValue().getCards(), null, TextHelper.centered("Unlocked Cards!"));
	}

	protected override int getUpgradeLevel() {
		return gameManagerIF.getUnlockedCardPacks().Count;
	}

	protected override int getUpgradeMaxLevel() {
		return gameManagerIF.getLockedCardPacks().Count + gameManagerIF.getUnlockedCardPacks().Count;
	}

	private Optional<UnlockableCardPack> getNewCardPack() {
		List<UnlockableCardPack> lockedPacks = gameManagerIF.getLockedCardPacks().ToList();
		if (lockedPacks.Count > 0) {
			UnlockableCardPack unlockedPack = lockedPacks[0];
			return Optional.Some<UnlockableCardPack>(unlockedPack);
		}
		return Optional.None<UnlockableCardPack>();
	}

}
