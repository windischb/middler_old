using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using middler.Common;
using middler.Common.Interfaces;
using middler.Common.Models;
using middler.Core.Map;
using Reflectensions.ExtensionMethods;

namespace middler.Core.ExtensionMethods {
    public static class MiddlerActionExtensions {
        
        //public static MiddlerAction ToBasicMiddlerAction<T>(this MiddlerAction<T> middlerAction, string actionTypeAlias) where T : class, new() {
        //    var act = new MiddlerAction {
        //        ContinueAfterwards = middlerAction.ContinueAfterwards,
        //        WriteStreamDirect = middlerAction.WriteStreamDirect,
        //        ActionType = actionTypeAlias,
        //        Parameters =
        //            middlerAction.Parameters?.GetType().GetProperties()
        //                .ToDictionary(p => p.Name, p => p.GetValue(middlerAction.Parameters)) ??
        //            new Dictionary<string, object>()
        //    };

        //    return act;
        //}

    }
}
