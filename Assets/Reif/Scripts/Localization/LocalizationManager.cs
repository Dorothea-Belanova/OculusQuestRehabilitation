using UnityEngine;
using System.IO;
using UnityEngine.Android;
using UnityEngine.UI;

public class LocalizationManager: MonoBehaviour
{ 
    [SerializeField] private CurrentLocalizationLanguage currentLocalizationLanguage;
    [SerializeField] private string[] localizationFiles;
    [SerializeField] private Button changeLanguageButton;

    private int localizationIndex = 0;

    void Awake() {
        changeLanguageButton.onClick.AddListener(delegate {
            ChangeLocalizationLanguage();
        });

        if (!currentLocalizationLanguage.IsLoaded())
        {
            BetterStreamingAssets.Initialize();
            LoadLocalizationData(localizationFiles[localizationIndex]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ChangeLocalizationLanguage();
    }

    /// <summary>
    /// When triggered by Change language button, it loads next file in a localization files list.
    /// </summary>
    public void ChangeLocalizationLanguage()
    {
        localizationIndex = (localizationIndex + 1 >= localizationFiles.Length) ? 0 : ++localizationIndex;
        LoadLocalizationData(localizationFiles[localizationIndex]);
    }

    /// <summary>
    /// Loads localization data from JSON file.
    /// </summary>
    /// <param name="fileName">Language localization file name</param>
    public void LoadLocalizationData(string fileName) {
        // Uses BetterStreamingAssets asset to find file location
        string[] paths = BetterStreamingAssets.GetFiles(Constants.LOCALIZATION_FILES_DIRECTORY, fileName + ".json", SearchOption.AllDirectories);

        if (paths.Length > 0)
        {
            // Uses BetterStreamingAssets asset to read contents of a file
            string dataAsJson = BetterStreamingAssets.ReadAllText(paths[0]);
            Debug.Log(dataAsJson);

            // Deserialization of data
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            currentLocalizationLanguage.ChangeLocalizationLanguage(loadedData);
        }
    }

    /// <summary>
    /// Before the scene loads, the application asks for a permission to read and write into the external storage
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }
}