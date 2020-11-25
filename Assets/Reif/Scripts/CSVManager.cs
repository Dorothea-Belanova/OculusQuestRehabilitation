using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class CSVManager
{
    /// <summary>
    /// Loads patients exercise data from a CSV file.
    /// </summary>
    /// <param name="fileName">Exercise file name</param>
    public static List<string[]> Load(string fileName)
    {
        List<string[]> data = new List<string[]>();

        var filePath = Application.persistentDataPath + Constants.PATIENTS_DATA_DIRECTORY + "/" + fileName + Constants.CSV_FORMAT;

        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                string[] dataTemp = new string[values.Length];
                for (int i = 0; i < values.Length; ++i)
                    dataTemp[i] = values[i];

                data.Add(dataTemp);
            }
        }

        return data;
    }

    /// <summary>
    /// Saves patients exercise data as a CSV file.
    /// </summary>
    /// <param name="data">Patients exercise data</param>
    /// <param name="fileName">Exercise file name</param>
    public static void Save(List<string[]> data, string fileName)
    {
        // Converts list of arrays of string into array of arrays of string
        string[][] output = new string[data.Count][];

        for (int i = 0; i < output.Length; ++i)
            output[i] = data[i];

        // Creates a single string from data separated by a delimiter
        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        // Creates PatientsData repository if it doesnt exist
        string dirPath = Application.persistentDataPath + Constants.PATIENTS_DATA_DIRECTORY;

        if (Directory.Exists(dirPath) == false)
            Directory.CreateDirectory(dirPath);

        // Saves the file
        string filePath = dirPath + "/" + fileName + Constants.CSV_FORMAT;

        StreamWriter outStream = File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }
}
