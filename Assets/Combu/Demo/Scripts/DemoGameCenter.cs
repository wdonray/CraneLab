using UnityEngine;
using System.Collections;
using Combu;

public class DemoGameCenter : MonoBehaviour {

	IEnumerator Start ()
	{
		while (!CombuManager.isInitialized)
			yield return null;

		Social.localUser.Authenticate((bool success) => {
			if (success)
			{
				CombuManager.localUser.AuthenticatePlatform("GameCenter", Social.localUser.id, (bool loginSuccess, string loginError) => {

					if (loginSuccess)
					{
						// Store the GameCenter name
						CombuManager.localUser.customData["GameCenterName"] = Social.localUser.userName;
						CombuManager.localUser.Update((bool updateSuccess, string updateError) => {

							if (success) {
								Debug.LogWarning("User updated: [" + CombuManager.localUser.id + "] " + CombuManager.localUser.customData["GameCenterName"]);
								var leaderboard = CombuManager.platform.CreateLeaderboard();
								leaderboard.SetUserFilter(new string[]{CombuManager.localUser.id});
								leaderboard.timeScope = UnityEngine.SocialPlatforms.TimeScope.AllTime;
								leaderboard.LoadScores((bool scoresSuccess) => {
									if (success)
										Debug.Log("Score: " + leaderboard.localUserScore.formattedValue);
								});
							}
							else
								Debug.LogError("User update failed: " + updateError);
						});
					}
					else
					{
						Debug.LogError("Login failed: " + loginError);
					}

				});
			}
			else
			{
				Debug.LogError("GameCenter login failed");
			}
		});
	}
}
