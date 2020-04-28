using System.Collections.Generic;
using System.Linq;
using DataWarehouseAutomation;

namespace Virtual_Data_Warehouse
{
    class InterfaceHandling
    {
        public static List<DataItemMapping> BusinessKeyComponentMappingList (string sourceBusinessKeyDefinition, string targetBusinessKeyDefinition)
        {
            // Set the return type
            List<DataItemMapping> returnList = new List<DataItemMapping>();

            // Evaluate key components for source and target key definitions
            var sourceBusinessKeyComponentList = businessKeyComponentList(sourceBusinessKeyDefinition);
            var targetBusinessKeyComponentList = businessKeyComponentList(targetBusinessKeyDefinition);

            int counter = 0;

            foreach (string keyPart in sourceBusinessKeyComponentList)
            {
                bool businessKeyEval = false;

                if (keyPart.StartsWith("'") && keyPart.EndsWith("'"))
                {
                    //businessKeyEval = "HardCoded";
                    businessKeyEval = true;
                }

                DataItemMapping keyComponent = new DataItemMapping();

                DataItem sourceColumn = new DataItem();
                DataItem targetColumn = new DataItem();

                sourceColumn.name = keyPart;
                sourceColumn.isHardCodedValue = businessKeyEval;

                keyComponent.sourceDataItem = sourceColumn;

                var indexExists = targetBusinessKeyComponentList.ElementAtOrDefault(counter) != null;
                if (indexExists)
                {
                    targetColumn.name = targetBusinessKeyComponentList[counter];                    
                }
                else
                {
                    targetColumn.name = "";
                }

                keyComponent.targetDataItem = targetColumn;

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

        internal static string EvaluateBusinessKey(DataItemMapping businessKey)
        {
            var businessKeyEval = "";
            if (businessKey.sourceDataItem.name.Contains("'"))
            {
                businessKeyEval = businessKey.sourceDataItem.name;
            }
            else
            {
                businessKeyEval = "[" + businessKey.sourceDataItem.name + "]";
            }

            return businessKeyEval;
        }
    }
}
