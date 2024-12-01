using Godot;
using System;
using System.Xml.Serialization;

public partial class FindObjectHelper
{

	private static readonly String NEW_BUTTON_NAME = "NewTurnButton";
	private static readonly String SCORE_NAME = "%Score";
	private static readonly String MATCH_BOARD_NAME = "MatchBoard";
	private static readonly String DECK_VIEW_NAME = "DeckView";
	private static readonly String UPGRADE_CARD_UI = "UpgradeCardUI";
	private static readonly String CARD_SELECTION_NAME = "CardSelectionUI";
	private static readonly String RELIC_SELECTION_NAME = "RelicSelection";
	private static readonly String FORM_SUBMITTER_NAME = "FormSubmitter";
	private static readonly String SETTINGS_MENU_NAME = "SettingsMenu";
	private static readonly String RELIC_HOLDER_UI_NAME = "RelicHolder";
	private static readonly String CARD_LIBRARY_NAME = "CardLibrary";
	private static readonly String CONTROLLER_HELPER_NAME = "ControllerHelper";
	private static readonly String CAMERA_NAME = "Camera";
	private static readonly String HAND_LINE_NAME = "HandLine";
	private static readonly String CHARACTER_PORTRAIT_NAME = "CharacterPortrait";


	public static HandLine getHandLine(Node node)
	{
		return (HandLine)node.GetTree().GetFirstNodeInGroup(HAND_LINE_NAME);
	}

	public static CharacterPortrait GetCharacterPortrait(Node node)
	{
		return (CharacterPortrait)node.GetTree().GetFirstNodeInGroup(CHARACTER_PORTRAIT_NAME);
	}


	public static ControllerHelper getControllerHelper(Node node)
	{
		return (ControllerHelper)node.GetTree().GetFirstNodeInGroup(CONTROLLER_HELPER_NAME);
	}

	public static UpgradeCard getUpgradeCardUI(Node node)
	{
		return (UpgradeCard)node.GetTree().GetFirstNodeInGroup(UPGRADE_CARD_UI);
	}

	public static ActivatableRelicUI getActivatableRelicUI(Node node)
	{
		return (ActivatableRelicUI)node.GetTree().GetFirstNodeInGroup("ActivatableRelicUI");
	}

	public static Hand getHand(Node node)
	{
		return (Hand)node.GetTree().GetFirstNodeInGroup("Hand");
	}

	public static GameCamera getCamera(Node node)
	{
		return (GameCamera)node.GetTree().GetFirstNodeInGroup(CAMERA_NAME);
	}
	public static Mana getMana(Node node)
	{
		return (Mana)node.GetTree().GetFirstNodeInGroup("Mana");
	}
	public static CardLibrary getCardLibrary(Node node)
	{
		return (CardLibrary)node.GetTree().GetFirstNodeInGroup(CARD_LIBRARY_NAME);
	}
	public static FormSubmitter getFormSubmitter(Node node)
	{
		return (FormSubmitter)node.GetTree().GetFirstNodeInGroup(FORM_SUBMITTER_NAME);
	}

	public static NewTurnButton getNewTurnButton(Node node)
	{
		return (NewTurnButton)node.GetTree().GetFirstNodeInGroup(NEW_BUTTON_NAME);
	}
	public static Score getScore(Node node)
	{
		return (Score)node.GetTree().GetFirstNodeInGroup("Score");
	}
	public static MatchBoard getMatchBoard(Node node)
	{
		Godot.Collections.Array<Node> list = node.GetTree().GetNodesInGroup(MATCH_BOARD_NAME);
		if (list.Count > 0)
		{
			return (MatchBoard)list[0];
		}
		else
		{
			return null;
		}
	}

	public static DeckViewUI getDeckView(Node node)
	{
		return (DeckViewUI)node.GetTree().GetFirstNodeInGroup(DECK_VIEW_NAME);
	}

	public static RelicHolderUI getRelicHolderUI(Node node)
	{
		return (RelicHolderUI)node.GetTree().GetNodesInGroup(RELIC_HOLDER_UI_NAME)[0];
	}


	public static SettingsMenu getSettingsMenu(Node node)
	{
		return (SettingsMenu)node.GetTree().GetNodesInGroup(SETTINGS_MENU_NAME)[0];
	}

	public static AudioPlayer GetMusicPlayer(Node node)
	{
		return (AudioPlayer)node.GetTree().GetNodesInGroup("MusicPlayer")[0];
	}

	public static NewCardSelection getCardSelection(Node node)
	{
		return (NewCardSelection)node.GetTree().GetNodesInGroup(CARD_SELECTION_NAME)[0];
	}

	public static RelicSelection getRelicSelection(Node node)
	{
		return (RelicSelection)node.GetTree().GetNodesInGroup(RELIC_SELECTION_NAME)[0];
	}

	public static GameManagerIF getGameManager(Node node)
	{
		return (GameManagerIF)node.GetTree().CurrentScene;
	}

}
