using UnityEngine;
using System.IO;
using UnityEngine.Android;

public class LocalizationManager: MonoBehaviour
{ 
    [SerializeField] private CurrentLocalizationLanguage currentLocalizationLanguage;
    [SerializeField] private string initialLocalizationFileName = "localization_jp";

    void Awake() {
            LoadLocalizationData(initialLocalizationFileName);
    }

    /// <summary>
    /// Loads localization data from JSON file.
    /// </summary>
    /// <param name="fileName">Language localization file name</param>
    public void LoadLocalizationData(string fileName) {
        // Uses BetterStreamingAssets asset to find file location
        BetterStreamingAssets.Initialize();
        string[] paths = BetterStreamingAssets.GetFiles("/", fileName + ".json", SearchOption.AllDirectories);

        if (paths.Length > 0)
        {
            // Uses BetterStreamingAssets asset to read contents of a file
            string dataAsJson = BetterStreamingAssets.ReadAllText(paths[0]);

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