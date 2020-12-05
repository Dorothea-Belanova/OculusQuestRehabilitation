using UnityEngine;

public static class Constants
{
    // STRINGS

    public const string MISSING_TEXT_STRING = "LOCALIZED TEXT NOT FOUND";

    public const string PATIENTS_DATA_DIRECTORY = "/PatientsData";

    public const string CSV_FORMAT = ".csv";

    public const string LOCALIZATION_FILES_DIRECTORY = "Localization";

    // NUMBERS  

    // Initial Scene Control
    public static float MIN_HAND_DISTANCE = 25f;

    public static float NUMBER_OF_POINTS_CALCULATION_MARGIN = 8f;

    public static int EXERCISE_LENGTH_STEP = 10;

    // Radius Clustering
    public const float RADIUS_MAX_LIMIT = 0.025f; // 2.5cm

    public const float RADIUS_MIN_LIMIT = 0.005f; // 0.5cm

    // Exercise Scene Control
    public static float CYLINDER_RADIUS = 0.03f;

    public static float CORNER_OFFSET = 0.05f;

    // LIMITS

    // Initial Scene Control
    public static Limits VERTICAL_GAME_HEIGHT_LIMITS = new Limits(15f, 40f); // with step 1 cm

    public static Limits MAX_HAND_DISTANCE_LIMITS = new Limits(35f, 70f); // with step 1 cm

    public static Limits EXERCISE_LENGTH_LIMITS = new Limits(12f, 100f); // with step 10 cm - EXERCISE_LENGTH_STEP

    // COLORS

    public static Color LEFT_HAND_COLOR = new Color(0.0f, 0.2f, 0.2f);

    public static Color RIGHT_HAND_COLOR = new Color(1.0f, 0.5f, 0.0f);
}
