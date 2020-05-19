using UnityEditor;
using UnityEngine;

namespace Jichaels.VRSDK.Editor
{
    public class SetupProject : MonoBehaviour
    {
        [MenuItem("Tools/Setup Project (Jichael's VR SDK)", false, 50)]
        public static void Setup()
        {

            if (LayerMask.NameToLayer("PostProcessing") == -1)
            {
                CreateLayer("PostProcessing");
            }

            if (LayerMask.NameToLayer("JVRInteract") == -1)
            {
                CreateLayer("JVRInteract");
            }


            Debug.LogWarning(
                "You now need to set the correct layer mask to certain assets. Open README.txt for all the information.");

        }

        private static int maxLayers = 31;

        private static bool CreateLayer(string layerName)
        {
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            if (PropertyExists(layersProp, 0, maxLayers, layerName)) return false;

            SerializedProperty sp;
            // Start at layer 9th index => first 8 reserved for unity
            for (int i = 8, j = maxLayers; i < j; i++)
            {
                sp = layersProp.GetArrayElementAtIndex(i);
                if (sp.stringValue != "") continue;
                sp.stringValue = layerName;
                Debug.Log($"Layer '{layerName}' has been added.");
                tagManager.ApplyModifiedProperties();
                return true;
            }

            return false;
        }

        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}