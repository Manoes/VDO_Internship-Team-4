using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class HighScoreEntry
{
    public string name;
    public int score;
}

[Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> entries = new();
}

public class HighScoreService
{
    private const int MaxEntries = 5;
    private readonly string filePath;

    public HighScoreData Data { get; private set; } = new HighScoreData();

    public HighScoreService(string fileName = "highscores.json")
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Load();
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Data = new HighScoreData();
                Save();
                return;
            }

            string json = File.ReadAllText(filePath);
            Data = JsonUtility.FromJson<HighScoreData>(json) ?? new HighScoreData();
            Data.entries ??= new List<HighScoreEntry>();

            SortAndTrim();
        }
        catch
        {
            Data = new HighScoreData();
            Save();
        }
    }

    public void Save()
    {
        SortAndTrim();

        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(filePath, json);
    }

    public IReadOnlyList<HighScoreEntry> GetTop()
    {
        return Data.entries;
    }

    public bool IsHighScore(int score)
    {
        if (Data.entries.Count < MaxEntries)
            return true;

        HighScoreEntry worst = Data.entries[^1];

        return score > worst.score;
    }

    public void AddHighScore(string name, int score)
    {
        name = SanitizeName(name);

        bool alreadyExists = Data.entries.Any(e =>
            e.name == name &&
            e.score == score
        );

        if (alreadyExists)
            return;

        if (!IsHighScore(score))
            return;

        Data.entries.Add(new HighScoreEntry
        {
            name = name,
            score = score
        });

        SortAndTrim();
        Save();
    }

    private void SortAndTrim()
    {
        Data.entries = Data.entries
            .OrderByDescending(e => e.score)
            .Take(MaxEntries)
            .ToList();
    }

    private static string SanitizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Unknown";

        name = new string(
            name.Trim()
                .Where(c => !char.IsControl(c))
                .ToArray()
        );

        return string.IsNullOrWhiteSpace(name)
            ? "Unknown"
            : name;
    }
}