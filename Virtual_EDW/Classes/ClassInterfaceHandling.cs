using System.Collections.Generic;
using System.Linq;
using Virtual_Data_Warehouse;

namespace Virtual_Data_Warehouse
{
    class InterfaceHandling
    {
        public static List<ColumnMapping> BusinessKeyComponentMappingList (string sourceBusinessKeyDefinition, string targetBusinessKeyDefinition)
        {
            // Set the return type
            List<ColumnMapping> returnList = new List<ColumnMapping>();

            // Evaluate key components for source and target key definitions
            var sourceBusinessKeyComponentList = businessKeyComponentList(sourceBusinessKeyDefinition);
            var targetBusinessKeyComponentList = businessKeyComponentList(targetBusinessKeyDefinition);

            int counter = 0;

            foreach (string keyPart in sourceBusinessKeyComponentList)
            {
                var businessKeyEval = "";
                if (keyPart.StartsWith("'") && keyPart.EndsWith("'"))
                {
                    businessKeyEval = "HardCoded";
                }

                ColumnMapping keyComponent = new ColumnMapping();

                Column sourceColumn = new Column();
                Column targetColumn = new Column();

                sourceColumn.columnName = keyPart;
                sourceColumn.columnType = businessKeyEval;

                keyComponent.sourceColumn = sourceColumn;

                var indexExists = targetBusinessKeyComponentList.ElementAtOrDefault(counter) != null;
                if (indexExists)
                {
                    targetColumn.columnName = targetBusinessKeyComponentList[counter];                    
                }
                else
                {
                    targetColumn.columnName = "";
                }

                keyComponent.targetColumn = targetColumn;

                returnList.Add(keyComponent);
                counter++;
            }

            return returnList;
        }

        private static List<string> businessKeyComponentList(string sourceBusinessKeyDefinition)
        {
            List<string> temporaryBusinessKeyComponentList = new List<string>();
            temporaryBusinessKeyComponentList =
                sourceBusinessKeyDefinition.Split(',').ToList(); // Split by the comma first to get the key parts

            List<string> sourceBusinessKeyComponentList = new List<string>();

            foreach (var keyComponent in temporaryBusinessKeyComponentList)
            {
                var keyPart = keyComponent.TrimStart().TrimEnd();
                keyPart = keyComponent.Replace("(", "").Replace(")", "").Replace(" ", "");

                if (keyPart.StartsWith("COMPOSITE"))
                {
                    keyPart = keyPart.Replace("COMPOSITE", "");

                    var temporaryKeyPartList = keyPart.Split(';').ToList();
                    foreach (var item in temporaryKeyPartList)
                    {
                        sourceBusinessKeyComponentList.Add(item);
                    }
                }
                else if (keyPart.StartsWith("CONCATENATE"))
                {
                    keyPart = keyPart.Replace("CONCATENATE", "");
                    keyPart = keyPart.Replace(";", "+");

                    sourceBusinessKeyComponentList.Add(keyPart);
                }
                else
                {
                    sourceBusinessKeyComponentList.Add(keyPart);
                }
            }

            sourceBusinessKeyComponentList = sourceBusinessKeyComponentList.Select(t => t.Trim()).ToList();
            return sourceBusinessKeyComponentList;
        }

        internal static string EvaluateBusinessKey(ColumnMapping businessKey)
        {
            var businessKeyEval = "";
            if (businessKey.sourceColumn.columnName.Contains("'"))
            {
                businessKeyEval = businessKey.sourceColumn.columnName;
            }
            else
            {
                businessKeyEval = "[" + businessKey.sourceColumn.columnName + "]";
            }

            return businessKeyEval;
        }
    }
}
