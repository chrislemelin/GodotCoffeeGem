using Godot;
using System;
using System.Xml.Serialization;

public partial class FindObjectHelper
{

    public static readonly String NEW_BUTTON_NAME = "%NewTurnButton";
    public static readonly String MANA_NAME = "%Mana";
    public static readonly String HAND_NAME = "%Hand";
    public static readonly String SCORE_NAME = "%Score";
    public static readonly String MATCH_BOARD_NAME = "%MatchBoard";



    public static Hand getHand(Node node) {
        return node.GetNode<Hand>(FindObjectHelper.HAND_NAME);
    }

    public static Hand getBoard(Node node) {
        return node.GetNode<Hand>(FindObjectHelper.HAND_NAME);
    }

    public static NewTurnButton getNewTurnButton(Node node) {
        return node.GetNode<NewTurnButton>(FindObjectHelper.NEW_BUTTON_NAME);
    }

    public static Score getScore(Node node) {
        return node.GetNode<Score>(FindObjectHelper.SCORE_NAME);
    }

    public static MatchBoard getMatchBoard(Node node) {
        return node.GetNode<MatchBoard>(FindObjectHelper.MATCH_BOARD_NAME);
    }
    
    
    public static GameManagerIF getGameManager(Node node) {
        return (GameManagerIF)node.GetTree().CurrentScene;
    }

}
