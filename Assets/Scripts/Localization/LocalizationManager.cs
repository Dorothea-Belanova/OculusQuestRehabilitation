using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Android;

//
// Summary:
//     Localization Manager is responsible for translation of application.
//
public class LocalizationManager: MonoBehaviour {

    public event Action OnLocalizationChange = delegate { };
    //public UnityEvent localizationChangedEvent = new UnityEvent();
    public static LocalizationManager instance;
    private const string missingTextString = "Localized text not found";
    private Language language;

    private Dictionary<string, string> localizedText;
    private bool isReady = false;

    void Awake() {
        if(instance == null) {
            instance = this;
        }
        else if(instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        BetterStreamingAssets.Initialize();

        LoadLocalizedText("localization_jp");
    }

    public Language GetLanguage() {
        return language;
    }

    public void LoadLocalizedText(string fileName) {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        localizedText = new Dictionary<string, string>();

        Debug.Log("PRED HLADANIM");
        string[] paths = BetterStreamingAssets.GetFiles("/", fileName + ".json", SearchOption.AllDirectories);
        Debug.Log("PO HLADANI");

        if (paths.Length != 0)
        {
            string dataAsJson = BetterStreamingAssets.ReadAllText(paths[0]);
            Debug.Log("FILE:  " + dataAsJson);

            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
            language = loadedData.language;

            for (int i = 0; i < loadedData.items.Count; ++i)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            Debug.Log("Data louded, dictionary contains: " + localizedText.Count + " entries.");
            Debug.Log("Language loaded: " + loadedData.language.name);
            Debug.Log("Language loaded: " + loadedData.language.code);
            OnLocalizationChange();
            isReady = true;
        }
    }

    public string GetLocalizedValue(string key) {
        if(localizedText.ContainsKey(key)) {
            return localizedText[key];
        }

        return missingTextString;
    }

    public bool GetIsReady() {
        return isReady;
    }
}