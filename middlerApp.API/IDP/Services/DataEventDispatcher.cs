using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using middlerApp.API.IDP.Models;

namespace middlerApp.API.IDP.Services
{
    public class DataEventDispatcher
    {

        private Subject<DataEvent> NotificationsSubject { get; } = new Subject<DataEvent>();
        public IObservable<DataEvent> Notifications => NotificationsSubject.Where(n => n != null).AsObservable();

        internal void DispatchEvent(DataEvent @event)
        {
            NotificationsSubject.OnNext(@event);
        }

        internal void DispatchEvent(DataEventAction action, string subject, object payload = null)
        {
            var ev = new DataEvent(action, subject, payload);
           DispatchEvent(ev);
        }

        internal void DispatchCreatedEvent(string subject, object payload = null)
        {
            DispatchEvent(DataEvent.Created(subject, payload));
        }

        internal void DispatchUpdatedEvent(string subject, object payload = null)
        {
            DispatchEvent(DataEvent.Updated(subject, payload));
        }

        internal void DispatchDeletedEvent(string subject, object payload = null)
        {
            DispatchEvent(DataEvent.Deleted(subject, payload));
        }
    }
}
