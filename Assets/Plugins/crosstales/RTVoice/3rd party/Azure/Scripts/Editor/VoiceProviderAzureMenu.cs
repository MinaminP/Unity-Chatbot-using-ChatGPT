#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.RTVoice.Azure
{
   /// <summary>Editor component for for adding the prefabs from 'Azure' in the "Tools"-menu.</summary>
   public static class VoiceProviderAWSMenu
   {
      [MenuItem("Tools/" + Crosstales.RTVoice.Util.Constants.ASSET_NAME + "/Prefabs/3rd party/VoiceProviderAzure", false, Crosstales.RTVoice.EditorUtil.EditorHelper.MENU_ID + 320)]
      private static void AddVoiceProvider()
      {
         Crosstales.RTVoice.EditorUtil.EditorHelper.InstantiatePrefab("Azure", $"{Crosstales.RTVoice.EditorUtil.EditorConfig.ASSET_PATH}3rd party/Azure/Prefabs/");
      }

      [MenuItem("Tools/" + Crosstales.RTVoice.Util.Constants.ASSET_NAME + "/Prefabs/3rd party/VoiceProviderAzure", true)]
      private static bool AddVoiceProviderValidator()
      {
         return !Crosstales.RTVoice.Azure.VoiceProviderAzureEditor.isPrefabInScene;
      }
   }
}
#endif
// © 2019-2023 crosstales LLC (https://www.crosstales.com)