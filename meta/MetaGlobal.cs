using Godot;
using System;
using System.Collections.Generic;

public partial class MetaGlobal : Node
{
	public const String LOAD_STRING = "/root/MetaGlobal/";

	private Godot.Collections.Dictionary<GemType, int> gemTypeToUpgradeLevel = new();
	public int gemUpgradeMax = 5;
	private const String COIN_DROP_RATE_STRING = "coinDropRate";
	public int coinDropRate = 0;
	public int coinDropRateMax = 5;

	private const String META_COIN_DROP_RATE_STRING = "metaCoinDropRate";
	public int metaCoinDropRate = 0;
	public int metaCoinDropRateMax = 5;

	private const String CARDS_UNLOCKED_STRING = "cardPacksUnlocked";
	public HashSet<String> cardPacksUnlocked = new HashSet<String>();

	private string saveFileName = "user://metaData.save";

	public override void _Ready() {
		initDictionary();
		if (!FileAccess.FileExists(saveFileName))
		{
			save();
		}
		else
		{
			load();
		}	
	}
	private void load() {
		using var saveGame = FileAccess.Open(saveFileName, FileAccess.ModeFlags.Read);
		var jsonString = saveGame.GetLine();
		// Creates the helper class to interact with JSON
		var json = new Json();
		var parseResult = json.Parse(jsonString);
		if (parseResult != Error.Ok)
		{
			GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
			return;
		}
		Godot.Collections.Dictionary<String, String> nodeData = new Godot.Collections.Dictionary<String, String>((Godot.Collections.Dictionary)json.Data);
		foreach(KeyValuePair<String, String> entry in nodeData)
		{
			if (GemTypeHelper.hasEnumValue(entry.Key)) {
				gemTypeToUpgradeLevel[GemTypeHelper.fromString(entry.Key)] = Int32.Parse(entry.Value);
			}
		}
		coinDropRate = Int32.Parse(nodeData[COIN_DROP_RATE_STRING]);
		metaCoinDropRate = Int32.Parse(nodeData[META_COIN_DROP_RATE_STRING]);
		cardPacksUnlocked = new HashSet<string>(nodeData[CARDS_UNLOCKED_STRING].Split(','));

	}

	private void save() {
		using var metaSave = FileAccess.Open(saveFileName, FileAccess.ModeFlags.Write);
		Godot.Collections.Dictionary<String, String> metaDict = new Godot.Collections.Dictionary<String, String>();
		foreach(KeyValuePair<GemType, int> entry in gemTypeToUpgradeLevel)
		{
			metaDict[entry.Key.ToString()] = entry.Value.ToString();
		}
		metaDict[COIN_DROP_RATE_STRING] = coinDropRate.ToString();
		metaDict[META_COIN_DROP_RATE_STRING] = metaCoinDropRate.ToString();
		metaDict[CARDS_UNLOCKED_STRING] = string.Join(",", cardPacksUnlocked);

		var jsonString = Json.Stringify(metaDict);
		metaSave.StoreLine(jsonString);
	}

	public void upgradeGemType(GemType gemType) {
		if (gemTypeToUpgradeLevel.ContainsKey(gemType)) {
			gemTypeToUpgradeLevel[gemType] = Math.Min(gemTypeToUpgradeLevel[gemType] + 1, gemUpgradeMax);
			save();
		} else {
			GD.PrintErr("tried to upgrade gem type " + gemType.ToString() + " which doenst exist");
		}
	}

	public void addUnlockedCardPack(UnlockableCardPack unlockableCardPack) {
		cardPacksUnlocked.Add(unlockableCardPack.title);
		save();
	}

	public void addCoinDropRate(int value) {
		Mathf.Max(coinDropRate += value, coinDropRateMax);
		save();
	}

	public void addMetaCoinDropRate(int value) {
		Mathf.Max(metaCoinDropRate += value, metaCoinDropRateMax);
		save();
	}


	public int getGemUpgrade(GemType gemType) {
		if (gemTypeToUpgradeLevel.ContainsKey(gemType)) {
			return gemTypeToUpgradeLevel[gemType];
		} else {
			GD.PrintErr("tried to fetch gem type " + gemType.ToString() + " which doenst exist");
			return 0;
		}
	}

	public void reset() {
		initDictionary();
		coinDropRate = 0;
		metaCoinDropRate = 0;
		cardPacksUnlocked = new HashSet<string>();
		save();
	}



	private void initDictionary() {
		gemTypeToUpgradeLevel[GemType.Coffee] = 0;
		gemTypeToUpgradeLevel[GemType.Vanilla] = 0;
		gemTypeToUpgradeLevel[GemType.Milk] = 0;
		gemTypeToUpgradeLevel[GemType.Sugar] = 0;
		gemTypeToUpgradeLevel[GemType.Leaf] = 0;
	}

	public List<ColorUpgrade> getColorUpgrades () {
		List<ColorUpgrade> colorUpgrades = new List<ColorUpgrade>();
		foreach(KeyValuePair<GemType, int> entry in gemTypeToUpgradeLevel)
		{
			if (entry.Value != 0) {
				ColorUpgrade colorUpgrade = new ColorUpgrade();
				colorUpgrade.gemType = entry.Key;
				colorUpgrade.baseIncrease = entry.Value * 25;
				colorUpgrades.Add(colorUpgrade);
			}
		}
		return colorUpgrades;
	}
}
