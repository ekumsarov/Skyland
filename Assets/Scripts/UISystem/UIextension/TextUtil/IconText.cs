using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Linq;

/// <summary>
/// Complete Util for show Text features: text typer, auto linewrapping, icons
/// </summary>
[RequireComponent(typeof(Text))]
public sealed class IconText : MonoBehaviour
{
    /// <summary>
    /// The print delay setting. Could make this an option some day, for fast readers.
    /// </summary>
    private const float PrintDelaySetting = 0.02f;

    // Characters that are considered punctuation in this language. TextTyper pauses on these characters
    // a bit longer by default. Could be a setting sometime since this doesn't localize.
    private readonly List<char> punctutationCharacters = new List<char>
        {
            '.',
            ',',
            '!',
            '?'
        };

    [SerializeField]
    [Tooltip("Event that's called when the text has finished printing.")]
    private UnityEvent printCompleted = new UnityEvent();

    [SerializeField]
    [Tooltip("Event called when a character is printed. Inteded for audio callbacks.")]
    private CharacterPrintedEvent characterPrinted = new CharacterPrintedEvent();

    private Text textComponent;
    private string printingText;
    private float defaultPrintDelay;
    private Coroutine typeTextCoroutine;

    protected string _animationDependence = null;

    /// <summary>
    /// Gets the PrintCompleted callback event.
    /// </summary>
    /// <value>The print completed callback event.</value>
    public UnityEvent PrintCompleted
    {
        get
        {
            return this.printCompleted;
        }
    }

    /// <summary>
    /// Gets the CharacterPrinted event, which includes a string for the character that was printed.
    /// </summary>
    /// <value>The character printed event.</value>
    public CharacterPrintedEvent CharacterPrinted
    {
        get
        {
            return this.characterPrinted;
        }
    }

    public Text TextComponent
    {
        get
        {
            if (this.textComponent == null)
            {
                this.textComponent = this.GetComponent<Text>();
            }

            return this.textComponent;
        }
    }

    public string String
    {
        get { return this.printingText; }
    }

    /// <summary>
    /// Prepare string to show
    /// </summary>
    /// <param name="text">Text to type.</param>
    /// <param name="printDelay">Print delay (in seconds) per character.</param>
    public void Text(string text)
    {
        this.Cleanup();
        this.CleanIcons();
        
        this.printingText =  text;
        CheckClickEnd();

        ProcessIconsTag();

        if (StringSymbols > 15)
            LineWrapping();

        var generator = new TypedTextGenerator();
        TypedTextGenerator.TypedText typedText;
        typedText = generator.GetTypedTextAt(this.printingText, 0);
        this.TextComponent.text = typedText.TextToPrint;
    }

    /// <summary>
    /// Types the text into the Text component character by character, using the specified (optional) print delay per character.
    /// </summary>
    /// <param name="text">Text to type.</param>
    /// <param name="printDelay">Print delay (in seconds) per character.</param>
    public void TypeText(float printDelay = -1)
    {
        this.Cleanup();

        this.defaultPrintDelay = printDelay > 0 ? printDelay : PrintDelaySetting;

        this.typeTextCoroutine = this.StartCoroutine(this.TypeTextCharByChar(this.printingText));
    }

