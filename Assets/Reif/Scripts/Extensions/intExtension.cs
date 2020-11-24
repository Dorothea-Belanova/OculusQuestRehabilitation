public static class intExtension {

    /// <summary>
    /// Evaluates whether int is even.
    /// </summary>
    public static bool IsEven(this int number) {
        return (number % 2 == 0);
    }

}