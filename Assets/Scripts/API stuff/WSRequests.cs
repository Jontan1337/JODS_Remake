using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RequestType
{
    login,
    uploadScoreboard
}
public class WSRequests
{
    public string type;
}

public class LoginRequest : WSRequests
{
    public string email;
    public string password;
}

public class ScoreboardRequest : WSRequests
{
    public bool isMaster;
    public int highscore;
}

public class SurvivorScoreboardRequest : ScoreboardRequest
{
    public string classType;
    public int kills;
    public int deaths; //actually a Down
    public int specialsUsed;
}

public class MasterScoreboardRequest : ScoreboardRequest
{
    public int unitsPlaced;
    public int totalUnitUpgrades;
}