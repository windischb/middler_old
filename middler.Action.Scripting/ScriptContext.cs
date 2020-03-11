using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using middler.Action.Scripting.ExtensionMethods;
using middler.Action.Scripting.Shared;
using middler.Common.SharedModels.Models;
using Reflectensions.ExtensionMethods;

namespace middler.Action.Scripting
{
    public class ScriptContext : IScriptContext
    {
        private ScriptContextMethods _scriptContextMethods;
        
        public IScriptContextResponse Response { get; }
        public IScriptContextRequest Request { get; }

        public ScriptContext(HttpContext httpContext, ScriptContextMethods scriptContextMethods)
        {
            _scriptContextMethods = scriptContextMethods;
            Request = new ScriptContextRequest(httpContext);
            Response = new ScriptContextResponse();
        }

        public void SendResponse()
        {
            _scriptContextMethods.SendResponse?.Invoke();
        }
    }

    public class ScriptContextRequest: IScriptContextRequest
    {
        public string HttpMethod { get; }
        public Uri Uri { get; }
        public Dictionary<string, object> RouteData { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; }
        public string UserAgent { get; }
        public string ClientIp { get; }
        public string[] ProxyServers { get; }

        public ScriptContextRequest(HttpContext httpContext)
        {
            HttpMethod = httpContext.Request.Method;
            Uri = new Uri(httpContext.Request.GetDisplayUrl());
            UserAgent = httpContext.Request.Headers["User-Agent"].ToString();
            ClientIp = httpContext.Request.FindSourceIp().FirstOrDefault()?.ToString();
            ProxyServers = httpContext.Request.FindSourceIp().Skip(1).Select(ip => ip.ToString()).ToArray();
            RouteData = new DynamicRouteData(httpContext.Features.Get<MiddlerRouteData>());
            Headers = new SimpleDictionary<object>(httpContext.Request.GetHeaders());
            QueryParameters = new DynamicQueryParameters(httpContext.Request.GetQueryParameters());
        }
    }

    public class ScriptContextResponse: IScriptContextResponse
    {
        public int? StatusCode { get; set; }
        public object Body { get; set; }
    }

    public class ScriptContextMethods
    {
        public System.Action SendResponse { get; set; }
    }

    public class DynamicRouteData : Dictionary<string, object> {


        public DynamicRouteData() : base() {

        }
        public DynamicRouteData(IDictionary<string, object> dict) : base(dict) {


        }
        public SimpleDictionary<object> AsSimpleDictionary() {
            return new SimpleDictionary<object>(this);
        }

        public new object this[string key] {
            get => TryGetValue(key?.ToLower(), out var val) ? val : null;
        }

        public new object Keys => this["keys"];

        public new object Values => this["values"];

    }

    public class DynamicQueryParameters : Dictionary<string, string> {

        private List<EndpointQueryParameter> QueryParameters { get; set; } = new List<EndpointQueryParameter>();

        public DynamicQueryParameters() : base() {

        }
        public DynamicQueryParameters(IDictionary<string, string> dict) : base(dict) {


        }

        public DynamicQueryParameters(IDictionary<string, string> dict, List<EndpointQueryParameter> queryParameters) : this(dict) {
            QueryParameters = queryParameters;
        }

        public SimpleDictionary<object> All() {
            var all = this
                .Select(kvp => {
                    return new KeyValuePair<string, object>(kvp.Key, GetValue(kvp.Key));
                });
            return new SimpleDictionary<object>(all);
        }

        public SimpleDictionary<object> Others() {
            var definedParams = QueryParameters.Select(q => q.Name);
            var others = this
                .Where(kvp => !definedParams.Contains(kvp.Key))
                .Select(kvp => {
                    return new KeyValuePair<string, object>(kvp.Key, GetValue(kvp.Key));
                });
            return new SimpleDictionary<object>(others);
        }


        public new object this[string key] {
            get => GetValue(key);
            
        }

        public new object Keys => this["keys"];

        public new object Values => this["values"];


        private object GetValue(string key) {

            if (TryGetValue(key, out var val)) {
                if (val == null) {
                    return null;
                }
                var isDefined = QueryParameters.FirstOrDefault(q => q.Name == key);
                if (isDefined != null) {
                    if (isDefined.IsArray) {
                        return val.Split(",");
                    }
                    return val;
                }
                return val;
            } else {
                return null;
            }

        }
    }

    public class EndpointQueryParameter {
        public string Name { get; set; }
        public bool IsArray { get; set; }
        public string Description { get; set; }
    }

    public class SimpleDictionary<T> : Dictionary<string, T> {

        private StringComparer StringComparer = StringComparer.CurrentCulture;

        public SimpleDictionary() : base(StringComparer.CurrentCulture) { }

