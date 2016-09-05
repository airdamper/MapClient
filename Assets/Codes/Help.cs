using UnityEngine;
using System.Collections;
using System;
using System.IO;

public static class Help 
{
    public static int timestamp { get { return Timestamp(); } }

    static int Timestamp()
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
        DateTime nowTime = DateTime.Now;
        int unixTime = (int)Math.Round((nowTime - startTime).TotalSeconds, MidpointRounding.AwayFromZero);
        return unixTime;
    }
    public static bool WriteTxt(string fileName, string content)
    {
        try
        {
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            writer.Write(content);
            writer.Flush();
            writer.Close();
            stream.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
    public static string ReadTxt(string path)
    {
        try
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            return text;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public static int ToInt(this string content)
    {
        int result;
        Int32.TryParse(content, out result);
        return result;
    }
}
