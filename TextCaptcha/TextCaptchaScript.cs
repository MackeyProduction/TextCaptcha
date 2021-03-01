using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w3bot.Api;
using w3bot.Input;
using w3bot.Script;
using w3bot.Util;

namespace TextCaptcha
{
	public class TextCaptchaScript : AbstractScript
	{
		[ScriptManifest("Captcha Example", "Captcha", "This script uses captchas.", "NoChoice", 1.0)]
		public class CaptchaExample : AbstractScript
		{
			private Captcha _captcha;
			private string _response;
			private CaptchaResult _captchaResult;

			public override void OnStart()
			{
				Status.Log("Test script has been started.");

				CreateBrowserWindow(); // create a new browser window

				_captcha = Methods.Captcha; // initialize captcha object

				Browser.Navigate("https://files.w3bot.org/text-captcha/"); // navigating to website
			}

			public string FetchResponse()
			{
				Task.Run(() =>
				{
					// fetch text of the first element
					var jsData = Browser.ExecuteJavascript("(function() { var result=document.getElementsByTagName('body')[0].innerText; return result; })()");

					if (jsData.Result != null)
					{
						// cast the response to a JavascriptResponse object
						var jsResponse = (JavascriptResponse)jsData.Result;

						_response = (string)jsResponse.Result;
					}
				});

				return _response;
			}

			public CaptchaResult FetchCaptchaResult(string jsBody)
			{
				Task.Run(() =>
				{
					_captchaResult = _captcha.SolveTextCaptcha(jsBody).Result;
				});

				return _captchaResult;
			}

			public override int OnUpdate()
			{
				// check if browser is ready
				if (Browser.IsReady)
				{
                    // fetch body
                    var jsBody = FetchResponse();
					if (jsBody == "undefined")
                    {
                        Status.Log("No text captcha found. Stopping script...");
                        return -1;
                    }

					Status.Log(jsBody);
					if (jsBody != "")
                    {
						// solving normal captcha
						var captchaResult = FetchCaptchaResult(jsBody);
						Sleep(5000, 10000);
						if (captchaResult.Success)
						{
							Status.Log(String.Format("The answer is: {0}.", captchaResult.Response));
							return -1;
						}
					}
				}

				return 100;
			}

			public override void OnFinish()
			{
				Status.Log("Thank you for using my script.");
			}
		}
	}
}
