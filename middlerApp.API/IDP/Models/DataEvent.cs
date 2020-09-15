using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace middlerApp.API.IDP.Models
{
    public class DataEvent
    {
        public string Subject { get; set; }
        public DataEventAction Action { get; set; }

        public object Payload { get; set; }

        public Dictionary<string, object> MetaData { get; set; } = new Dictionary<string, object>();


        public DataEvent(DataEventAction action, string subject, object payload = null)
        {
            Action = action;
            Subject = subject;
            Payload = payload;
        }

        public DataEvent AddMetaData(string key, object value)
        {
            MetaData[key] = value;
            return this;
        }

        public static DataEvent Created(string subject, object payload = null)
        {
            return new DataEvent(DataEventAction.Created, subject, payload);
        }

        public static DataEvent Updated(string subject, object payload = null)
        {
            return new DataEvent(DataEventAction.Updated, subject, payload);
        }

        public static DataEvent Deleted(string subject, object payload = null)
        {
            return new DataEvent(DataEventAction.Deleted, subject, payload);
        }

    }

    public enum DataEventAction
    {
        Created,
        Updated,
        Deleted
    }
}
