using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.Azure
{
   /// <summary>Set the access settings for Azure.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_azure_1_1_access_settings.html")]
   public class AccessSettings : MonoBehaviour
   {
      #region Variables

      public VoiceProviderAzure Provider;

      public GameObject SettingsPanel;

      public InputField Endpoint;
      public InputField Request;
      public InputField APIKey;

      public Button OkButton;

      private string enteredEndpoint = string.Empty;
      private string enteredRequest = string.Empty;
      private string enteredKey = string.Empty;

      private static string lastEndpoint;
      private static string lastRequest;
      private static string lastKey;

      private Color okColor;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         okColor = OkButton.image.color;

         if (!string.IsNullOrEmpty(lastEndpoint))
            Provider.Endpoint = lastEndpoint;

         if (!string.IsNullOrEmpty(lastRequest))
            Provider.RequestUri = lastRequest;

         if (!string.IsNullOrEmpty(lastKey))
            Provider.APIKey = lastKey;

         if (!string.IsNullOrEmpty(Provider.Endpoint))
            enteredEndpoint = lastEndpoint = Endpoint.text = Provider.Endpoint;

         if (!string.IsNullOrEmpty(Provider.RequestUri))
            enteredRequest = lastRequest = Request.text = Provider.RequestUri;

         if (!string.IsNullOrEmpty(Provider.APIKey))
            enteredKey = lastKey = APIKey.text = Provider.APIKey;

         if (string.IsNullOrEmpty(Provider.Endpoint) || string.IsNullOrEmpty(Provider.RequestUri) || string.IsNullOrEmpty(Provider.APIKey))
         {
            ShowSettings();
         }
         else
         {
            HideSettings();
         }

         SetOkButton();
      }

      #endregion


      #region Public methods

      public void OnEndpointEntered(string ep)
      {
         enteredEndpoint = ep ?? string.Empty;
         SetOkButton();
      }

      public void OnRequestEntered(string req)
      {
         enteredRequest = req ?? string.Empty;
         SetOkButton();
      }

      public void OnAPIKeyEntered(string key)
      {
         enteredKey = key ?? string.Empty;
         SetOkButton();
      }

      public void HideSettings()
      {
         SettingsPanel.SetActive(false);

         if ((!string.IsNullOrEmpty(enteredEndpoint) && (!enteredEndpoint.Equals(lastEndpoint)) || (!string.IsNullOrEmpty(enteredRequest) && !enteredRequest.Equals(lastRequest)) || (!string.IsNullOrEmpty(enteredKey) && !enteredKey.Equals(lastKey))))
         {
            lastEndpoint = Provider.Endpoint = enteredEndpoint;
            lastRequest = Provider.RequestUri = enteredRequest;
            lastKey = Provider.APIKey = enteredKey;
            Speaker.Instance.ReloadProvider();
         }
      }

      public void ShowSettings()
      {
         SettingsPanel.SetActive(Speaker.Instance.CustomProvider.isPlatformSupported);
      }

      public void SetOkButton()
      {
         if (enteredKey.Length >= 32 && enteredEndpoint.Contains("api.cognitive.microsoft.com") && enteredRequest.Contains("tts.speech.microsoft.com"))
         {
            OkButton.interactable = true;
            OkButton.image.color = okColor;
         }
         else
         {
            OkButton.interactable = false;
            OkButton.image.color = Color.gray;
         }
      }

      #endregion
   }
}
// © 2020-2023 crosstales LLC (https://www.crosstales.com)