using System.Collections.Generic;

public static class StringExtensions
{
    public static List<string> SplitByLength(this string input, int partLength)
    {
        string[] words = input.Split(' ');
        var parts = new List<string>();
        string part = string.Empty;
        int partCounter = 0;
        foreach (var word in words)
        {
            if (part.Length + word.Length < partLength)
            {
                part += string.IsNullOrEmpty(part) ? word : " " + word;
            }
            else
            {
                parts.Add(part);
                part = word;
                partCounter++;
            }
        }

        parts.Add(part);

        return parts;
    }
}