using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static class DialogueLoader
{
    public static DialogueConversation FromJson(TextAsset asset)
    {
        if (asset == null)
        {
            Debug.LogError("DialogueLoader: Cannot load a null dialogue asset.");
            return new DialogueConversation();
        }

        Dictionary<string, object> document;
        try
        {
            document = SimpleJson.Deserialize(asset.text) as Dictionary<string, object>;
        }
        catch (Exception exception)
        {
            Debug.LogError("DialogueLoader: Failed to parse dialogue asset " + asset.name + ": " + exception.Message);
            return new DialogueConversation();
        }

        if (document == null)
        {
            Debug.LogError("DialogueLoader: Failed to parse dialogue asset: " + asset.name);
            return new DialogueConversation();
        }

        Dictionary<string, object> meta = GetObject(document, "meta");
        Dictionary<string, object> nodes = GetObject(document, "nodes");

        DialogueConversation conversation = new DialogueConversation
        {
            conversationId = meta != null ? GetString(meta, "id") : asset.name,
            startNodeId = GetString(document, "startNode")
        };

        if (nodes == null)
            return conversation;

        foreach (KeyValuePair<string, object> nodePair in nodes)
        {
            DialogueNode node = ConvertNode(nodePair.Key, nodePair.Value as Dictionary<string, object>, meta);
            conversation.nodes.Add(node);
        }

        return conversation;
    }

    private static DialogueNode ConvertNode(string nodeId, Dictionary<string, object> dto, Dictionary<string, object> meta)
    {
        DialogueNode node = new DialogueNode
        {
            nodeId = nodeId,
            kind = ParseNodeKind(dto != null ? GetString(dto, "type") : null),
            speaker = dto != null ? GetString(dto, "speaker") : null,
            portraitCharacterId = meta != null ? GetString(meta, "character") : null,
            message = dto != null ? GetString(dto, "text") : null,
            nextNodeId = dto != null ? GetString(dto, "autoNext") : null
        };

        if (dto == null)
            return node;

        List<object> routes = GetList(dto, "routes");
        if (routes != null)
        {
            foreach (object routeValue in routes)
            {
                Dictionary<string, object> routeDto = routeValue as Dictionary<string, object>;
                if (routeDto == null)
                    continue;

                node.routes.Add(new DialogueRoute
                {
                    conditions = ConvertConditions(GetObject(routeDto, "conditions")),
                    nextNodeId = GetString(routeDto, "nextNode")
                });
            }
        }

        List<object> choices = GetList(dto, "choices");
        if (choices != null)
        {
            foreach (object choiceValue in choices)
            {
                Dictionary<string, object> choiceDto = choiceValue as Dictionary<string, object>;
                node.options.Add(ConvertOption(choiceDto));
            }
        }

        return node;
    }

    private static DialogueOption ConvertOption(Dictionary<string, object> dto)
    {
        DialogueOption option = new DialogueOption
        {
            optionText = dto != null ? GetString(dto, "text") : null,
            nextNodeId = dto != null ? GetString(dto, "nextNode") : null
        };

        if (dto != null)
        {
            ApplyConditions(option, ConvertConditions(GetObject(dto, "conditions")));
            Dictionary<string, object> effects = GetObject(dto, "effects");

            if (effects != null)
            {
                Dictionary<string, object> addTrust = GetObject(effects, "addTrust");
                if (addTrust != null)
                {
                    option.effects.Add(new DialogueEffect
                    {
                        trustChangeCharacter = GetString(addTrust, "character"),
                        trustChange = GetFloat(addTrust, "amount")
                    });
                }

                Dictionary<string, object> setFlag = GetObject(effects, "setFlag");
                if (setFlag != null)
                {
                    option.effects.Add(new DialogueEffect
                    {
                        flagKey = GetString(setFlag, "key"),
                        flagValue = GetBool(setFlag, "value", true)
                    });
                }

                string journalEntry = GetString(effects, "addJournalEntry");
                if (!string.IsNullOrEmpty(journalEntry))
                {
                    option.effects.Add(new DialogueEffect
                    {
                        journalEntry = journalEntry
                    });
                }
            }
        }

        return option;
    }

    private static DialogueConditionSet ConvertConditions(Dictionary<string, object> dto)
    {
        DialogueConditionSet conditions = new DialogueConditionSet();

        if (dto == null)
            return conditions;

        conditions.requiredFlag = GetString(dto, "requiresFlag");
        conditions.requiredNotFlag = GetString(dto, "requiresNotFlag");

        Dictionary<string, object> minTrust = GetObject(dto, "minTrust");
        if (minTrust != null)
        {
            conditions.trustCharacter = GetString(minTrust, "character");
            conditions.hasMinTrust = true;
            conditions.minTrust = GetFloat(minTrust, "value");
        }

        Dictionary<string, object> maxTrust = GetObject(dto, "maxTrust");
        if (maxTrust != null)
        {
            if (string.IsNullOrEmpty(conditions.trustCharacter))
                conditions.trustCharacter = GetString(maxTrust, "character");

            conditions.hasMaxTrust = true;
            conditions.maxTrust = GetFloat(maxTrust, "value", -1f);
        }

        return conditions;
    }

    private static void ApplyConditions(DialogueOption option, DialogueConditionSet conditions)
    {
        if (conditions == null)
            return;

        option.requiredFlag = conditions.requiredFlag;
        option.requiredNotFlag = conditions.requiredNotFlag;
        option.trustCharacter = conditions.trustCharacter;
        option.hasMinTrust = conditions.hasMinTrust;
        option.minTrust = conditions.minTrust;
        option.hasMaxTrust = conditions.hasMaxTrust;
        option.maxTrust = conditions.maxTrust;
    }

    private static DialogueNodeKind ParseNodeKind(string type)
    {
        if (string.Equals(type, "router", System.StringComparison.OrdinalIgnoreCase))
            return DialogueNodeKind.Router;

        if (string.Equals(type, "system", System.StringComparison.OrdinalIgnoreCase))
            return DialogueNodeKind.System;

        return DialogueNodeKind.Npc;
    }

    private static Dictionary<string, object> GetObject(Dictionary<string, object> source, string key)
    {
        if (source == null || !source.TryGetValue(key, out object value))
            return null;

        return value as Dictionary<string, object>;
    }

    private static List<object> GetList(Dictionary<string, object> source, string key)
    {
        if (source == null || !source.TryGetValue(key, out object value))
            return null;

        return value as List<object>;
    }

    private static string GetString(Dictionary<string, object> source, string key)
    {
        if (source == null || !source.TryGetValue(key, out object value) || value == null)
            return null;

        return value as string ?? Convert.ToString(value, CultureInfo.InvariantCulture);
    }

    private static float GetFloat(Dictionary<string, object> source, string key, float defaultValue = 0f)
    {
        if (source == null || !source.TryGetValue(key, out object value) || value == null)
            return defaultValue;

        return Convert.ToSingle(value, CultureInfo.InvariantCulture);
    }

    private static bool GetBool(Dictionary<string, object> source, string key, bool defaultValue = false)
    {
        if (source == null || !source.TryGetValue(key, out object value) || value == null)
            return defaultValue;

        return value is bool boolValue ? boolValue : defaultValue;
    }

    private static class SimpleJson
    {
        public static object Deserialize(string json)
        {
            return new Parser(json).ParseValue();
        }

        private class Parser
        {
            private readonly string json;
            private int index;

            public Parser(string json)
            {
                this.json = json ?? string.Empty;
            }

            public object ParseValue()
            {
                SkipWhitespace();

                switch (Peek)
                {
                    case '{':
                        return ParseObject();
                    case '[':
                        return ParseArray();
                    case '"':
                        return ParseString();
                    case 't':
                        ReadLiteral("true");
                        return true;
                    case 'f':
                        ReadLiteral("false");
                        return false;
                    case 'n':
                        ReadLiteral("null");
                        return null;
                    default:
                        return ParseNumber();
                }
            }

            private Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> table = new Dictionary<string, object>();
                Read('{');
                SkipWhitespace();

                if (Peek == '}')
                {
                    Read('}');
                    return table;
                }

                while (true)
                {
                    string key = ParseString();
                    SkipWhitespace();
                    Read(':');
                    table[key] = ParseValue();
                    SkipWhitespace();

                    if (Peek == '}')
                    {
                        Read('}');
                        return table;
                    }

                    Read(',');
                }
            }

            private List<object> ParseArray()
            {
                List<object> array = new List<object>();
                Read('[');
                SkipWhitespace();

                if (Peek == ']')
                {
                    Read(']');
                    return array;
                }

                while (true)
                {
                    array.Add(ParseValue());
                    SkipWhitespace();

                    if (Peek == ']')
                    {
                        Read(']');
                        return array;
                    }

                    Read(',');
                }
            }

            private string ParseString()
            {
                StringBuilder builder = new StringBuilder();
                Read('"');

                while (index < json.Length)
                {
                    char value = Next;
                    if (value == '"')
                        return builder.ToString();

                    if (value != '\\')
                    {
                        builder.Append(value);
                        continue;
                    }

                    char escaped = Next;
                    switch (escaped)
                    {
                        case '"':
                        case '\\':
                        case '/':
                            builder.Append(escaped);
                            break;
                        case 'b':
                            builder.Append('\b');
                            break;
                        case 'f':
                            builder.Append('\f');
                            break;
                        case 'n':
                            builder.Append('\n');
                            break;
                        case 'r':
                            builder.Append('\r');
                            break;
                        case 't':
                            builder.Append('\t');
                            break;
                        case 'u':
                            builder.Append(ParseUnicodeEscape());
                            break;
                        default:
                            throw new FormatException("Invalid JSON escape sequence.");
                    }
                }

                throw new FormatException("Unterminated JSON string.");
            }

            private char ParseUnicodeEscape()
            {
                if (index + 4 > json.Length)
                    throw new FormatException("Invalid JSON unicode escape.");

                string hex = json.Substring(index, 4);
                index += 4;
                return (char)int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            private object ParseNumber()
            {
                int start = index;

                while (index < json.Length && "-+0123456789.eE".IndexOf(json[index]) >= 0)
                {
                    index++;
                }

                if (start == index)
                    throw new FormatException("Unexpected JSON token: " + Peek);

                string number = json.Substring(start, index - start);
                if (number.IndexOf('.') >= 0 || number.IndexOf('e') >= 0 || number.IndexOf('E') >= 0)
                    return double.Parse(number, CultureInfo.InvariantCulture);

                return long.Parse(number, CultureInfo.InvariantCulture);
            }

            private char Peek => index < json.Length ? json[index] : '\0';

            private char Next
            {
                get
                {
                    if (index >= json.Length)
                        throw new FormatException("Unexpected end of JSON.");

                    return json[index++];
                }
            }

            private void Read(char expected)
            {
                SkipWhitespace();
                char actual = Next;
                if (actual != expected)
                    throw new FormatException("Expected '" + expected + "' but found '" + actual + "'.");
            }

            private void ReadLiteral(string literal)
            {
                for (int i = 0; i < literal.Length; i++)
                {
                    if (Next != literal[i])
                        throw new FormatException("Invalid JSON literal.");
                }
            }

            private void SkipWhitespace()
            {
                while (index < json.Length && char.IsWhiteSpace(json[index]))
                {
                    index++;
                }
            }
        }
    }
}
