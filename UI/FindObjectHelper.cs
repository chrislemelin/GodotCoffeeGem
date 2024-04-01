using Godot;
using System;
using System.Xml.Serialization;

public partial class FindObjectHelper
{

	public static readonly String NEW_BUTTON_NAME = "%NewTurnButton";
	public static readonly String SCORE_NAME = "%Score";
	public static readonly String MATCH_BOARD_NAME = "%MatchBoard";
	public static readonly String DECK_VIEW_NAME = "DeckViewUI";
	public static readonly String CARD_SELECTION_NAME = "CardSelectionUI";
	public static readonly String FORM_SUBMITTER_NAME = "FormSubmitter";
	public static readonly String SETTINGS_MENU_NAME = "SettingsMenu";



	public static Hand getHand(Node node)
	{
		return (Hand)node.GetTree().GetFirstNodeInGroup("Hand");
	}
	public static Mana getMana(Node node)
	{
		return (Mana)node.GetTree().GetFirstNodeInGroup("Mana");
	}
	public static FormSubmitter getFormSubmitter(Node node)
	{
		return (FormSubmitter)node.GetTree().GetFirstNodeInGroup(FORM_SUBMITTER_NAME);
	}

	public static NewTurnButton getNewTurnButton(Node node)
	{
		return node.GetNode<NewTurnButton>(FindObjectHelper.NEW_BUTTON_NAME);
	}
	public static Score getScore(Node node)
	{
		return (Score)node.GetTree().GetFirstNodeInGroup("Score");
	}
	public static MatchBoard getMatchBoard(Node node)
	{
		return node.GetNode<MatchBoard>(FindObjectHelper.MATCH_BOARD_NAME);
	}

	public static DeckViewUI getDeckView(Node node)
	{
		return (DeckViewUI)node.GetTree().GetNodesInGroup(DECK_VIEW_NAME)[0];
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

	public static GameManagerIF getGameManager(Node node)
	{
		return (GameManagerIF)node.GetTree().CurrentScene;
	}

}
