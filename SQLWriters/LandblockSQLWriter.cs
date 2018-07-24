﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PhatACCacheBinParser.SQLWriters
{
    static class LandblockSQLWriter
    {
        public static void WriteFiles(IEnumerable<ACE.Database.Models.World.LandblockInstances> input, string outputFolder, Dictionary<uint, string> weenieNames, bool includeDELETEStatementBeforeInsert = false)
        {
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            // Sort the input by landblock
            var sortedInput = new Dictionary<uint, List<ACE.Database.Models.World.LandblockInstances>>();

            foreach (var value in input)
            {
                var landblock = (value.ObjCellId >> 16);

                if (sortedInput.TryGetValue(landblock, out var list))
                    list.Add(value);
                else
                    sortedInput.Add(landblock, new List<ACE.Database.Models.World.LandblockInstances> { value });
            }

            var sqlWriter = new ACE.Database.SQLFormatters.World.LandblockInstancesWriter();

            sqlWriter.WeenieNames = weenieNames;

            Parallel.ForEach(sortedInput, kvp =>
            //foreach (var kvp in sortedInput)
            {
                string fileName = sqlWriter.GetDefaultFileName(kvp.Value[0]);

                using (StreamWriter writer = new StreamWriter(outputFolder + fileName))
                {
                    if (includeDELETEStatementBeforeInsert)
                    {
                        sqlWriter.CreateSQLDELETEStatement(kvp.Value, writer);
                        writer.WriteLine();
                    }

                    sqlWriter.CreateSQLINSERTStatement(kvp.Value, writer);
                }
            });
        }
    }
}
