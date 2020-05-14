using middler.Common.Variables;

namespace middler.Variables.LiteDB {

    public class VariableStorageEvent {

        public VariableStorageAction Action { get; }
        public TreeNode Node { get; }

        public VariableStorageEvent(VariableStorageAction action, TreeNode node) {
            Action = action;
            Node = node;
        }
    }

    public class VariableStorageEventDto
    {

        public VariableStorageAction Action { get; }
        public object Node { get; }

        public VariableStorageEventDto(VariableStorageAction action, object node) {
            Action = action;
            Node = node;
        }
    }

    public enum VariableStorageAction {

        Insert,
        Update,
        Delete

    }
}
