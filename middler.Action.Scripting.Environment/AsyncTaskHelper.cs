using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
using Reflectensions.ExtensionMethods;
using Reflectensions.Helper;

namespace middler.Scripting
{
    public class AsyncTaskHelper
    {
        public object RunSync(Task task)
        {
            var type = task.GetType();
            if (type.GenericTypeArguments.Any())
            {
                return task.ConvertToTaskOf<object>().WaitAndUnwrapException();
                //return AsyncHelper.RunSync(() => (task).ConvertToTaskOf<object>());
            }

            task.WaitAndUnwrapException();
            //AsyncHelper.RunSync(() => task);
            return null;

        }

    }

   
}
