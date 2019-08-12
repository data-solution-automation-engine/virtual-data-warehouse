using System.Collections.Generic;
using System.Linq;
using System.Security;
using Virtual_EDW;

namespace Virtual_Data_Warehouse.Classes
{
    class InterfaceHandling
    {
        public static List<ColumnMapping> BusinessKeyComponentMappingList (string sourceBusinessKeyDefinition, string targetBusinessKeyDefinition)
        {
            List<ColumnMapping> returnList = new List<ColumnMapping>();

            // Evaluate source key components
            List<string> temporaryBusinessKeyComponentList = new List<string>();
            temporaryBusinessKeyComponentList = sourceBusinessKeyDefinition.Split(',').ToList(); // Split by the comma first to get the key parts

            List<string> sourceBusinessKeyComponentList = new List<string>();

            foreach (var keyComponent in temporaryBusinessKeyComponentList)
            {
                var keyPart = keyComponent.TrimStart().TrimEnd();
                keyPart = keyComponent.Replace("(", "").Replace(")", "").Replace(" ","");

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

            List<string> targetBusinessKeyComponentList = targetBusinessKeyDefinition.Split(',').ToList();

            sourceBusinessKeyComponentList = sourceBusinessKeyComponentList.Select(t => t.Trim()).ToList();
            targetBusinessKeyComponentList = targetBusinessKeyComponentList.Select(t => t.Trim()).ToList();

            int counter = 0;

            foreach (string keyPart in sourceBusinessKeyComponentList)
            {
                ColumnMapping keyComponent = new ColumnMapping();

                Column sourceColumn = new Column();
                Column targetColumn = new Column();

                sourceColumn.columnName = keyPart;                

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
