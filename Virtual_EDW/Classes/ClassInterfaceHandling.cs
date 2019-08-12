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
            List<string> sourceBusinessKeyComponentList = new List<string>();

            sourceBusinessKeyDefinition = sourceBusinessKeyDefinition.Replace("(", "").Replace(")","");
            if (sourceBusinessKeyDefinition.StartsWith("COMPOSITE"))
            {
                sourceBusinessKeyDefinition = sourceBusinessKeyDefinition.Replace("COMPOSITE", "");
                sourceBusinessKeyComponentList = sourceBusinessKeyDefinition.Split(';').ToList();
            }
            else if (sourceBusinessKeyDefinition.StartsWith("CONCATENATE"))
            {
                sourceBusinessKeyDefinition = sourceBusinessKeyDefinition.Replace("CONCATENATE", "");
                sourceBusinessKeyDefinition = sourceBusinessKeyDefinition.Replace(";", "+");

                sourceBusinessKeyComponentList.Add(sourceBusinessKeyDefinition);
            }
            else
            {
                sourceBusinessKeyComponentList = sourceBusinessKeyDefinition.Split(',').ToList();
            }

            List<string> targetBusinessKeyComponentList = targetBusinessKeyDefinition.Split(',').ToList();

            sourceBusinessKeyComponentList = sourceBusinessKeyComponentList.Select(t => t.Trim()).ToList();
            targetBusinessKeyComponentList = targetBusinessKeyComponentList.Select(t => t.Trim()).ToList();

            int counter = 0;

            foreach (string keyPart in sourceBusinessKeyComponentList)
            {
                ColumnMapping keyComponent = new ColumnMapping();
 
                keyComponent.sourceColumn = keyPart;

                var indexExists = targetBusinessKeyComponentList.ElementAtOrDefault(counter) != null;
                if (indexExists)
                {
                    keyComponent.targetColumn = targetBusinessKeyComponentList[counter];
                }
                else
                {
                    keyComponent.targetColumn = "";
                }

                returnList.Add(keyComponent);
                counter++;
            }

            return returnList;
        }

        internal static string EvaluateBusinessKey(ColumnMapping businessKey)
        {
            var businessKeyEval = "";
            if (businessKey.sourceColumn.Contains("'"))
            {
                businessKeyEval = businessKey.sourceColumn;
            }
            else
            {
                businessKeyEval = "[" + businessKey.sourceColumn + "]";
            }

            return businessKeyEval;
        }
    }
}
