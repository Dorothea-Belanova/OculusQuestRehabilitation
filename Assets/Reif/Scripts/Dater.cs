using UnityEngine;

public static class Dater
{

    public static string GetTime()
    {
        return System.DateTime.Now.ToString("HH:mm:ss");
    }

    public static string GetDate()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd");
    }

}