using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RequestType
{
    login,
    uploadScoreboard,
    sendMessage,
    disconnect,
    setActive,
    getStats
}
public class WSRequests
{
    public string type;
}

[System.Serializable]
public class LoginRequest : WSRequests
{
    public string username;
    public string email;
    public string password;
}

[System.Serializable]
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

public class WSResponse<T> : WSRequests
{
    public int code;
    public string message;
    public List<T> data;
}

public class DisconnectRequest : WSRequests
{
    public string reason;
}
[System.Serializable]
public class UserStats
{
    [System.Serializable]
    public class MasterStats
    {
        public int highscore;
        public int unitsPlaced;
        public int unitUpgrades;
    }
    public MasterStats masterStats;

    [System.Serializable]
    public class SurvivorStats
    {
        public int classType;
        public int highscore;
        public int kills;
        public int specialUsed;
        public int deaths;
    }
    public SurvivorStats[] classes;
}

public enum SurvivorType
{
    Soldier,
    Taekwondo,
    Engineer,
    Doctor
}