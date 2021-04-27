using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class WizardMassPrefabricator : ScriptableWizard
{
    public GameObject replacementPrefab;
    public float scaleFactor = 1;
    
    [MenuItem("GameObject/Mass Prefabricator")]
    static void ReplaceObjectWizard()
    {
        ScriptableWizard.DisplayWizard<WizardMassPrefabricator>("Replace all selected objects with replacement prefab?", "Replace all " + Selection.transforms.Length + " selected objects", "Close");
    }

    void OnWizardCreate()
    {
        ReplaceSelection(replacementPrefab);
    }

    void OnWizardOtherButton()
    {
        Close();
    }

    void OnWizardUpdate()
    {
        if (replacementPrefab == null)
        {
            helpString = "Please set the replacement prefab!";
            isValid = false;
        }
        else if (Selection.transforms.Length <= 0)
        {
            helpString = "Please select something!";
            isValid = false;
        }
        else
        {
            helpString = "Are you sure you want to replace " + Selection.transforms.Length + " objects in the scene with a " + ObjectNames.NicifyVariableName(replacementPrefab.name) + "?";

            isValid = true;
        }

        createButtonName = "Replace all " + Selection.transforms.Length + " selected objects";
    }

    private void ReplaceSelection(GameObject replacement)
    {
        foreach (Transform selectedTransform in Selection.transforms)
        {
            if (selectedTransform != null)
            {
                GameObject replacementObject = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab);

                if (replacementObject != null)
                {
                    Undo.RegisterCreatedObjectUndo(replacementObject, "Created replacement game object");

                    replacementObject.transform.parent = selectedTransform.parent;
                    replacementObject.transform.SetSiblingIndex(selectedTransform.GetSiblingIndex());

                    replacementObject.transform.position = selectedTransform.position;
                    replacementObject.transform.rotation = selectedTransform.rotation;
                    replacementObject.transform.localScale = selectedTransform.localScale * scaleFactor;

                    replacementObject.SetActive(selectedTransform.gameObject.activeInHierarchy);

                    Undo.DestroyObjectImmediate(selectedTransform.gameObject);
                }
            }
        }
    }
}

#endif