    /// <summary>
    /// Skips the typing to the end.
    /// </summary>
    public void ShowComplete()
    {
        this.Cleanup();

        var generator = new TypedTextGenerator();
        var typedText = generator.GetCompletedText(this.printingText);
        this.TextComponent.text = typedText.TextToPrint;

        if (icons == null)
        {
            icons = new Dictionary<int, IcoText>();
            return;
        }
        
        this.TextComponent.cachedTextGeneratorForLayout.Populate(this.TextComponent.text, this.TextComponent.GetGenerationSettings(new Vector2(this.TextComponent.preferredWidth, this.textComponent.preferredHeight)));
        float unitPerPixel = 1 / this.TextComponent.pixelsPerUnit;
        float height = this.TextComponent.cachedTextGeneratorForLayout.lines[0].height * unitPerPixel;

        foreach (var index in icons.Keys)
        {
            if (icons[index].gameObject.activeSelf)
                continue;

            IcoText iconObject = icons[index];
            iconObject.Rect.sizeDelta = new Vector2(height, height);
            Vector2 vPos = this.TextComponent.cachedTextGeneratorForLayout.characters[iconObject.cursorPosition].cursorPos;
            iconObject.transform.localPosition = new Vector3(vPos.x, vPos.y) * unitPerPixel;
            iconObject.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Skips the typing to the end.
    /// </summary>
    public void Skip()
    {
        this.Cleanup();

        var generator = new TypedTextGenerator();
        var typedText = generator.GetCompletedText(this.printingText);
        this.TextComponent.text = typedText.TextToPrint;

        if (icons == null)
        {
            icons = new Dictionary<int, IcoText>();
            this.OnTypewritingComplete();
            return;
        }

        this.TextComponent.cachedTextGeneratorForLayout.Populate(this.TextComponent.text, this.TextComponent.GetGenerationSettings(new Vector2(this.TextComponent.preferredWidth, this.textComponent.preferredHeight)));
        float unitPerPixel = 1 / this.TextComponent.pixelsPerUnit;
        float height = this.TextComponent.cachedTextGeneratorForLayout.lines[0].height * unitPerPixel;

        foreach (var index in icons.Keys)
        {
            if (icons[index].gameObject.activeSelf)
                continue;

            IcoText iconObject = icons[index];
            iconObject.Rect.sizeDelta = new Vector2(height, height);
            Vector2 vPos = this.TextComponent.cachedTextGeneratorForLayout.characters[iconObject.cursorPosition].cursorPos;
            float width = this.TextComponent.cachedTextGeneratorForLayout.characters[iconObject.cursorPosition].charWidth;
            iconObject.transform.localPosition = new Vector3(vPos.x, vPos.y) * unitPerPixel;
            iconObject.gameObject.SetActive(true);
        }

        this.OnTypewritingComplete();
    }

    /// <summary>
    /// Determines whether this instance is skippable.
    /// </summary>
    /// <returns><c>true</c> if this instance is skippable; otherwise, <c>false</c>.</returns>
    public bool IsSkippable()
    {
        return this.typeTextCoroutine != null;
    }

    private void Cleanup()
    {
        if (this.typeTextCoroutine != null)
        {
            this.StopCoroutine(this.typeTextCoroutine);
            this.typeTextCoroutine = null;
        }
    }

    private void CleanIcons()
    {
        if (icons == null)
            icons = new Dictionary<int, IcoText>();

        foreach (var kvp in icons.Keys.ToList())
        {
            var temp = icons[kvp];
            icons.Remove(kvp);
            temp.transform.SetParent(null);
            GameObject.DestroyImmediate(temp.gameObject);
        }
        icons.Clear();
    }

    private IEnumerator TypeTextCharByChar(string text)
    {
        var generator = new TypedTextGenerator();
        TypedTextGenerator.TypedText typedText;
        int printedCharCount = 0;
        do
        {
            typedText = generator.GetTypedTextAt(text, printedCharCount);
            this.TextComponent.text = typedText.TextToPrint;
            this.OnCharacterPrinted(typedText.LastPrintedChar.ToString());

            if(this.icons.ContainsKey(typedText.VisiblePosition))
            {
                this.TextComponent.cachedTextGeneratorForLayout.Populate(this.TextComponent.text, this.TextComponent.GetGenerationSettings(new Vector2(this.TextComponent.preferredWidth, this.textComponent.preferredHeight)));
                float unitPerPixel = 1 / this.TextComponent.pixelsPerUnit;

                float height = this.TextComponent.cachedTextGeneratorForLayout.lines[0].height * unitPerPixel;
                IcoText iconObject = icons[typedText.VisiblePosition];
                iconObject.Rect.sizeDelta = new Vector2(height, height);
                Vector2 vPos = this.TextComponent.cachedTextGeneratorForLayout.characters[iconObject.cursorPosition].cursorPos;
                float width = this.TextComponent.cachedTextGeneratorForLayout.characters[iconObject.cursorPosition].charWidth;
                iconObject.transform.localPosition = new Vector3(vPos.x, vPos.y) * unitPerPixel;
                iconObject.gameObject.SetActive(true);
            }

            ++printedCharCount;

            var delay = typedText.Delay > 0 ? typedText.Delay : this.GetPrintDelayForCharacter(typedText.LastPrintedChar);
            yield return new WaitForSeconds(delay);
        }
        while (!typedText.IsComplete);

        this.typeTextCoroutine = null;
        this.OnTypewritingComplete();
    }

    private float GetPrintDelayForCharacter(char characterToPrint)
    {
        // Then get the default print delay for the current character
        float punctuationDelay = this.defaultPrintDelay * 8.0f;
        if (this.punctutationCharacters.Contains(characterToPrint))
        {
            return punctuationDelay;
        }
        else
        {
            return this.defaultPrintDelay;
        }
    }

    private void OnCharacterPrinted(string printedCharacter)
    {
        if (this.CharacterPrinted != null)
        {
            this.CharacterPrinted.Invoke(printedCharacter);
        }
    }

    private void OnTypewritingComplete()
    {
        if (this.PrintCompleted != null)
        {
            this.PrintCompleted.Invoke();
        }
    }

    /// <summary>
    /// Event that signals a Character has been printed to the Text component.
    /// </summary>
    [System.Serializable]
    public class CharacterPrintedEvent : UnityEvent<string>
    {
    }


    #region Self features
    const string TAG_CHECK_OPEN = "<ending/>";

    const string ICON_REPLACE_STRING_BEGIN = "<color=#ffffff00>";
    const string ICON_REPLACE_STRING = ICON_REPLACE_STRING_BEGIN + "W</color>";

    const string TAG_OPEN = "<icon=";
    const string TAG_CLOSE = "/>";

    const string COLOR_TAG_OPEN = "<color=";
    const string COLOR_TAG_CLOSE = ">";

    public int StringSymbols = 40;
    
    public void CheckClickEnd()
    {
        if (this.printingText.IndexOf(TAG_CHECK_OPEN) != -1)
        {
            string txt = this.printingText.Remove(this.printingText.IndexOf(TAG_CHECK_OPEN), TAG_CHECK_OPEN.Length);
            var endTxt = LocalizationManager.Get("Ending");
            this.printingText = txt + endTxt;
        }
    }

    void LineWrapping()
    {
        if (this.printingText.Length < StringSymbols)
            return;

        string input = this.printingText;

        List<string> inlist = new List<string>();

        List<string> cinlist = new List<string>();
        int diifString = 0;

        int pos = input.IndexOf(TAG_OPEN);
        while (pos > -1)
        {
            int name_pos = pos + TAG_OPEN.Length;
            int end_pos = input.IndexOf(TAG_CLOSE);
            int length = end_pos - name_pos;
            inlist.Add(input.Substring(pos, end_pos + TAG_CLOSE.Length - pos));
            input = input.Remove(pos, end_pos + TAG_CLOSE.Length - pos).Insert(pos, "~");

            pos = input.IndexOf(TAG_OPEN);
        }

        pos = input.IndexOf("</color>");
        while (pos > -1)
        {
            int name_pos = pos + "</color>".Length;
            int length = name_pos;
            input = input.Remove(pos, "</color>".Length).Insert(pos, "*");
            diifString++;

            pos = input.IndexOf("</color>");
        }

        pos = input.IndexOf(COLOR_TAG_OPEN);
        while (pos > -1)
        {
            int name_pos = pos + COLOR_TAG_OPEN.Length;
            int end_pos = input.IndexOf(COLOR_TAG_CLOSE);
            int length = end_pos - name_pos;
            cinlist.Add(input.Substring(pos, end_pos + COLOR_TAG_CLOSE.Length - pos));
            diifString++;
            input = input.Remove(pos, end_pos + COLOR_TAG_CLOSE.Length - pos).Insert(pos, "^");

            pos = input.IndexOf(COLOR_TAG_OPEN);
        }

        string[] lines = input.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

        string readyLine = "";

        for (int j = 0; j < lines.Length; j++)
        {
            string line = lines[j];
            string ends = line;
            ends.Trim();

            while (ends.Length > StringSymbols + diifString)
            {
                ends.Trim();

                int i = StringSymbols + diifString - 1;

                while (i > 0)
                {
                    if (Char.IsWhiteSpace(ends[i]) && !Char.IsWhiteSpace(ends[i - 1]))
                    {
                        readyLine += ends.Substring(0, i + 1);
                        readyLine += "\n";
                        ends = ends.Substring(i + 1);

                        i = 0;
                    }

                    i--;
                }
            }

            readyLine += ends;

            if (j < lines.Length - 2)
                readyLine += "\n";
        }

        pos = readyLine.IndexOf("~");
        while (pos > -1)
        {
            int name_pos = pos + 1;
            readyLine = readyLine.Remove(pos, 1).Insert(pos, inlist[0]);
            inlist.RemoveAt(0);

            pos = readyLine.IndexOf("~");
        }

        pos = readyLine.IndexOf("*");
        while (pos > -1)
        {
            int name_pos = pos + 1;
            readyLine = readyLine.Remove(pos, 1).Insert(pos, "</color>");

            pos = readyLine.IndexOf("*");
        }

        pos = readyLine.IndexOf("^");
        while (pos > -1)
        {
            int name_pos = pos + 1;
            readyLine = readyLine.Remove(pos, 1).Insert(pos, cinlist[0]);
            cinlist.RemoveAt(0);

            pos = readyLine.IndexOf("^");
        }

        readyLine.Trim();
//        readyLine += " ";
        this.printingText = readyLine;
    }

    string ReplaceColors(string result)
    {
        result = ChangeColor(result, "green", "#00ff00ff");
        result = ChangeColor(result, "red", "#ff3f3fff");
        result = ChangeColor(result, "blue", "#60b0f4ff");
        result = ChangeColor(result, "gray", "#bebebeff");
        result = ChangeColor(result, "purple", "#800080");
        return result;
    }

    string ChangeColor(string s, string old_color, string new_color)
    {
        return s.Replace(string.Format("<color={0}>", old_color), string.Format("<color={0}>", new_color));
    }
    #endregion

    #region Icons
    Dictionary<int, IcoText> icons;

    void ProcessIconsTag()
    {
        if (icons == null)
            icons = new Dictionary<int, IcoText>();
        if (this == null)
            return;

        foreach (var kvp in icons.Keys.ToList())
        {
            icons.Remove(kvp);
            icons[kvp].transform.SetParent(null);
            GameObject.DestroyImmediate(icons[kvp].gameObject);
        }
        icons.Clear();

        string txt = ReplaceColors(this.printingText);
        int pos = txt.IndexOf(TAG_OPEN);
        while (pos > -1)
        {
            int name_pos = pos + TAG_OPEN.Length;
            int end_pos = txt.IndexOf(TAG_CLOSE);
            int length = end_pos - name_pos;
            string icon_name = txt.Substring(name_pos, length);
            txt = txt.Remove(pos, end_pos + TAG_CLOSE.Length - pos).Insert(pos, ICON_REPLACE_STRING);

            IcoText temp = IcoText.Create(icons.Count, icon_name, transform, pos + ICON_REPLACE_STRING_BEGIN.Length);

            icons.Add(pos, temp);

            //Looking for next tag
            pos = txt.IndexOf(TAG_OPEN);
        }

        this.printingText = txt;
    }
    #endregion

    #region Reset

    public void ResetIconPosition()
    {
        if (icons == null)
            return;

        this.TextComponent.cachedTextGeneratorForLayout.Populate(this.TextComponent.text, this.TextComponent.GetGenerationSettings(new Vector2(this.TextComponent.preferredWidth, this.textComponent.preferredHeight)));
        float unitPerPixel = 1 / this.TextComponent.pixelsPerUnit;
        float height = this.TextComponent.cachedTextGeneratorForLayout.lines[0].height * unitPerPixel;

        foreach (var index in icons.Keys)
        {
            if (icons[index].gameObject.activeSelf)
                continue;

            IcoText iconObject = icons[index];
            iconObject.GetComponent<RectTransform>().sizeDelta = new Vector2(height, height);
            Vector2 vPos = this.TextComponent.cachedTextGeneratorForLayout.characters[iconObject.cursorPosition].cursorPos;
            float width = this.TextComponent.cachedTextGeneratorForLayout.characters[iconObject.cursorPosition].charWidth;
            iconObject.transform.localPosition = new Vector3(vPos.x + width, vPos.y) * unitPerPixel;
            iconObject.gameObject.SetActive(true);
        }
    }

    #endregion
}

public class IcoText : Image
{
    public int cursorPosition;
    public RectTransform Rect;

    public static IcoText Create(int count, string icon_name, Transform trans, int cursor)
    {
        GameObject iconObject = new GameObject("icon_" + count, typeof(IcoText));
        iconObject.transform.SetParent(trans);
        iconObject.GetComponent<RectTransform>().pivot = new Vector2(0.1f, 1f);
        iconObject.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        iconObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        iconObject.transform.localScale = Vector3.one;
        iconObject.transform.position = new Vector3(-50, -50);
        iconObject.transform.localRotation = Quaternion.identity;
        IcoText img = iconObject.GetComponent<IcoText>();
        img.Rect = iconObject.GetComponent<RectTransform>();
        img.sprite = TextIconProvider.GetIcon(icon_name);
        img.gameObject.SetActive(false);
        img.cursorPosition = cursor;

        return img;
    }
}