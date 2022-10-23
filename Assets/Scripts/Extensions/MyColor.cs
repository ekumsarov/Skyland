﻿using UnityEngine;
using UnityEngine.UI;

public static class MyColor
{

	public static Color RandomBright => new Color(Random.Range(.4f, 1), Random.Range(.4f, 1), Random.Range(.4f, 1));

	public static Color RandomDim => new Color(Random.Range(.4f, .6f), Random.Range(.4f, .8f), Random.Range(.4f, .8f));
	
	public static Color RandomColor => new Color(Random.Range(.1f, .9f), Random.Range(.1f, .9f), Random.Range(.1f, .9f));


	public static Color WithAlphaSetTo(this Color color, float a)
	{
		return new Color(color.r, color.g, color.b, a);
	}

	public static void SetAlpha(this Image image, float a)
	{
		image.color = new Color(image.color.r, image.color.g, image.color.b, a);
	}

	public static void SetAlpha(this SpriteRenderer renderer, float a)
	{
		renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, a);
	}

	public static string ToHex(this Color color)
	{
		return $"#{(int) (color.r * 255):X2}{(int) (color.g * 255):X2}{(int) (color.b * 255):X2}";
	}

	public static Color BrightnessOffset(this Color color, float offset)
	{
		return new Color(
			color.r + offset,
			color.g + offset,
			color.b + offset,
			color.a);
	}

    public enum HighlightMark
    {
        Red,
        Green,
        Blue
    }


    public static Color GetHighlightColor(HighlightMark mark)
    {
        if (mark == HighlightMark.Red)
            return new Color(237, 46, 56);
        else if (mark == HighlightMark.Blue)
            return new Color(0, 51, 160);
        else // green
            return new Color(67, 176, 42);
    }

    public static void MarkImage(Image im, HighlightMark mark)
    {
        Color marked = MyColor.GetHighlightColor(mark);
        im.color = new Color(marked.r, marked.g, marked.b, im.color.a);
    }
}
