//using UnityEngine;

//public class ScoreManager : MonoBehaviour
//{
//    public AudioClip[] m_titleAudioClips;
//    public AudioSource m_audioSource;

//    public string m_fileName;

//    private void AppendToScoreFile()
//    {
//        string filePath = Application.dataPath + "/" + m_fileName;

//        string score = Date() + ", ";
//        score += System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute.ToString("00") + ", ";
//        string testAnswers = "";
//        float landScore = 0;

//        foreach (string s in FindObjectOfType<TestManager>().GetAnswers())
//        {
//            testAnswers += s + ", ";
//        }
//        testAnswers += FindObjectOfType<TestManager>().GetScore() + ", ";

//        foreach (_PLACEHOLDER_LAND_DEFORM p in FindObjectsOfType<_PLACEHOLDER_LAND_DEFORM>())
//        {
//            landScore += p.CalculateLandRemaining();
//        }
//        landScore /= FindObjectsOfType<_PLACEHOLDER_LAND_DEFORM>().Length;

//        score += testAnswers;
//        score += ((landScore) * 100f).ToString("00") + "%";

//        System.IO.StreamWriter file = System.IO.File.AppendText(filePath);
//        file.WriteLine(score);
//        file.Close();
//    }

//    private string Date()
//    {
//        return System.DateTime.Now.Year.ToString() + " " +
//            System.DateTime.Now.Month.ToString() + " " +
//            System.DateTime.Now.Day.ToString();
//    }
    
//    public void PrintScoreToText(UnityEngine.UI.Text text)
//    {
//        float landScore = 0;
//        foreach (_PLACEHOLDER_LAND_DEFORM p in FindObjectsOfType<_PLACEHOLDER_LAND_DEFORM>())
//        {
//            landScore += p.CalculateLandRemaining();
//        }
//        landScore /= FindObjectsOfType<_PLACEHOLDER_LAND_DEFORM>().Length;

//        string title = "";

//        if(landScore <= 0.50f)
//        {
//            title = "Steward of the Swamp";
//            m_audioSource.PlayOneShot(m_titleAudioClips[0]);
//        }
//        else if (landScore <= 0.60f)
//        {
//            title = "Warden of the Water";
//            m_audioSource.PlayOneShot(m_titleAudioClips[1]);

//        }
//        else if (landScore <= 0.70f)
//        {
//            title = "Defender of the Delta";
//            m_audioSource.PlayOneShot(m_titleAudioClips[2]);

//        }
//        else if (landScore <= 0.80f)
//        {
//            title = "Guardian of the Ground";
//            m_audioSource.PlayOneShot(m_titleAudioClips[3]);

//        }
//        else if (landScore <= 0.90f)
//        {
//            title = "Hero of the Habitat";
//            m_audioSource.PlayOneShot(m_titleAudioClips[4]);

//        }
//        else // Perfect score or better
//        {
//            title = "Champion of the Coast";
//            m_audioSource.PlayOneShot(m_titleAudioClips[5]);

//        }

//        text.text = "With a final score of " + "\n" +
//            "<b>" + (landScore * 10000).ToString("0") + "</b>" + "\n" +
//            "You have earned the title " + "\n" +
//            "<b>" + title + "!" + "</b>";

//        AppendToScoreFile();
//    }

//    private void OnDestroy()
//    {
//        print("dead");
//    }
//}