        public SimpleDictionary(bool ignoreCase) : base(ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            StringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
        }

        public SimpleDictionary(StringComparer stringComparer) : base(stringComparer) {
            StringComparer = stringComparer;
        }

        public SimpleDictionary(IDictionary<string, T> dict) : base(dict, StringComparer.CurrentCulture) {

        }
        public SimpleDictionary(IDictionary<string, T> dict, bool ignoreCase) : base(dict, ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            StringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
        }

        public SimpleDictionary(IDictionary<string, T> dict, StringComparer stringComparer) : base(dict, stringComparer) {
            StringComparer = stringComparer;
        }

        public SimpleDictionary(SortedList<string, T> dict) : base(dict, StringComparer.CurrentCulture) {

        }
        public SimpleDictionary(SortedList<string, T> dict, bool ignoreCase) : base(dict, ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            StringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
        }

        public SimpleDictionary(SortedList<string, T> dict, StringComparer stringComparer) : base(dict, stringComparer) {
            StringComparer = stringComparer;
        }


        public SimpleDictionary(IEnumerable<KeyValuePair<string, T>> ienumerbale) : base(StringComparer.CurrentCulture) {
            foreach (var keyValuePair in ienumerbale) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValuePair<string, T>> ienumerbale, bool ignoreCase) : base(ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            StringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            foreach (var keyValuePair in ienumerbale) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValuePair<string, T>> ienumerbale, StringComparer stringComparer) : base(stringComparer) {
            StringComparer = stringComparer;
            foreach (var keyValuePair in ienumerbale) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValueItem<string, T>> ienumerbale) : base(StringComparer.CurrentCulture) {
            foreach (var keyValuePair in ienumerbale) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValueItem<string, T>> ienumerbale, bool ignoreCase) : base(ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            StringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            foreach (var keyValuePair in ienumerbale) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValueItem<string, T>> ienumerbale, StringComparer stringComparer) : base(stringComparer) {
            StringComparer = stringComparer;
            foreach (var keyValuePair in ienumerbale) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SimpleDictionary(object value) {

            var converter = new Reflectensions.Json();
            var json = converter.ToJson(value);
            var dict = converter.ToDictionary<string, T>(json);

            foreach (var keyValuePair in dict) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public bool IsEmpty() {
            return !this.Any();
        }

        public new string[] Keys => base.Keys.ToArray();

        public string[] GetKeys() {
            return Keys;
        }

        public new T this[string key] {
            get => TryGetValue(key, out var val) ? val : default(T);
            set {

                base[key] = value;
            }
        }

        public SimpleDictionary<T> Map(Action<KeyValueItem<string, T>> action) {

            var act = this.Select(kvp => {
                var kvi = new KeyValueItem<string, T>(kvp);
                return action.InvokeAction(kvi);
            });

            return new SimpleDictionary<T>(act.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        public SimpleDictionary<T> Filter(Func<KeyValueItem<string, T>, bool> filter) {

            var list = this.Select(kvp => new KeyValueItem<string, T>(kvp))
                .Where(filter);

            return new SimpleDictionary<T>(list.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), StringComparer);
        }

        public KeyValueItem<string, T>[] Entries() {
            return this.Select(kvp => new KeyValueItem<string, T>(kvp)).ToArray();
        }

        //public static SimpleDictionary<T> Merge(params SimpleDictionary<T>[] dicts) {

        //    var dict = new SimpleDictionary<T>();

        //    dicts.ToList().ForEach(d => {
        //        d.Entries().ToList().ForEach(ent => {
        //            dict[ent.Key] = ent.Value;
        //        });
        //    });

        //    return dict;
        //}

        public SimpleDictionary<T> Merge(SimpleDictionary<T> dict) {
            return Merge(new SimpleDictionary<T>[] {this, dict});
        }

        public static SimpleDictionary<T> Merge(SimpleDictionary<T>[] dicts) {

            
            var dict = new SimpleDictionary<T>();

            dicts.ToList().ForEach(d => {
                if (d == null)
                    return;

                d.Entries().ToList().ForEach(ent => {
                    dict[ent.Key] = ent.Value;
                });
            });

            return dict;
        }

        public class KeyValueItem<TKey, TValue> {
            public TKey Key { get; set; }
            public TValue Value { get; set; }

            public KeyValueItem() {

            }

            public KeyValueItem(KeyValuePair<TKey, TValue> kvp): this (kvp.Key, kvp.Value) {
           
            }

            public KeyValueItem(TKey key, TValue value) {
                Key = key;
                Value = value;
            }

            public static implicit operator KeyValuePair<TKey, TValue>(KeyValueItem<TKey, TValue> kvi) {
                return new KeyValuePair<TKey, TValue>(kvi.Key, kvi.Value);
            }
        }

    }


}
