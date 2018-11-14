using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Combu;

public class CombuDemoWordpress : CombuDemoScene
{
	public Text textWordpressUserId;

	public override void UserLogin ()
	{
		if (string.IsNullOrEmpty(loginUsername.text) || string.IsNullOrEmpty(loginPassword.text))
		{
			loginError.text = "Enter your credentials";
			return;
		}
		loginError.text = "Loading...";

		var form = CombuManager.instance.CreateForm();
		form.AddField("Username", loginUsername.text);
		form.AddField("Password", loginPassword.text);

		CombuManager.instance.CallWebservice(CombuManager.instance.GetUrl("extra/login_wordpress.php"), form, (string text, string error) => {

			loginError.text = "";
			if (string.IsNullOrEmpty(error))
			{
				Hashtable result = text.hashtableFromJson();
				if (result != null)
				{
					bool success = false;
					if (result.ContainsKey("success"))
						bool.TryParse(result["success"].ToString(), out success);
					if (success)
					{
						User user = new User(true);
						user.FromJson(result["message"].ToString());
						CombuManager.instance.SetLocalUser(user);
						textWordpressUserId.text = user.platforms[0].platformId;
					}
					else if (!success && result.ContainsKey("message"))
					{
						error = result["message"].ToString();
					}
				}
			}

			if (!CombuManager.localUser.authenticated)
				loginError.text = error;
			else
				OpenPanel(panelMenu);

		});
	}
}
