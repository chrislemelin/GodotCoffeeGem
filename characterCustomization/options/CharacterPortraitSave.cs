using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterPortraitSave : Node2D
{
	public const String LOAD_STRING = "/root/CharacterPortrait/";
	protected const string SAVE_FILE_NAME = "user://CharacterPortrait.save";


	protected Dictionary<CharacterLayerType, String> characterLayerToType = new Dictionary<CharacterLayerType, string>();

	public void setCharacterLayer(CharacterLayerType type, CharacterPortraitResource resource) {
		characterLayerToType[type] = resource.name;
		save();
	}
	public override void _Ready() {
		initDictionary();
		if (!FileAccess.FileExists(SAVE_FILE_NAME))
		{
			save();
		}
		else
		{
			load();
		}	
	}

	private void initDictionary() {
		characterLayerToType[CharacterLayerType.Background] = "Green";
		characterLayerToType[CharacterLayerType.Base] = "Nick";
		characterLayerToType[CharacterLayerType.Clothes] = "Default";
		characterLayerToType[CharacterLayerType.Hat] = "None";
		characterLayerToType[CharacterLayerType.Misc] = "None";
	}

	public void save() {
		using var saveFile = FileAccess.Open(SAVE_FILE_NAME, FileAccess.ModeFlags.Write);
		Godot.Collections.Dictionary<String, String> saveDict = new Godot.Collections.Dictionary<String, String>();
		foreach(KeyValuePair<CharacterLayerType, String> entry in characterLayerToType)
		{
			saveDict[entry.Key.ToString()] = entry.Value.ToString();
		}

		var jsonString = Json.Stringify(saveDict);
		saveFile.StoreLine(jsonString);
	}

	private void load() {
		using var saveGame = FileAccess.Open(SAVE_FILE_NAME, FileAccess.ModeFlags.Read);
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
			CharacterLayerType layerType = (CharacterLayerType) Enum.Parse(typeof(CharacterLayerType), entry.Key, true);
			characterLayerToType[layerType] = entry.Value.ToString();
		}
	}

}
