using Godot;
using System;
using System.Xml.Serialization;

public partial class FindObjectHelper
{

    public static readonly String NEW_BUTTON_NAME = "%NewTurnButton";
    public static readonly String SCORE_NAME = "%Score";
    public static readonly String MATCH_BOARD_NAME = "%MatchBoard";


    public static Hand getHand(Node node)
    {
        return (Hand)node.GetTree().GetFirstNodeInGroup("Hand");
    }
    public static Mana getMana(Node node)
    {
        return (Mana)node.GetTree().GetFirstNodeInGroup("Mana");
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


    public static GameManagerIF getGameManager(Node node)
    {
        return (GameManagerIF)node.GetTree().CurrentScene;
    }

}
