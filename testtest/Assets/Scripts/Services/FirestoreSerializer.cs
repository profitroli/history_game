using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class FirestoreSerializer
{
    public static string SerializeFields(Dictionary<string, object> data)
    {
        var sb = new StringBuilder();
        sb.Append("{\"fields\":{");
        bool first = true;
        foreach (var kv in data)
        {
            if (!first) sb.Append(",");
            first = false;
            sb.Append($"\"{kv.Key}\":");
            sb.Append(SerializeValue(kv.Value));
        }
        sb.Append("}}");
        return sb.ToString();
    }

    private static string SerializeValue(object val)
    {
        if (val == null) return "{\"nullValue\":null}";
        if (val is string s) return $"{{\"stringValue\":\"{EscapeJson(s)}\"}}";
        if (val is int i) return $"{{\"integerValue\":\"{i}\"}}";
        if (val is long l) return $"{{\"integerValue\":\"{l}\"}}";
        if (val is float f) return $"{{\"doubleValue\":{f}}}";
        if (val is double d) return $"{{\"doubleValue\":{d}}}";
        if (val is bool b) return $"{{\"booleanValue\":{(b ? "true" : "false")}}}";
        if (val is Dictionary<string, object> map)
        {
            var sb = new StringBuilder();
            sb.Append("{\"mapValue\":{\"fields\":{");
            bool first = true;
            foreach (var kv in map)
            {
                if (!first) sb.Append(",");
                first = false;
                sb.Append($"\"{kv.Key}\":");
                sb.Append(SerializeValue(kv.Value));
            }
            sb.Append("}}}");
            return sb.ToString();
        }
        return $"{{\"stringValue\":\"{EscapeJson(val.ToString())}\"}}";
    }

    public static Dictionary<string, object> DeserializeDocument(string json)
    {
        try
        {
            var result = new Dictionary<string, object>();
            var doc = JsonUtility.FromJson<FirestoreDocument>(json);
            if (doc?.fields == null) return result;

            foreach (var field in doc.fields)
                result[field.key] = ExtractValue(field);

            return result;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[FirestoreSerializer] Ошибка парсинга: {e.Message}");
            return ParseFieldsManually(json);
        }
    }

    public static List<Dictionary<string, object>> DeserializeQueryResults(string json)
    {
        var results = new List<Dictionary<string, object>>();
        try
        {
            int docStart = 0;
            while ((docStart = json.IndexOf("\"document\"", docStart)) >= 0)
            {
                int fieldsStart = json.IndexOf("\"fields\"", docStart);
                if (fieldsStart < 0) break;

                int nameStart = json.IndexOf("\"name\"", docStart);
                string docPath = "";
                if (nameStart >= 0 && nameStart < fieldsStart)
                {
                    int valStart = json.IndexOf("\"", nameStart + 7) + 1;
                    int valEnd = json.IndexOf("\"", valStart);
                    docPath = json.Substring(valStart, valEnd - valStart);
                }

                var doc = ParseFieldsManually(json.Substring(fieldsStart));
                if (doc.Count > 0)
                {
                    string docId = docPath.Contains("/") ? docPath.Substring(docPath.LastIndexOf("/") + 1) : docPath;
                    doc["__docId__"] = docId;
                    results.Add(doc);
                }
                docStart = fieldsStart + 1;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[FirestoreSerializer] Ошибка парсинга запроса: {e.Message}");
        }
        return results;
    }

    private static Dictionary<string, object> ParseFieldsManually(string json)
    {
        var result = new Dictionary<string, object>();
        int idx = json.IndexOf("\"fields\"");
        if (idx < 0) return result;

        idx = json.IndexOf("{", idx + 8);
        if (idx < 0) return result;

        int depth = 1;
        int pos = idx + 1;
        while (pos < json.Length && depth > 0)
        {
            if (json[pos] == '{') depth++;
            else if (json[pos] == '}') depth--;

            if (depth == 1 && json[pos] == '"')
            {
                int keyEnd = json.IndexOf("\"", pos + 1);
                string key = json.Substring(pos + 1, keyEnd - pos - 1);

                int valueTypeStart = json.IndexOf("\"", keyEnd + 1);
                if (valueTypeStart < 0) break;
                int valueTypeEnd = json.IndexOf("\"", valueTypeStart + 1);
                string valueType = json.Substring(valueTypeStart + 1, valueTypeEnd - valueTypeStart - 1);

                if (valueType == "stringValue" || valueType == "integerValue" || valueType == "doubleValue" || valueType == "booleanValue")
                {
                    int valStart = json.IndexOf(":", valueTypeEnd) + 1;
                    while (valStart < json.Length && json[valStart] == ' ') valStart++;
                    bool isQuoted = json[valStart] == '"';
                    string rawVal;
                    if (isQuoted)
                    {
                        int valEnd = json.IndexOf("\"", valStart + 1);
                        rawVal = json.Substring(valStart + 1, valEnd - valStart - 1);
                    }
                    else
                    {
                        int valEnd = valStart;
                        while (valEnd < json.Length && json[valEnd] != ',' && json[valEnd] != '}') valEnd++;
                        rawVal = json.Substring(valStart, valEnd - valStart).Trim();
                    }

                    switch (valueType)
                    {
                        case "stringValue": result[key] = rawVal; break;
                        case "integerValue": result[key] = long.TryParse(rawVal, out long lv) ? (object)lv : rawVal; break;
                        case "doubleValue": result[key] = double.TryParse(rawVal, out double dv) ? (object)dv : rawVal; break;
                        case "booleanValue": result[key] = rawVal == "true"; break;
                    }
                    pos = valueTypeEnd + 1;
                }
                else if (valueType == "mapValue")
                {
                    int mColon = json.IndexOf(":", valueTypeEnd);
                    int mBrace = mColon >= 0 ? json.IndexOf("{", mColon) : -1;
                    if (mBrace >= 0)
                    {
                        int mEnd = FindMatchingBrace(json, mBrace);
                        if (mEnd > mBrace)
                        {
                            result[key] = ParseFieldsManually(json.Substring(mBrace, mEnd - mBrace + 1));
                            pos = mEnd + 1;
                        }
                        else pos = valueTypeEnd + 1;
                    }
                    else pos = valueTypeEnd + 1;
                }
                else
                {
                    pos = valueTypeEnd + 1;
                }
            }
            else
            {
                pos++;
            }
        }
        return result;
    }

    private static int FindMatchingBrace(string json, int openPos)
    {
        int d = 0;
        bool inStr = false;
        for (int i = openPos; i < json.Length; i++)
        {
            char c = json[i];
            if (inStr)
            {
                if (c == '\\') { i++; continue; }
                if (c == '"') inStr = false;
                continue;
            }
            if (c == '"') { inStr = true; continue; }
            if (c == '{') d++;
            if (c == '}') { d--; if (d == 0) return i; }
        }
        return -1;
    }

    private static object ExtractValue(FirestoreField field) => field.stringValue;

    private static string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    [Serializable] private class FirestoreDocument { public FirestoreField[] fields; }
    [Serializable] private class FirestoreField { public string key; public string stringValue; public string integerValue; }
}
