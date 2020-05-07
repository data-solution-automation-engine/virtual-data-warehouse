using System;
using System.Collections.Generic;
using System.IO;

namespace Virtual_Data_Warehouse_Library
{
    public class EventLog : List<Event> { }

    public enum EventTypes
    {
        Information = 0,
        Error = 1,
        Warning = 2
    }

    public class Event
    {
        public int eventCode { get; set; }
        public string eventDescription { get; set; }

        public static Event CreateNewEvent(EventTypes eventType, string eventDescription)
        {
            var localEvent = new Event
            {
                eventCode = (int)eventType,
                eventDescription = eventDescription
            };

            return localEvent;
        }

        public static Event SaveEventToFile(string targetFile, Event inputEvent)
        {
            Event localEvent = new Event();
            try
            {
                //Output to file
                using (var outfile = new StreamWriter(targetFile))
                {
                    outfile.Write(inputEvent.eventCode + ": " + inputEvent.eventDescription);
                    outfile.Close();
                }

                localEvent = Event.CreateNewEvent(EventTypes.Information, "The file was successfully saved to disk.\r\n");
            }
            catch (Exception ex)
            {
                localEvent = Event.CreateNewEvent(EventTypes.Error, "There was an issue saving the output to disk. The message is: " + ex + ".\r\n");
            }

            return localEvent;
        }
    }
}
