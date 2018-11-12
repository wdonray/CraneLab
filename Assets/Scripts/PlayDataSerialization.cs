using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class PlayDataSerialization
{
    public static void SerializePlaytimeData(PlayTimeData ptd)
    {
        FileStream fs = new FileStream("dbmm.dat", FileMode.OpenOrCreate);
        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(fs, ptd);
    }

    private static void DeserializePlaytimeData(out PlayTimeData ptd)
    {
        FileStream fs = new FileStream("dbmm.dat", FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();

        ptd = (PlayTimeData)formatter.Deserialize(fs);
    }


    public class PlayTimeData
    {
        public string date;

        public Dictionary<string, bool> scenarioPassFail = new Dictionary<string, bool>();

        public string keyID;
    }
}
