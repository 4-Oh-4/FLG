using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies the text table in Resources/DialogueRewrites.txt to dialogue and objective text at display time.
///
/// The scenes keep their original lines; each line is looked up by its exact original text (whitespace
/// ignored) and swapped for the rewritten version. Nothing about when a conversation starts, how it
/// ends or which flag it sets changes: NPC_A still hands over the same conversation and gets the same
/// completion callback. Lines that aren't in the table are shown unchanged.
/// </summary>
public static class DialogueRewritesA {
    private const string TableResource = "DialogueRewrites";

    private struct Line {
        public string speaker; // null = keep the block's speaker
        public string text;
    }

    private static Dictionary<string, List<Line>> lines;
    private static Dictionary<string, string> names;

    /// <summary>Returns a rewritten copy of a conversation. The originals are never modified.</summary>
    public static DialougeA[] Apply(DialougeA[] dialogues) {
        if (dialogues == null) return null;
        Load();
        var result = new List<DialougeA>();
        foreach (DialougeA block in dialogues) {
            if (block == null) continue;
            string blockSpeaker = MapName(block.name);
            DialougeA current = null;
            if (block.sentences == null) continue;
            foreach (string sentence in block.sentences) {
                if (lines.TryGetValue(Normalize(sentence), out List<Line> replacement)) {
                    foreach (Line line in replacement) {
                        if (line.text == null) continue; // dropped line
                        Append(result, ref current, line.speaker ?? blockSpeaker, line.text);
                    }
                } else {
                    Append(result, ref current, blockSpeaker, Clean(sentence));
                }
            }
        }
        return result.ToArray();
    }

    /// <summary>Rewritten version of a block of text (objectives, journal pages). Several "=" lines become separate lines.</summary>
    public static string ApplyLine(string text) {
        if (string.IsNullOrEmpty(text)) return text;
        Load();
        if (lines.TryGetValue(Normalize(text), out List<Line> replacement)) {
            var kept = new List<string>();
            foreach (Line line in replacement) if (line.text != null) kept.Add(line.text);
            return string.Join("\n", kept);
        }
        return Clean(text);
    }

    // Consecutive lines by the same speaker share one box, like the original blocks.
    private static void Append(List<DialougeA> result, ref DialougeA current, string speaker, string text) {
        if (current == null || current.name != speaker) {
            current = new DialougeA { name = speaker, sentences = new string[0] };
            result.Add(current);
        }
        var list = new List<string>(current.sentences) { text };
        current.sentences = list.ToArray();
    }

    private static string MapName(string name) {
        if (name == null) return null;
        return names.TryGetValue(name.Trim(), out string mapped) ? mapped : name;
    }

    // Safety net for lines not in the table: no em dashes on screen.
    private static string Clean(string text) {
        if (text == null || text.IndexOf('—') < 0) return text;
        return text.Replace(" — ", ", ").Replace("—", ", ");
    }

    private static string Normalize(string text) {
        if (text == null) return "";
        return string.Join(" ", text.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries));
    }

    private static void Load() {
        if (lines != null) return;
        lines = new Dictionary<string, List<Line>>();
        names = new Dictionary<string, string>();
        TextAsset table = Resources.Load<TextAsset>(TableResource);
        if (table == null) {
            Debug.LogWarning("DialogueRewritesA: Resources/" + TableResource + " not found, showing original text.");
            return;
        }

        List<Line> target = null;
        foreach (string raw in table.text.Split('\n')) {
            string row = raw.TrimEnd('\r');
            if (row.StartsWith("@ ")) {
                target = new List<Line>();
                lines[Normalize(row.Substring(2))] = target;
            } else if (row.StartsWith("= ") || row == "=") {
                if (target == null) continue;
                string value = row.Length > 2 ? row.Substring(2).Trim() : "";
                if (value == "-") {
                    target.Add(new Line { text = null });
                } else if (value.StartsWith("[") && value.IndexOf(']') > 0) {
                    int close = value.IndexOf(']');
                    target.Add(new Line { speaker = value.Substring(1, close - 1), text = value.Substring(close + 1).Trim() });
                } else {
                    target.Add(new Line { text = value });
                }
            } else if (row.StartsWith("name ")) {
                int arrow = row.IndexOf("=>");
                if (arrow > 0) names[row.Substring(5, arrow - 5).Trim()] = row.Substring(arrow + 2).Trim();
            } else if (row.Trim().Length == 0 || row.StartsWith("#")) {
                // blank lines and comments end nothing; replacements may follow a comment
            }
        }
    }
}
