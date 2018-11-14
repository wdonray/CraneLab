using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public static class PlayDataSerialization
{
    public static string binaryData = UnityEngine.Application.dataPath  + "/databaseBinary.dat";
    public static string XMLData = UnityEngine.Application.dataPath     + "/databaseXML.dat";

    // string is the players' name/ email/ unique ID
    public static Dictionary<string, PlayerHistory> playerHistory;






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





    // ..............................................................................................................................................
    [System.Serializable]
    public struct PlayerHistory 
    {
        // string is the scenario name
        public Dictionary<string, List<PlaySession>> playerHistory;

        public string AddScenarioToPlayHistory(string scenario, string date, string time, string pass, string keyID)
        {
            SessionData sd = new SessionData(time, pass, keyID);

            return scenario + " " +
                date + " " +
                time + " " +
                pass + " " +
                keyID;
        }
    }


    // ..............................................................................................................................................
    [System.Serializable]
    public struct PlaySession
    {
        // string is the date of the session
        public Dictionary<string, List<SessionData>> sessionInfo;

        public void AddSessionToHistory(string scenario, SessionData session)
        {
            if (sessionInfo == null) sessionInfo = new Dictionary<string, List<SessionData>>();

            if (!sessionInfo.ContainsKey(scenario)) sessionInfo.Add(scenario, new List<SessionData>());
            sessionInfo[scenario].Add(session);
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