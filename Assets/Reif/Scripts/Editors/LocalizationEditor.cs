using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class LocalizationEditor: EditorWindow {

    private static int MARGIN = 18;

    private string[] localizationFiles;
    private List<LocalizationData> localizationDatas;
    private LocalizationData newLocalizationData;
    private string docName = "Not Named";
    private int removeAtIndex = -1;
    private bool addLanguage = false;
    private bool addItem = false;
    private string newItemKey = "";
    Vector2 scrollPos = new Vector2(0, 0);

    [MenuItem("Window/Localization/Localization Editor")]
    private static void Init() {
        // Get existing open window or if none, make a new one
        LocalizationEditor window = (LocalizationEditor)EditorWindow.GetWindow(typeof(LocalizationEditor));
        window.Show();
    }

    /// <summary>
    /// Handles the contents of Localization Editor window
    /// </summary>
    private void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 0, position.width, position.height));
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none);

        // Handles removal of language and a language file
        if(removeAtIndex != -1) {
            localizationDatas.RemoveAt(removeAtIndex);
            File.Delete(localizationFiles[removeAtIndex]);
            removeAtIndex = -1;
        }

        // Handles the situation when localization files were loaded
        if (localizationDatas != null)
        {
            // Handles localization editor content during creation of a new language
            if (addLanguage)
            {
                GUILayout.BeginVertical("Add New Language", "window");
                newLocalizationData.language.name = EditorGUILayout.TextField("Name", newLocalizationData.language.name == null ? "" : newLocalizationData.language.name);
                newLocalizationData.language.code = EditorGUILayout.TextField("Code", newLocalizationData.language.code == null ? "" : newLocalizationData.language.code);
                if (GUILayout.Button("Add"))
                    AddNewLanguage(newLocalizationData);

                if (GUILayout.Button("Cancel"))
                {
                    addLanguage = false;
                    newLocalizationData = null;
                }

                GUILayout.EndVertical();
            }
            // Handles localization editor content during creation of a new item
            else if (addItem)
            {
                GUILayout.BeginVertical("Add New Item", "window");
                newItemKey = EditorGUILayout.TextField("Item Key", newItemKey);
                if (GUILayout.Button("Add"))
                    AddNewItem();

                if (GUILayout.Button("Cancel"))
                {
                    addItem = false;
                    newItemKey = "";
                }

                GUILayout.EndVertical();
            }
            // Handles localization editor content with all the languages and items visible
            else
            {
                // Document Name Title
                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 };

                // LANGUAGE
                // Language Menu
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
                {
                    style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontStyle = FontStyle.Bold };
                    GUILayout.Label("Languages", style, GUILayout.Width(position.width - 30 - MARGIN));

                    if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30)))
                    {
                        addLanguage = true;
                        newLocalizationData = new LocalizationData();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Naming all languages
                for (int i = 0; i < localizationDatas.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                        removeAtIndex = i;

                    GUILayout.Label(localizationDatas[i].language.name);
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                // LOCALIZED TEXT
                // Localized Text Menu
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
                {
                    style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontStyle = FontStyle.Bold };
                    GUILayout.Label("Localized Strings", style, GUILayout.Width(position.width - 30 - MARGIN));

                    if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30)))
                        addItem = true;
                }
                EditorGUILayout.EndHorizontal();

                style = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 11 };

                // For each item
                if (localizationDatas.Count != 0)
                {
                    for (int i = 0; i < localizationDatas[0].items.Count; ++i)
                    {
                        EditorGUILayout.LabelField(localizationDatas[0].items[i].key, style, GUILayout.ExpandWidth(true));

                        EditorGUI.indentLevel++;

                        // For each language
                        for (int j = 0; j < localizationDatas.Count; ++j)
                            localizationDatas[j].items[i].value = EditorGUILayout.TextField(localizationDatas[j].language.name, localizationDatas[j].items[i].value);

                        EditorGUI.indentLevel--;
                    }
                }

                if (localizationDatas.Count != 0 && localizationDatas[0].items.Count > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Save Data"))
                        SaveData();
                }
            }
        }
        // Handles localization editor when localization datas were not yet loaded
        else if (!addLanguage && !addItem)
        {
            if (GUILayout.Button("Load Data"))
                LoadData();

            if (GUILayout.Button("Create New Data"))
                CreateNewData();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Handles creation of a new item
    /// </summary>
    private void AddNewItem() {
        for(int i = 0; i < localizationDatas.Count; ++i) {
            localizationDatas[i].items.Add(new LocalizationItem());
            localizationDatas[i].items[localizationDatas[i].items.Count - 1].key = newItemKey;
        }

        newItemKey = "";
        addItem = false;
    }

    /// <summary>
    /// Handles creation of a new localization language
    /// </summary>
    private void AddNewLanguage(LocalizationData localizationData) {
        localizationData.items = new List<LocalizationItem>();

        if(localizationDatas.Count != 0) {
            for(int i = 0; i < localizationDatas[0].items.Count; ++i) {
                localizationData.items.Add(new LocalizationItem());
                localizationData.items[i].key = localizationDatas[0].items[i].key;
                localizationData.items[i].value = "";
            }
        }

        localizationDatas.Add(localizationData);
        newLocalizationData = null;
        addLanguage = false;
    }

    /// <summary>
    /// Handles loading of localization files
    /// </summary>
    private void LoadData() {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.streamingAssetsPath, "json");
        string filenameWithoutPath = Path.GetFileName(filePath);
        string directoryName = Path.GetDirectoryName(filePath);
        int length = filenameWithoutPath.LastIndexOf("_") > 0 ? filenameWithoutPath.LastIndexOf("_") : filenameWithoutPath.Length - 5;
        docName = filenameWithoutPath.Substring(0, length);
        title = docName;

        localizationDatas = new List<LocalizationData>();
        localizationFiles = Directory.GetFiles(directoryName, "*.json", SearchOption.TopDirectoryOnly);

        // Go through all the files and compare whether they are from the same group (same base name)
        for(int i = 0; i < localizationFiles.Length; ++i) {
            if(localizationFiles[i].Contains(docName)) {
                if(!string.IsNullOrEmpty(localizationFiles[i])) {
                    string dataAsJson = File.ReadAllText(localizationFiles[i]);
                    LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

                    localizationDatas.Add(localizationData);
                }
            }
        }
    }

    /// <summary>
    /// Handles saving of localization files
    /// </summary>
    private void SaveData() {
        string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.streamingAssetsPath, "", "json");
        string filenameWithoutPath = Path.GetFileName(filePath);
        string directoryName = Path.GetDirectoryName(filePath);
        int length = filenameWithoutPath.LastIndexOf("_") > 0 ? filenameWithoutPath.LastIndexOf("_") : filenameWithoutPath.Length - 5;
        docName = filenameWithoutPath.Substring(0, length);
        title = docName;

        if(!string.IsNullOrEmpty(filePath)) {
            for(int i = 0; i < localizationDatas.Count; ++i) {
                string dataAsJson = JsonUtility.ToJson(localizationDatas[i]);
                var newFilePath = directoryName + "/" + docName + "_" + localizationDatas[i].language.code + ".json";
                File.WriteAllText(newFilePath, dataAsJson);
            }
        }
    }

    /// <summary>
    /// Handles creation of new data if language has no localization data yet
    /// </summary>
    private void CreateNewData() {
        localizationDatas = new List<LocalizationData>();
    }
}
#endif