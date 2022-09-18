using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kinksweeper.Models;

public class KinkStorage
{
    private static readonly Random random = new();
    private readonly HashSet<string> _kinks = KinksBackup.GetBackupKinks();

    public KinkStorage()
    {
        if (!File.Exists("kinks.txt")) return;
        
        _kinks.Clear();
        using var streamReader = new StreamReader("kinks.txt");
        while (!streamReader.EndOfStream)
        {
            var str = streamReader.ReadLine();
            _kinks.Add(str!);
        }
    }

    public bool IsStorageEmpty() => _kinks.Count == 0;

    public string PopRandomKink()
    {
        var idx = random.Next(_kinks.Count);
        var result = _kinks.ElementAt(idx);
        _kinks.Remove(result);
        return result;
    }
}