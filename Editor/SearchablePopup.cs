using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class SearchablePopup : PopupWindowContent
{
    private static GUIStyle labelStyle = null;

    private string[] searchKeys;

    private string searchString = "";
    private Vector2 scrollPosition = Vector2.zero;
    private SearchField searchField = new SearchField();

    private int selected = -1;

    public delegate void SearchableAction(int selectedItem);
    public event SearchableAction onWindowClosed;

    public SearchablePopup(string[] searchKeys)
    {
        this.searchKeys = searchKeys;
    }

    private void CreateStyle()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.hover = new GUIStyleState();
            labelStyle.hover.textColor = new Color(0.1f, 0.35f, 0.8f);
            Texture2D background = new Texture2D(10, 10);
            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    background.SetPixel(x, y, new Color(0, 0.2f, 1.0f));
                }
            }
            background.Apply();
            labelStyle.hover.background = background;
        }
    }

    public override void OnGUI(Rect rect)
    {
        CreateStyle();

        Vector2 size = GetWindowSize();
        //draw search bar
        Rect searchRect = GUILayoutUtility.GetRect(size.x, EditorGUIUtility.singleLineHeight);
        searchString = searchField.OnGUI(searchRect, searchString);

        //draw scroll window with options
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        List<int> indices = Search(searchString);
        for(int i = 0; i < indices.Count; i++)
        {
            if(GUILayout.Button(searchKeys[indices[i]], labelStyle))
            {
                selected = i;
                editorWindow.Close();
            }
        }

        GUILayout.EndScrollView();

        editorWindow.Repaint();
    }

    List<int> Search(string inputString)
    {

        List<int> results = new List<int>();
        for(int i = 0; i < searchKeys.Length; i++)
        {
            if(searchKeys[i].ToUpper().StartsWith(inputString.ToUpper()))
            {
                results.Add(i);
            }
        }
        return results;
    }

    public override void OnOpen()
    {
        base.OnOpen();

    }

    public override void OnClose()
    {
        base.OnClose();

        if(onWindowClosed != null)
        {
            onWindowClosed.Invoke(selected);
        }
        //ShouldClose = false;
    }
}
