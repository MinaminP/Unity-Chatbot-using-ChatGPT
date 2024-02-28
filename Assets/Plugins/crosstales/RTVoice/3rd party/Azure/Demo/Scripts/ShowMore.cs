using UnityEngine;

namespace Crosstales.RTVoice.Azure
{
   /// <summary>Shows the details for Azure.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_azure_1_1_show_more.html")]
   public class ShowMore : MonoBehaviour
   {
      #region Public methods

      public void Show()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL("https://docs.microsoft.com/en-us/azure/cognitive-services/Speech-Service/");
      }

      #endregion
   }
}
// © 2020-2023 crosstales LLC (https://www.crosstales.com)