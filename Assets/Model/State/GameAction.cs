using System.Collections.Generic;

[System.Serializable]
public class GameAction
{

}

[System.Serializable]
public class GameActions
{
    public int playerID;
    public List<GameAction> actions;
}