using UnityEngine;
#if UNITY_EDITOR
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class ProgressBarWithFieldsAttributeDrawer : OdinAttributeDrawer<ProgressBarWithFieldsAttribute, float>
{
    private float internalValue = 0f;

    // (ODIN) Methods [START]
    protected override void DrawPropertyLayout(GUIContent label)
    {
        internalValue = ValueEntry.SmartValue;

        DrawProgressBarWithFields(label);

        ValueEntry.SmartValue = internalValue;

        ValueEntry.Property.MarkSerializationRootDirty();
    }
    // (ODIN) Methods [END]

    private void DrawProgressBarWithFields(GUIContent label)
    {
        GUILayout.Space(1f);

        SirenixEditorGUI.BeginHorizontalPropertyLayout(label);

        Rect progressBarRect = GUILayoutUtility.GetRect(50f, 20f, GUILayout.ExpandWidth(true));

        ProgressBarConfig pbc = ProgressBarConfig.Default;
        pbc.DrawValueLabel = true;
        pbc.ForegroundColor = new Color(Attribute.r, Attribute.g, Attribute.b, 1f);

        internalValue = (float) SirenixEditorFields.ProgressBarField(progressBarRect, internalValue, Attribute.min, Attribute.max, pbc);

        GUILayout.Space(5f);

        Rect fieldRect = GUILayoutUtility.GetRect(60f, 20f, GUILayout.ExpandWidth(false));

        internalValue = SirenixEditorFields.FloatField(fieldRect, internalValue);

        internalValue = Mathf.Clamp(internalValue, Attribute.min, Attribute.max);

        SirenixEditorGUI.EndHorizontalPropertyLayout();

        GUILayout.Space(1f);
    }

}

#endif

////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////