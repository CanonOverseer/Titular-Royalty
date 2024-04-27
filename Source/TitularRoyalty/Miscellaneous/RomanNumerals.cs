﻿using System.Text;

namespace TitularRoyalty;

public static class RomanNumerals
{
    public static readonly Dictionary<char, int> RomanNumberDictionary;
    public static readonly Dictionary<int, string> NumberRomanDictionary;

    static RomanNumerals()
    {
        RomanNumberDictionary = new Dictionary<char, int>
        {
            { 'I', 1 },
            { 'V', 5 },
            { 'X', 10 },
            { 'L', 50 },
            { 'C', 100 },
            { 'D', 500 },
            { 'M', 1000 },
        };

        NumberRomanDictionary = new Dictionary<int, string>
        {
            { 1000, "M" },
            { 900, "CM" },
            { 500, "D" },
            { 400, "CD" },
            { 100, "C" },
            { 90, "XC" },
            { 50, "L" },
            { 40, "XL" },
            { 10, "X" },
            { 9, "IX" },
            { 5, "V" },
            { 4, "IV" },
            { 1, "I" },
        };
    }

    public static string FromInt(int number)
    {
        var roman = new StringBuilder();

        foreach (var item in NumberRomanDictionary)
        {
            while (number >= item.Key)
            {
                roman.Append(item.Value);
                number -= item.Key;
            }
        }

        return roman.ToString();
    }

    public static int ToInt(string roman)
    {
        int total = 0;
        char previousRoman = '\0';

        foreach (var currentRoman in roman)
        {
            var previous = previousRoman != '\0' ? RomanNumberDictionary[previousRoman] : '\0';
            var current = RomanNumberDictionary[currentRoman];

            if (previous != 0 && current > previous)
            {
                total = total - (2 * previous) + current;
            }
            else
            {
                total += current;
            }

            previousRoman = currentRoman;
        }

        return total;
    }
}