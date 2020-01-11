using System;
using System.Collections.Generic;
using System.Text;

namespace middler.Common.Storage {

    public class MiddlerStorageEvent {

        public MiddlerStorageAction Action { get; }
        public MiddlerRuleDbModel Entity { get; }

        public MiddlerStorageEvent(MiddlerStorageAction action, MiddlerRuleDbModel entity) {
            Action = action;
            Entity = entity;
        }
    }

    public class MiddlerStorageEventDto {

        public MiddlerStorageAction Action { get; }
        public object Entity { get; }

        public MiddlerStorageEventDto(MiddlerStorageAction action, object entity) {
            Action = action;
            Entity = entity;
        }
    }

    public enum MiddlerStorageAction {

        Insert,
        Update,
        Delete

    }
}
