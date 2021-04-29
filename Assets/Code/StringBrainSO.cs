using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[CreateAssetMenu(fileName = "StringParsing", menuName = "StringProcessor")]
public class StringBrainSO : ScriptableObject
{
    [Range(0.1f, 1f)]
    public float weightThreshold = 0.7f;
    public int minimumNumOfChars = 4;
    [Range(0.1f, 1f)]
    public float SimilarityThreshold = 0.5f;

    public string[] CustomStringList;
    public TextAsset CustomXMLList;


    public void ParseCustomList()
    {
        if (CustomStringList == null) return;
        ProccessList(CustomStringList);
    }
    public void ParseXmlFile()
    {
        if (CustomXMLList == null) return;
        saveFile(ProccessList(loadFile().list));
    }
    public string[] ProccessList(string[] listOfStrings)
    {
        string[] proccessedList;
        List<string> duplicates = new List<string>();
        for (int i = 0; i < listOfStrings.Length; i++)
        {
            for (int k = i + 1; k < listOfStrings.Length; k++)
            {
                if (Similarity(listOfStrings[i], listOfStrings[k]) >= SimilarityThreshold)
                {
                    duplicates.Add(listOfStrings[i]);
                    duplicates.Add(listOfStrings[k]);
                    Debug.Log(listOfStrings[i] + " and " + listOfStrings[k] + " : are duplicates");
                }
            }
        }

        List<string> TempoList = new List<string>(listOfStrings);
        foreach (string s in duplicates)
            TempoList.Remove(s);

        proccessedList = TempoList.ToArray();
        return proccessedList;
    }
    public double Similarity(string s1, string s2)
    {
        int s1Length = s1.Length;
        int s2Length = s2.Length;
        if (s1Length == 0 || s2Length == 0)
            return 0;

        int searchRange = Mathf.Max(0, Mathf.Max(s1Length, s2Length) / 2 - 1);
        bool[] s1Matches = new bool[s1Length];
        bool[] s2Matches = new bool[s2Length];

        int match = 0;

        for (int i = 0; i < s1Length; i++)
        {
            int start = Mathf.Max(0, i - searchRange);
            int end = Mathf.Min(i + searchRange + 1, s2Length);
            for (int j = start; j < end; j++)
            {
                if (s2Matches[j]) continue;
                if (s1[i] != s2[j]) continue;
                s1Matches[i] = true;
                s2Matches[j] = true;
                match++;
                break;
            }
        }
        if (match == 0) return 0;

        int NumSemiTranspos = 0;
        int k = 0;
        for (int i = 0; i < s1Length; i++)
        {
            if (!s1Matches[i]) continue;
            while (!s2Matches[k]) k++;
            if (s1[i] != s2[k])
                NumSemiTranspos++;
            k++;
        }
        int NumTranspos = NumSemiTranspos / 2;
        double SimilaritiesD = match;
        double weight = (SimilaritiesD / s1Length + SimilaritiesD / s2Length + (match - NumTranspos) / SimilaritiesD) / 3f;
        if (weight <= weightThreshold) return weight;
        int sMax = Mathf.Min(minimumNumOfChars, Mathf.Min(s1.Length, s2.Length));
        int sPos = 0;
        while (sPos < sMax && s1[sPos] == s2[sPos]) sPos++;
        if (sPos == 0) return weight;
        return weight + 0.1f * sPos * (1f - weight);
    }
    public void saveFile(string[] _file)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(StringList));
        FileStream stream = new FileStream(Application.dataPath + "/XML_Exports/StringsList.xml", FileMode.Create);
        StringList sl = new StringList() { list = _file };
        serializer.Serialize(stream, sl);
        stream.Close();
        Debug.Log("XML DATA SAVED..");
    }
    public StringList loadFile()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(StringList));
        using (StringReader reader = new StringReader(CustomXMLList.text))
        {
            return serializer.Deserialize(reader) as StringList;
        }
    }

}
[System.Serializable]
public class StringList
{
    public string[] list;
}