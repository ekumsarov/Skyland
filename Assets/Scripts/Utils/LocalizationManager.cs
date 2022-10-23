using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Lodkod;

public enum Mark
{
    red,
    green,
    yellow
}

/// <summary>
/// Provides localizable string for selected system language
/// </summary>
public class LocalizationManager
{
    static string THOUSAND_KEY = "ui_short_thousand";
    static string MILLION_KEY = "ui_short_million";
    static string BILLION_KEY = "ui_short_billion";
    static string TRILLION_KEY = "ui_short_trillion";
    static string QUADRILLION_KEY = "ui_short_quadrillion";

    public static string GetRounded(float p, bool inverseCeiling = false, bool toFloat = false)
    {
        string res = "";
        try
        {
            float f = Convert.ToSingle(p);

            string formatted_number = toFloat ? p.ToString("F1") : Mathf.FloorToInt(f).ToString();

            if (f >= 1000 || f <= -1000)
                formatted_number = FormatByBase(f, 1000, instance.dictionary[THOUSAND_KEY]);
            if (f >= 1000000 || f <= -1000000)
                formatted_number = FormatByBase(f, 1000000, instance.dictionary[MILLION_KEY]);
            if (f >= 1000000000 || f <= -1000000000)
                formatted_number = FormatByBase(f, 1000000000, instance.dictionary[BILLION_KEY]);
            if (f >= 1000000000000 || f <= -100000000000)
                formatted_number = FormatByBase(f, 1000000000000, instance.dictionary[TRILLION_KEY]);
            if (f >= 1000000000000000 || f <= -1000000000000000)
                formatted_number = FormatByBase(f, 1000000000000000, instance.dictionary[QUADRILLION_KEY]);

            res = formatted_number;
        }
        catch
        { }

        return res;
    }

    public static string GetWithoutRounding(string key, params object[] obj)
    {
        if (instance.dictionary.ContainsKey(key))
        {
            string result = "";
            try
            {
                result = string.Format(instance.dictionary[key], obj);
            }
            catch (System.Exception e)
            {
                result = "ERROR: " + e.Message;
            }
            return result;
        }
        return key;
    }

    public static string Mark(Mark type, string str)
    {
        return "<color=" + type.ToString() + ">" + str + "</color>";
    }

    public static string Mark(Mark type, int str)
    {
        return "<color=" + type.ToString() + ">" + str + "</color>";
    }

    public static string Mark(Mark type, float str)
    {
        return "<color=" + type.ToString() + ">" + str + "</color>";
    }

    /// <summary>
    /// Returns localized formatted string
    /// </summary>
    /// <param name="key">String ID from localization file</param>
    /// <param name="obj...">List of parameters, separated by comma</param>
    /// <returns>Localized formatted string with parameter substitution</returns>
    public static string Get(string key, params object[] obj)
    {
        if (key == null)
            return "";
        if (instance.dictionary.ContainsKey(key))
        {
            string result = "";
            try
            {
                List<object> parrams = new List<object>();
                foreach (var p in obj)
                {
                    if (!(p is string))
                    {
                        try
                        {
                            float f = Convert.ToSingle(p);
                            parrams.Add(GetRounded(f));
                            continue;
                        }
                        catch
                        {
                            //object is not a number and do not required formatting
                        }
                    }
                    parrams.Add(p);
                }
                result = string.Format(instance.dictionary[key], parrams.ToArray());
            }
            catch (System.Exception e)
            {
                result = "ERROR: " + e.Message;
            }
            return result;
        }
        return key;
    }

    public static string GetRandomBuildName()
    {
        int index = UnityEngine.Random.Range(1, 20);

        int iter = 0;
        while(Instance.selectedName.Any(num => num == index) || iter < 100)
        {
            index = UnityEngine.Random.Range(1, 20);
            iter++;
        }

        string result = "&!Empty!&";

        if(!instance.dictionary.ContainsKey("name" + index))
        {
            result = "Cannot found Town name";
            return result;
        }


        Instance.selectedName.Add(index);
        result = instance.dictionary["name" + index];
        return result;
    }

    private static List<char> ignoredChars = new List<char>() { '\\', '{', '}', ':', '=', '<', '>', '"', '/', '.', ',', '-', '#' };

    private static string FixPunctuation(string str)
    {
        StringBuilder sb = new StringBuilder();
        bool met_punctuation = false;
        foreach (var c in str)
        {
            if (met_punctuation && c != ' ')
                sb.Append(' ');

            met_punctuation = false;
            sb.Append(c);
            if (Char.IsPunctuation(c) && !ignoredChars.Contains(c))
                met_punctuation = true;
        }

        return sb.ToString();
    }

    private static string FormatByBase(float value, long b, string label)
    {
        return string.Format("{0}{1}{2}", (int)(value / b),
            value % b > (b / 10) && (int)(value / b) < 10
                ? "." + (int)((value % b) / (b / 10))
                : ""
                , label);
    }

    public static void LoadLanguage(string language)
    {
        instance.LoadLanguageFromFile(language);
    }

