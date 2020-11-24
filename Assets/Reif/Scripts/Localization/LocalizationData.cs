using System;
using System.Collections.Generic;

[Serializable]
public class LocalizationData {
    public Language language = new Language();
    public List<LocalizationItem> items = new List<LocalizationItem>();
}

[Serializable]
public class Language {
    public string name;
    public string code;
}

[Serializable]
public class LocalizationItem {
    public string key;
    public string value;
}