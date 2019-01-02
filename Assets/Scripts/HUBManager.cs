using System;
using System.Collections;
using System.Collections.Generic;
using Combu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUBManager : MonoBehaviour
{
    public Button LogoutButton, StartButton;
    public TextMeshProUGUI NameText, TotalVRTime, DropText, PickUpText;
    private List<Score> allScores = new List<Score>();
    private bool allReady = true;
    // Use this for initialization
    void Start()
    {
        SetNameText(CombuManager.platform.localUser.userName);
        Combu.CombuManager.platform.LoadScoresByUser("total_time", Combu.CombuManager.localUser, Combu.eLeaderboardInterval.Total, 1,
            (Combu.Score score, int page, string error) =>
        {
            if (score != null)
            {
                TotalVRTime.transform.GetChild(0).GetComponent<Text>().text =
                    TimeSpan.FromSeconds(score.value).ToString("");
            }
            allScores.Add(score);
        });

        //TotalVRTime.transform.GetChild(0).GetComponent<Text>().text = TimeSpan.FromSeconds(Round(SceneLoader._instance.GetTimeInTest(), 0)).ToString("");

        Combu.CombuManager.platform.LoadScoresByUser("accuracy_dropoff", Combu.CombuManager.localUser, Combu.eLeaderboardInterval.Total, 1,
            (Combu.Score score, int page, string error) =>
            {
                if (score != null)
                {
                    print("<color=yellow> [Score Loaded] Drop Off: " + score.value + "</color>");
                    AccuracyScoreManager.Instance.SetDropOffScore(score.value > 0 ? score.value : 1f);
                    float acc = AccuracyScoreManager.Instance.GetDropOffGraded();
                    DropText.transform.GetChild(0).GetComponent<Text>().text = acc.ToString("") + "%";
                }
                allScores.Add(score);
            });

        Combu.CombuManager.platform.LoadScoresByUser("accuracy_pickup", Combu.CombuManager.localUser,
            Combu.eLeaderboardInterval.Total, 1,
            (Combu.Score score, int page, string error) =>
            {
                if (score != null)
                {
                    print("<color=yellow> [Score Loaded] Pick Up: " + score.value + "</color>");
                    AccuracyScoreManager.Instance.SetLoadUpScore(score.value > 0 ? score.value : 1f);
                    float acc = AccuracyScoreManager.Instance.GetLoadUpGraded();
                    PickUpText.transform.GetChild(0).GetComponent<Text>().text = acc.ToString("") + "%";
                }
                allScores.Add(score);
            });
    }

    private void Update()
    {
        if (LogoutButton.interactable == false)
        {
            foreach (var score in allScores)
            {
                if (score == null)
                {
                    allReady = false;
                }
            }

            if (allScores.Count == 3)
            {
                if (allReady)
                {
                    LogoutButton.interactable = true;
                    StartButton.interactable = true;
                }
            }
        }
    }

    public void SetNameText(string value)
    {
        NameText.transform.GetChild(0).GetComponent<Text>().text = value.ToUpper();
    }
}