    static LocalizationManager instance = new LocalizationManager();
    public static LocalizationManager Instance { get { return instance; } }

    Dictionary<string, string> dictionary;
    List<int> selectedName;

    LocalizationManager()
    {
        selectedName = new List<int>();
        dictionary = new Dictionary<string, string>();
        LoadLanguageFromFile("ru_ru");
        LoadLanguageForAI();
        LoadLanguageForMission();
        LoadLanguageForBuilds();
    }

    public static string CurrentLanguage { get; private set; }

    void LoadLanguageFromFile(string language)
    {
        CurrentLanguage = language;
        TextAsset localizedFile = Resources.Load<TextAsset>(
            GetFilepathForCode(language));

        if (localizedFile == null)
        {
            CurrentLanguage = GetCodeForLanguage(Application.systemLanguage);
            localizedFile = Resources.Load<TextAsset>(
                GetFilepathForCode(CurrentLanguage));
        }

        dictionary.Clear();
        string[] s = new string[2];

        foreach (var line in localizedFile.text.Split('\n'))
        {
            try
            {
                if (line.Trim().Equals(""))
                    continue;

                int mark = line.IndexOf("==");
                s[0] = line.Substring(0, mark);
                s[1] = line.Substring(mark + 2).Replace("\\n", "\n");
                dictionary.Add(s[0], FixPunctuation(s[1]));
            }
            catch (System.Exception ex)
            {
                Debug.Log(s[0] + " " + s[1] + " : " + ex.Message);
            }
        }
    }

    void LoadLanguageForMission()
    {
        TextAsset localizedFile = Resources.Load<TextAsset>(
            "missions/mission" + GM.mission + "/languages/" + "text_"+ CurrentLanguage);

        if(localizedFile == null)
        {
            Debug.LogError("Where no lamnguage file");
            return;
        }

        string[] s = new string[2];

        foreach (var line in localizedFile.text.Split('\n'))
        {
            try
            {
                if (line.Trim().Equals(""))
                    continue;

                int mark = line.IndexOf("==");
                s[0] = line.Substring(0, mark);
                s[1] = line.Substring(mark + 2).Replace("\\n", "\n");
                dictionary.Add(s[0], FixPunctuation(s[1]));
            }
            catch (System.Exception ex)
            {
                Debug.Log(s[0] + " " + s[1] + " : " + ex.Message);
            }
        }
    }

    void LoadLanguageForAI()
    {
        TextAsset localizedFile = Resources.Load<TextAsset>(
            "Languages/" + "AI_strings_" + CurrentLanguage);

        if (localizedFile == null)
        {
            Debug.LogError("Where no lamnguage file");
            return;
        }

        string[] s = new string[2];

        foreach (var line in localizedFile.text.Split('\n'))
        {
            try
            {
                if (line.Trim().Equals(""))
                    continue;

                int mark = line.IndexOf("==");
                s[0] = line.Substring(0, mark);
                s[1] = line.Substring(mark + 2).Replace("\\n", "\n");
                dictionary.Add(s[0], FixPunctuation(s[1]));
            }
            catch (System.Exception ex)
            {
                Debug.Log(s[0] + " " + s[1] + " : " + ex.Message);
            }
        }
    }

    void LoadLanguageForBuilds()
    {
        TextAsset localizedFile = Resources.Load<TextAsset>(
            "Languages/" + "randombuild_" + CurrentLanguage);

        if (localizedFile == null)
        {
            Debug.LogError("Where no lamnguage file");
            return;
        }

        string[] s = new string[2];

        foreach (var line in localizedFile.text.Split('\n'))
        {
            try
            {
                if (line.Trim().Equals(""))
                    continue;

                int mark = line.IndexOf("==");
                s[0] = line.Substring(0, mark);
                s[1] = line.Substring(mark + 2).Replace("\\n", "\n");
                dictionary.Add(s[0], FixPunctuation(s[1]));
            }
            catch (System.Exception ex)
            {
                Debug.Log(s[0] + " " + s[1] + " : " + ex.Message);
            }
        }
    }

    string GetCodeForLanguage(SystemLanguage language)
    {
        string lang_code = "en_us";
        switch (language)
        {
            case SystemLanguage.Belarusian:
            case SystemLanguage.Ukrainian:
            case SystemLanguage.Russian:
                lang_code = "ru_ru";
                break;
            case SystemLanguage.French:
                lang_code = "fr_fr";
                break;
            case SystemLanguage.German:
                lang_code = "de_de";
                break;
            case SystemLanguage.English:
                lang_code = "en_us";
                break;
            case SystemLanguage.Spanish:
                lang_code = "es_es";
                break;
            case SystemLanguage.Chinese:
                lang_code = "zh-CHS";
                break;
            default:
                lang_code = "en_us";
                break;
        }
        return lang_code;
    }

    string GetFilepathForCode(string lang_code)
    {
        return string.Format("Languages/strings_{0}", lang_code);
    }
}
