using System.Collections.Generic;
using System.Linq;
using System.Security;
using Virtual_EDW;

namespace Virtual_Data_Warehouse.Classes
{
    class InterfaceHandling
    {
        public static List<BusinessKeyComponentMapping> BusinessKeyComponentMappingList (string sourceBusinessKeyDefinition, string targetBusinessKeyDefinition)
        {
            List<BusinessKeyComponentMapping> returnList = new List<BusinessKeyComponentMapping>();

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
            foreach (string keyPart in targetBusinessKeyComponentList)
            {
                BusinessKeyComponentMapping keyComponent = new BusinessKeyComponentMapping();

                keyComponent.sourceComponentName = sourceBusinessKeyComponentList[counter];
                keyComponent.targetComponentName = keyPart;
                returnList.Add(keyComponent);
                counter++;
            }

            return returnList;
        }


    }
}
