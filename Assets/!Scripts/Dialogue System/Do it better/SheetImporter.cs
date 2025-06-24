using UnityEngine;
using ExcelDataReader;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class SheetImporter : MonoBehaviour
{
    private const int _ID = 0;
    private const int _CHAR_NAME = 2;
    private const int _DIALOGUE = 3;
    private const int _NEXT_ID = 7;

    void Start()
    {
        StringBuilder sb = new StringBuilder();

        using (var stream = File.Open(Path.Combine(Application.streamingAssetsPath, "Dialogue.xlsx"), FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            { 
                do
                {
                    sb.Clear();
                    List<string> row = new List<string>();
                    while (reader.Read())
                    {
                        if (string.IsNullOrEmpty(reader.GetString(0))) break;
                        string parsed = $"{reader.GetString(0)} - {reader.GetString(2)}: {reader.GetString(3)}\n";
                        sb.Append(parsed);
                    }
                    
                    Debug.Log(sb.ToString());
                } while (reader.NextResult());
            }
        }
    }

    private string ConstructEntry(List<string> row)
    {
        string ID = row[_ID];
        string name = row[_CHAR_NAME];
        string dialogue = row[_DIALOGUE];

        return $"{ID} - {name}: {dialogue}\n";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
