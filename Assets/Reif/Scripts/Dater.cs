public static class Dater
{
    /// <summary>
    /// Returns current time in a form of "HH:mm:ss"
    /// </summary>
    public static string GetTime()
    {
        return System.DateTime.Now.ToString("HH:mm:ss");
    }

    /// <summary>
    /// Returns current date in a form of "yyyy-MM-dd"
    /// </summary>
    public static string GetDate()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd");
    }

}