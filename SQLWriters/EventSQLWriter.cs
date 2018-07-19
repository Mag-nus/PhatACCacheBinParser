﻿using System;
using System.Globalization;
using System.IO;

using PhatACCacheBinParser.Properties;
using PhatACCacheBinParser.SegB_GameEventDefDB;

namespace PhatACCacheBinParser.SQLWriters
{
    static class EventSQLWriter
    {
        public static void WriteEventFiles(GameEventDefDB gameEventDefDB)
        {
            var outputFolder = Settings.Default["OutputFolder"] + "\\" + "B GameEventDefDB" + "\\" + "\\SQL\\";

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            string sqlCommand = "INSERT";

            foreach (var gameEvent in gameEventDefDB.GameEventDefs)
            {
                string FileNameFormatter(GameEventDef obj) => Util.IllegalInFileName.Replace(obj.Name, "_");

                string fileNameFormatter = FileNameFormatter(gameEvent);

                using (StreamWriter writer = new StreamWriter(outputFolder + fileNameFormatter + ".sql"))
                {
                    string eventLine = "";

                    eventLine += $"     , ('{gameEvent.Name.Replace("'", "''")}', {(gameEvent.StartTime == -1 ? $"{gameEvent.StartTime}" : $"{gameEvent.StartTime} /* {DateTimeOffset.FromUnixTimeSeconds(gameEvent.StartTime).DateTime.ToUniversalTime().ToString(CultureInfo.InvariantCulture)} */")}, {(gameEvent.EndTime == -1 ? $"{gameEvent.EndTime}" : $"{gameEvent.EndTime} /* {DateTimeOffset.FromUnixTimeSeconds(gameEvent.EndTime).DateTime.ToUniversalTime().ToString(CultureInfo.InvariantCulture)} */")}, {(int)gameEvent.GameEventState})" + Environment.NewLine;

                    if (eventLine != "")
                    {
                        eventLine = $"{sqlCommand} INTO `event` (`name`, `start_Time`, `end_Time`, `state`)" + Environment.NewLine
                            + "VALUES " + eventLine.TrimStart("     ,".ToCharArray());
                        eventLine = eventLine.TrimEnd(Environment.NewLine.ToCharArray()) + ";" + Environment.NewLine;
                        writer.WriteLine(eventLine);
                    }
                }
            }
        }
    }
}