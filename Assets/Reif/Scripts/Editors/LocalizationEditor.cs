using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

#if UNITY_EDITOR
public class LocalizationEditor: EditorWindow {

    public static int MARGIN = 18;

    public List<LocalizationData> localizationDatas;
    public LocalizationData newLocalizationData;
    public bool isLanguageListVisible = true;
    public string docName = "Not Named";
    public int remove = -1;
    public bool addLanguage = false;
    public bool addItem = false;
    public string newItemKey = "";

    [MenuItem("Window/Localization Editor")]
    static void Init() {
        // Get existing open window or if none, make a new one:
        LocalizationEditor window = (LocalizationEditor)EditorWindow.GetWindow(typeof(LocalizationEditor));
        window.Show();
    }

    Vector2 scrollPos = new Vector2(0, 0);

    void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 0, position.width, position.height));
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none);

        if(remove != -1) {
            localizationDatas.RemoveAt(remove);
            remove = -1;

            // TODO add action on removing the file of the language
        }

        if(localizationDatas != null) {
            if(addLanguage) {
                GUILayout.BeginVertical("Add New Language", "window");
                newLocalizationData.language.name = EditorGUILayout.TextField("Name", newLocalizationData.language.name == null ? "" : newLocalizationData.language.name);
                newLocalizationData.language.code = EditorGUILayout.TextField("Code", newLocalizationData.language.code == null ? "" : newLocalizationData.language.code);
                if(GUILayout.Button("Add")) {
                    AddNewLanguage(newLocalizationData);
                }
                if(GUILayout.Button("Cancel")) {
                    addLanguage = false;
                    newLocalizationData = null;
                }

                GUILayout.EndVertical();
            }
            else if(addItem) {
                GUILayout.BeginVertical("Add New Item", "window");
                newItemKey = EditorGUILayout.TextField("Item Key", newItemKey);
                if(GUILayout.Button("Add")) {
                    AddNewItem();
                }
                if(GUILayout.Button("Cancel")) {
                    addItem = false;
                    newItemKey = "";
                }

                GUILayout.EndVertical();
            }
            else {
                // Document Name Title
                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 };
                //EditorGUILayout.LabelField("'" + docName + "' Files", style, GUILayout.ExpandWidth(true));
                //EditorGUILayout.Space();

                // LANGUAGE
                // Language Menu
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
                {
                    style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft, fontStyle = FontStyle.Bold };
                    GUILayout.Label("Languages", style, GUILayout.Width(position.width - 30 - MARGIN));

                    if(GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30))) {
                        addLanguage = true;
                        newLocalizationData = new LocalizationData();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Naming all languages
                for(int i = 0; i < localizationDatas.Count; ++i) {
                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button("X", GUILayout.Width(30))) {
                        remove = i;
                        Debug.Log("kliked X on " + localizationDatas[i].language.name);
                    }
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

                    if(GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(30))) {
                        addItem = true;
                    }
                }
                EditorGUILayout.EndHorizontal();

                style = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 11 };
                // For each item

                if(localizationDatas.Count != 0) {
                    for(int i = 0; i < localizationDatas[0].items.Count; ++i) {
                        EditorGUILayout.LabelField(localizationDatas[0].items[i].key, style, GUILayout.ExpandWidth(true));

                        EditorGUI.indentLevel++;
                        // For each language
                        for(int j = 0; j < localizationDatas.Count; ++j) {
                            localizationDatas[j].items[i].value = EditorGUILayout.TextField(localizationDatas[j].language.name, localizationDatas[j].items[i].value);
                        }
                        EditorGUI.indentLevel--;
                    }
                }

                if(localizationDatas.Count != 0 && localizationDatas[0].items.Count > 0) {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    if(GUILayout.Button("Save Data")) {
                        SaveData();
                    }
                }
            }
        }
        else if(!addLanguage && !addItem) {
            if(GUILayout.Button("Load Data")) {
                LoadData();
            }

            if(GUILayout.Button("Create New Data")) {
                CreateNewData();
            }
        }

        //Custom Button with Image as Thumbnail
        //1
        GUILayout.Space(20f);
        GUILayout.Label("Spawn Prop");
        GUILayout.BeginHorizontal();

        var thumbnailWidth = 50;
        var thumbnailHeight = 50;

        //2
        var texture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Icons/folder/add.png", typeof(Texture));
        // Define a GUIContent which uses the texture
        var textureContent = new GUIContent(texture);
        if(GUILayout.Button(textureContent, GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight))) {
            
        }

        if(GUILayout.Button(Resources.Load<Texture>("Icons/remove"),
        GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight))) {
            
        }

        if(GUILayout.Button(Resources.Load<Texture>("Icons/cancel"),
        GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight))) {
           
        }

        if(GUILayout.Button(Resources.Load<Texture>("Icons/ok"),
        GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight))) {
            
        }

        if(GUILayout.Button(Resources.Load<Texture>("Icons/edit"),
        GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight))) {
            
        }

        GUILayout.EndHorizontal(); //4

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void AddNewItem() {
        for(int i = 0; i < localizationDatas.Count; ++i) {
            localizationDatas[i].items.Add(new LocalizationItem());
            localizationDatas[i].items[localizationDatas[i].items.Count - 1].key = newItemKey;
        }

        newItemKey = "";
        addItem = false;
    }

    void AddNewLanguage(LocalizationData localizationData) {
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

    private void LoadData() {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.streamingAssetsPath, "json");
        string filenameWithoutPath = Path.GetFileName(filePath);
        string directoryName = Path.GetDirectoryName(filePath);
        int length = filenameWithoutPath.LastIndexOf("_") > 0 ? filenameWithoutPath.LastIndexOf("_") : filenameWithoutPath.Length - 5;
        docName = filenameWithoutPath.Substring(0, length);
        title = docName;

        localizationDatas = new List<LocalizationData>();
        string[] files = Directory.GetFiles(directoryName, "*.json", SearchOption.TopDirectoryOnly);
        // Go through all the files and compare, whether they are from the same group
        for(int i = 0; i < files.Length; ++i) {
            if(files[i].Contains(docName)) {
                if(!string.IsNullOrEmpty(files[i])) {
                    string dataAsJson = File.ReadAllText(files[i]);
                    LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

                    localizationDatas.Add(localizationData);
                }
            }
        }
    }

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

    private void CreateNewData() {
        localizationDatas = new List<LocalizationData>();
    }
}
#endif