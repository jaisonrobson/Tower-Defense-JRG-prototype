using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.EnterpriseServices;
using System.Collections.Generic;
using Core.General;
using static UnityEngine.LightProbeProxyVolume;
using static UnityEditor.Rendering.InspectorCurveEditor;

[Searchable]
public class DataManagerEditorWindow : OdinMenuEditorWindow
{
    public static string DEFAULTSOASSETSPATH = "Assets/Tower Defense/Resources/SO's";

    public string backupSOAssetsPath = "";

    private static Type[] typesToDisplay = TypeCache.GetTypesWithAttribute<ManageableDataAttribute>().OrderBy(m => m.Name).ToArray();

    private Type selectedType = typesToDisplay[0];

    protected override void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayoutUtility.GetRect(0, 15);

        GUILayout.EndVertical();

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.FlexibleSpace();

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("     Backup Directory     ")))
            {
                string dest = EditorUtility.OpenFolderPanel("Select a new Backup Directory", "", "");

                if (!string.IsNullOrEmpty(dest))
                {
                    backupSOAssetsPath = dest;
                }
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("     Backup     ")))
            {
                if (!Directory.Exists(backupSOAssetsPath) || string.IsNullOrEmpty(backupSOAssetsPath))
                {
                    EditorUtility.DisplayDialog("Invalid Path", "Invalid backup directory path", "ok");

                    return;
                }

                string destinationPath = backupSOAssetsPath + "/SO's Backup/"+ DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + "/";
                string destBackupPath = backupSOAssetsPath + "/SO's Backup/";

                if (!Directory.Exists(destBackupPath))
                {
                    Directory.CreateDirectory(destBackupPath);
                    AssetDatabase.Refresh();
                }

                FileUtil.CopyFileOrDirectory(DEFAULTSOASSETSPATH, destinationPath);

                EditorUtility.RevealInFinder(destinationPath);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("     Delete     ")))
            {
                BaseOptionDataSO boda = (BaseOptionDataSO) this.MenuTree.Selection.SelectedValue;

                string assetPath = AssetDatabase.GetAssetPath(boda);

                if (EditorUtility.DisplayDialog("Delete Asset", "Do you really want to delete "+boda.name+ " at path:\n\n" + assetPath + " ?", "Yes", "No"))
                {
                    File.Delete(assetPath);
                    AssetDatabase.Refresh();
                }
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("     Create     ")))
            {
                ScriptableObjectCreator.ShowDialog(typesToDisplay, DataManagerEditorWindow.DEFAULTSOASSETSPATH, obj =>
                {
                    base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                });
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();

        GUILayout.BeginVertical();

        GUILayoutUtility.GetRect(0, 15);

        GUILayout.EndVertical();

        //Custom GUI drawing
        //Draws the SO Organization Buttons
        if (GUIUtils.SelectButtonList(ref selectedType, typesToDisplay))
            this.ForceMenuTreeRebuild(); //If a button is pressed force the Odin tree interface to rebuild

        //Default GUI drawing
        base.OnGUI();
    }

    [MenuItem("Tools/Data Manager")]
    private static void OpenEditor() => GetWindow<DataManagerEditorWindow>();

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Config.DrawSearchToolbar = true;

        tree.AddAllAssetsAtPath("", DataManagerEditorWindow.DEFAULTSOASSETSPATH + "/", selectedType, true, true);

        tree.SortMenuItemsByName(false);

        return tree;
    }

    protected override void OnBeginDrawEditors()
    {
        if (this.MenuTree == null)
            return;

        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}


////////////////////////////////////////////////////////////////////////////////
////////////SCRIPT MADE BY JAISON ROBSON GUSAVA UNDER MIT LICENSE///////////////
/////////////////////// https://github.com/jaisonrobson/ ///////////////////////