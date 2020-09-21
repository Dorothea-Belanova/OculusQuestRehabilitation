using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Globalization;

public class CSVManager
{

    public static List<string[]> Load(string path)
    {
        List<string[]> data = new List<string[]>();

        using (var reader = new StreamReader(path))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                string[] dataTemp = new string[values.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    dataTemp[i] = values[i];
                }
                data.Add(dataTemp);
            }
        }

        return data;
    }

    public static void Uloz(List<string[]> data, string name)
    {
        string[][] output = new string[data.Count][];

        for (int i = 0; i < output.Length; ++i)
        {
            output[i] = data[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        string filePath = Application.streamingAssetsPath + "/Parsed Data/" + name;

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    public void Save(List<string[]> data)
    {
        string[][] output = new string[data.Count][];

        for (int i = 0; i < output.Length; ++i)
        {
            output[i] = data[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        string filePath = getPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    // Following method is used to retrive the relative path as device platform
    private string getPath()
    {
        /*ApplicationControl applicationControl = GameObject.FindGameObjectWithTag("ApplicationControl").GetComponent<ApplicationControl>();
        string selectedHand;
        if (applicationControl.rehabilitationInfo.selectedHand == SelectedHand.BothHands)
        {
            selectedHand = "alternating";
        }
        else if (applicationControl.rehabilitationInfo.selectedHand == SelectedHand.LeftHand)
        {
            selectedHand = "left";
        }
        else
        {
            selectedHand = "right";
        }
        string addition = "";
        if (applicationControl.rehabilitationInfo.fixedExerciseLength)
        {
            addition += "_" + applicationControl.rehabilitationInfo.exerciseLength + "m_" + applicationControl.rehabilitationInfo.numberOfPoints + "points";
        }
        return Application.streamingAssetsPath + "/Patients Data/" + applicationControl.rehabilitationInfo.patientID + "_" + selectedHand + addition + "_" + Dater.GetDate() + ".csv";*/

        // FINISH
        string path = Application.streamingAssetsPath + "/Dori.csv";
        return path;
    }
}
