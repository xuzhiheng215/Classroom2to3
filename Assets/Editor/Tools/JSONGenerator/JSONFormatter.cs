using UnityEngine;
using System.Text;

public static class JSONFormatter
{
    public static string FormatJSON(string json, int indentSize = 2)
    {
        if (string.IsNullOrEmpty(json))
            return json;

        StringBuilder formattedJson = new StringBuilder();
        int indentLevel = 0;
        bool insideString = false;
        char lastChar = ' ';

        foreach (char c in json)
        {
            switch (c)
            {
                case '{':
                case '[':
                    formattedJson.Append(c);
                    if (!insideString)
                    {
                        formattedJson.Append('\n');
                        indentLevel++;
                        AddIndent(formattedJson, indentLevel, indentSize);
                    }
                    break;

                case '}':
                case ']':
                    if (!insideString)
                    {
                        formattedJson.Append('\n');
                        indentLevel--;
                        AddIndent(formattedJson, indentLevel, indentSize);
                    }
                    formattedJson.Append(c);
                    break;

                case ',':
                    formattedJson.Append(c);
                    if (!insideString)
                    {
                        formattedJson.Append('\n');
                        AddIndent(formattedJson, indentLevel, indentSize);
                    }
                    break;

                case ':':
                    formattedJson.Append(c);
                    if (!insideString)
                        formattedJson.Append(' ');
                    break;

                case '"':
                    formattedJson.Append(c);
                    if (lastChar != '\\')
                        insideString = !insideString;
                    break;

                case ' ': case '\t': case '\r': case '\n':
                    if (insideString)
                        formattedJson.Append(c);
                    break;

                default:
                    formattedJson.Append(c);
                    break;
            }

            lastChar = c;
        }

        return formattedJson.ToString();
    }

    private static void AddIndent(StringBuilder builder, int indentLevel, int indentSize)
    {
        builder.Append(' ', indentLevel * indentSize);
    }

    public static string CompactJSON(string json)
    {
        if (string.IsNullOrEmpty(json))
            return json;

        StringBuilder compactJson = new StringBuilder();
        bool insideString = false;
        char lastChar = ' ';

        foreach (char c in json)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    if (insideString)
                        compactJson.Append(c);
                    break;

                case '"':
                    compactJson.Append(c);
                    if (lastChar != '\\')
                        insideString = !insideString;
                    break;

                default:
                    compactJson.Append(c);
                    break;
            }

            lastChar = c;
        }

        return compactJson.ToString();
    }
}