using UnityEngine;
using UnityEditor;
using ExcelDataReader;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Imports a user-selected Excel sheet → creates / updates DialogueNode assets.
/// Each row = one node with ONE dialogue line and up to 3 choices.
/// </summary>
public static class DialogueSheetImporter
{
    // ── Spreadsheet column indices (adjust if layout changes) ─────────────────────────
    private const int COL_ID          = 0;
    private const int COL_SPEAKER     = 2;
    private const int COL_LINE        = 3;
    private const int COL_NEXT        = 7;
    private const int COL_CH1_TEXT    = 11;
    private const int COL_CH1_ID      = 12;
    private const int COL_CH2_TEXT    = 13;
    private const int COL_CH2_ID      = 14;
    private const int COL_CH3_TEXT    = 15;
    private const int COL_CH3_ID      = 16;

    // ──────────────────────────────────────────────────────────────────────────────────
    [MenuItem("Tools/Dialogue/Import From Spreadsheet")]
    public static void ImportFromSheet()
    {
        //------------------------------------------
        // 1️⃣  Pick Excel file
        //------------------------------------------
        string sheetPath = EditorUtility.OpenFilePanel(
            "Select Dialogue Excel (.xlsx)",
            Application.streamingAssetsPath,
            "xlsx"
        );
        if (string.IsNullOrWhiteSpace(sheetPath))
        {
            Debug.Log("Dialogue import cancelled (no file selected).");
            return;
        }

        //------------------------------------------
        // 2️⃣  Pick output folder (must be inside Assets)
        //------------------------------------------
        string outFolderAbs = EditorUtility.OpenFolderPanel(
            "Select Output Folder for Dialogue Nodes",
            "Assets",
            ""
        );
        if (string.IsNullOrWhiteSpace(outFolderAbs) ||
            !outFolderAbs.StartsWith(Application.dataPath))
        {
            Debug.LogWarning("Import cancelled (invalid output folder).");
            return;
        }
        string outFolder = "Assets" + outFolderAbs.Substring(Application.dataPath.Length);

        //------------------------------------------
        RunImport(sheetPath, outFolder);
    }

    // ──────────────────────────────────────────────────────────────────────────────────
    private static void RunImport(string sheetPath, string outputFolder)
    {

        var nodeMap   = new Dictionary<string, DialogueNode>();
        var nextLinks = new List<(DialogueNode from, string toId)>();
        var choices   = new List<(DialogueNode from, string text, string toId)>();

        // ── PASS 1 – create / fill nodes ──────────────────────────────────────────────
        using (var stream = File.Open(sheetPath, FileMode.Open, FileAccess.Read))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            while (reader.Read())
            {
                string id      = reader.GetValue(COL_ID)?.ToString();
                if (string.IsNullOrWhiteSpace(id)) break;     // Blank first col = end

                string speaker = reader.GetValue(COL_SPEAKER)?.ToString();
                string line    = reader.GetValue(COL_LINE   )?.ToString();

                if (IsInvalid(speaker) || IsInvalid(line)) continue;

                // Create or update node
                if (!nodeMap.TryGetValue(id, out DialogueNode node))
                {
                    node = ScriptableObject.CreateInstance<DialogueNode>();
                    node.nodeID      = id;
                    node.speakerName = speaker;
                    node.line        = line;
                    node.responses   = new List<DialogueResponse>();

                    Directory.CreateDirectory(outputFolder);
                    AssetDatabase.CreateAsset(node, $"{outputFolder}/Node_{id}.asset");
                    nodeMap[id] = node;
                }
                else
                {
                    node.line = line;               // Overwrite if duplicate
                    EditorUtility.SetDirty(node);
                }

                // Default next link
                string next = reader.GetValue(COL_NEXT)?.ToString();
                if (!string.IsNullOrWhiteSpace(next))
                    nextLinks.Add((node, next));

                // Up to 3 branching choices
                AddChoice(reader, node, COL_CH1_TEXT, COL_CH1_ID, choices);
                AddChoice(reader, node, COL_CH2_TEXT, COL_CH2_ID, choices);
                AddChoice(reader, node, COL_CH3_TEXT, COL_CH3_ID, choices);
            }
        }

        // ── PASS 2 – resolve links & choices ──────────────────────────────────────────
        foreach (var (from, toId) in nextLinks)
            if (nodeMap.TryGetValue(toId, out var to))
            {
                from.nextNodeID = toId;
                from.nextNode   = to;
                EditorUtility.SetDirty(from);
            }

        foreach (var (from, txt, toId) in choices)
            if (nodeMap.TryGetValue(toId, out var to))
            {
                from.responses.Add(new DialogueResponse
                {
                    responseText = txt,
                    nextNode     = to
                });
                EditorUtility.SetDirty(from);
            }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Dialogue import complete. Nodes processed: {nodeMap.Count}");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────────────
    private static bool IsInvalid(string s) =>
        string.IsNullOrWhiteSpace(s) || s.Trim().ToUpper() is "N/A" or "NA";

    private static void AddChoice(IExcelDataReader rdr, DialogueNode node,
                                  int colTxt, int colID,
                                  List<(DialogueNode, string, string)> buf)
    {
        string text   = rdr.GetValue(colTxt)?.ToString();
        string nextId = rdr.GetValue(colID)?.ToString();
        if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(nextId))
            buf.Add((node, text, nextId));
    }
}
