using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public static class PlayDataSerialization
{
    public static string binaryData = UnityEngine.Application.dataPath  + "/databaseBinary.dat";
    public static string csvFile = UnityEngine.Application.dataPath     + "/userInfo.csv";

    // string is the players' name/ email/ unique ID
    public static Dictionary<string, PlayerHistory> playerHistory;


    public static void AddToPlayHistory(string playerName, string scenario, string date, string time, string pass, string keyID)
    {
        if (playerHistory == null) playerHistory = new Dictionary<string, PlayerHistory>();

        if (!playerHistory.ContainsKey(scenario)) playerHistory.Add(scenario, new PlayerHistory());

        playerHistory[playerName].AddScenarioToPlayHistory(scenario, date, time, pass, keyID);
    }


    // ..............................................................................................................................................
    public static string SerializePlaytimeData(Dictionary<string, PlayerHistory> playData)
    {
        FileStream fstream = new FileStream(binaryData, FileMode.OpenOrCreate);
        BinaryFormatter formatter = new BinaryFormatter();
        
        formatter.Serialize(fstream, playData);

        return System.IO.File.GetLastWriteTimeUtc(binaryData).ToString();
    }



    // ..............................................................................................................................................
    private static void DeserializePlaytimeData()
    {
        FileStream fstream = new FileStream(binaryData, FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();

        playerHistory = (Dictionary <string, PlayerHistory>) formatter.Deserialize(fstream);
    }


    public static void GenerateRandomUser()
    {
        AddToPlayHistory(
            "TestDummy:" + UnityEngine.Time.time.ToString(),
            "HelloWorld",
            System.DateTime.Today.ToString(), 
            System.DateTime.Now.ToString(), 
            "pass",
            "12345678");
    }


    // ..............................................................................................................................................
    [System.Serializable]
    public struct PlayerHistory 
    {
        // string is the scenario name
        public Dictionary<string, PlaySessions> sessionHistory;

        public void AddScenarioToPlayHistory(string scenario, string date, string time, string pass, string keyID)
        {
            SessionData sd = new SessionData(time, pass, keyID);

            if (sessionHistory == null) sessionHistory = new Dictionary<string, PlaySessions>();

            if (!sessionHistory.ContainsKey(scenario)) sessionHistory.Add(scenario, new PlaySessions());

            sessionHistory[scenario].AddSessionToHistory(date, sd);
        }
    }


    // ..............................................................................................................................................
    [System.Serializable]
    public struct PlaySessions
    {
        // string is the date of the session
        public Dictionary<string, List<SessionData>> sessionInfo;

        public void AddSessionToHistory(string date, SessionData session)
        {
            if (sessionInfo == null) sessionInfo = new Dictionary<string, List<SessionData>>();

            if (!sessionInfo.ContainsKey(date)) sessionInfo.Add(date, new List<SessionData>());

            sessionInfo[date].Add(session);
        }
    }


    // ..............................................................................................................................................
    [System.Serializable]
    public struct SessionData 
    {
        public string time;
        public string pass;
        public string keyID;

        public SessionData(string t, string p, string k)
        {
            time = t;
            pass = p;
            keyID = k;
        }
    }
}