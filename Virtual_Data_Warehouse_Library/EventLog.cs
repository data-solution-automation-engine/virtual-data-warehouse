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

        public DateTime eventTime { get; set; } = DateTime.Now;


        /// <summary>
        /// Constructor that only captures type and description of an event. This will assume 'now' as date/time of the event.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventDescription"></param>
        /// <returns></returns>
        public static Event CreateNewEvent(EventTypes eventType, string eventDescription)
        {
            var localEvent = new Event
            {
                eventCode = (int)eventType,
                eventTime = DateTime.Now,
                eventDescription = eventDescription
            };

            return localEvent;
        }

        /// <summary>
        /// Constructor that also captures date / time.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventDateTime"></param>
        /// <param name="eventDescription"></param>
        /// <returns></returns>
        public static Event CreateNewEvent(EventTypes eventType, DateTime eventDateTime, string eventDescription)
        {
            var localEvent = new Event
            {
                eventCode = (int)eventType,
                eventTime = eventDateTime,
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
