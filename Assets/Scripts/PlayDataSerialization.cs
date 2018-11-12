using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class PlayDataSerialization
{
    public static string SerializePlaytimeData(List<PlayTimeData> ptd)
    {
        string defaultPath = UnityEngine.Application.dataPath + "/dbmm.dat";
        string backupPath = UnityEngine.Application.dataPath + "/dbbu.dat";

        FileStream fs1 = new FileStream(defaultPath, FileMode.OpenOrCreate);
        FileStream fs2 = new FileStream(backupPath, FileMode.OpenOrCreate);
        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(fs1, ptd);
        formatter.Serialize(fs2, ptd);

        return System.IO.File.GetLastWriteTimeUtc(defaultPath).ToString() + "\n" +
        System.IO.File.GetLastWriteTimeUtc(backupPath).ToString();
    }


    private static void DeserializePlaytimeData(out List<PlayTimeData> ptd)
    {
        FileStream fs = new FileStream("dbmm.dat", FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();

        ptd = (List<PlayTimeData>)formatter.Deserialize(fs);
    }


    public static IEnumerator GenerateRandomData(List<PlayTimeData> ptd)
    {
        for (int i = 0; i < 1000; i++)
        {
            PlayTimeData player = new PlayTimeData();
            player.date = "Today";
            player.keyID = UnityEngine.Random.Range(0, 1000000).ToString();

            ptd.Add(player);
            yield return null;
        }

        UnityEngine.Debug.Log(SerializePlaytimeData(ptd));

    }


    

    [System.Serializable]
    public class PlayTimeData
    {
        public string date;
        public Dictionary<string, bool> scenarioPassFail = new Dictionary<string, bool>();
        public string keyID;
    }

    [System.Serializable]
    public struct ScenarioData
    {
        public string date;
        public bool pass;
        public string keyID;
    }

    [System.Serializable]
    public struct PlayerHistor
    {
        public Dictionary<string, ScenarioData> playerHistory;
    }
}