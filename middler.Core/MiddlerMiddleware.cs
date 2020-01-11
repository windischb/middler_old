using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using middler.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing.Constraints;
using middler.Common;
using middler.Common.Enums;
using middler.Common.ExtensionMethods;
using middler.Common.Models;
using middler.Core.ExtensionMethods;
using middler.Core.Models;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;

namespace middler.Core {
    public class MiddlerMiddleware {

        private readonly RequestDelegate _next;
        private ILogger Logger { get; set; }
        private ILogger ConstraintLogger { get; set; }

        public MiddlerMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMiddlerOptions middlerOptions) {

            EnsureLoggers(context);
            Stream originalBody = null;

            var middlerMap = context.RequestServices.GetRequiredService<IMiddlerMap>();
            var endpoints = middlerMap.GetFlatList(context.RequestServices);

            MiddlerActionContext actionContext = null;
            var intHelper = new InternalHelper(context.RequestServices);

            var executedActions = new Stack<IMiddlerAction>();

            try {
                bool @continue;
                do {

                    var matchingEndpoint = FindMatchingEndpoint(middlerOptions, endpoints, context);


                    if (matchingEndpoint == null) {
                        await _next(context).ConfigureAwait(false);
                        break;
                    }

                    if (matchingEndpoint.AccessMode == AccessMode.Deny) {
                        await context.Forbid().ConfigureAwait(false);
                        return;
                    }

                    endpoints = matchingEndpoint.RemainingEndpointInfos;

                    actionContext = new MiddlerActionContext(middlerOptions, context, matchingEndpoint);

                    var interMediateStreamNeeded = matchingEndpoint.MiddlerRule.Actions.Any(a => !a.WriteStreamDirect);
                    if (interMediateStreamNeeded) {
                        originalBody ??= context.Response.Body;
                        actionContext.ResponseBody =
                            new AutoStream(middlerOptions.AutoStreamDefaultMemoryThreshold,
                                context.RequestAborted);
                        context.Response.Body = actionContext.ResponseBody;
                    }


                    @continue = false;

                    foreach (var endpointAction in matchingEndpoint.MiddlerRule.Actions)
                    {


                        var action = intHelper.BuildConcreteActionInstance(endpointAction);
                        
                        if (action != null) {
                            await ExecuteRequestAction(action, context.RequestServices, actionContext);
                            executedActions.Push(action);
                            @continue = action.ContinueAfterwards;
                        }
                        
                        if (!@continue)
                            break;
                    }


                } while (@continue);

                while (executedActions.TryPop(out var executedAction))
                {
                    await ExecuteResponseAction(executedAction, context.RequestServices, actionContext);
                }


                await WriteToAspNetCoreResponseBody(context, actionContext, originalBody).ConfigureAwait(false);
            } catch (Exception ex) {

                Logger.LogError(ex, ex.Message);

                if (actionContext?.ResponseBody != null) {
                    context.Response.RegisterForDispose(actionContext.ResponseBody);
                }
                throw;
            }



        }
        

        private async Task ExecuteRequestAction(IMiddlerAction middlerAction, IServiceProvider serviceProvider, MiddlerActionContext actionContext)
        {
            var method = middlerAction.GetType().GetMethod("ExecuteRequestAsync") ?? middlerAction.GetType().GetMethod("ExecuteRequest");
            if (method == null)
            {
                return;
            }

            var man = new Dictionary<Type, object>
            {
                { typeof(IMiddlerActionContext), actionContext }
            };

            var parameters = BuildExecuteMethodParameters(method, serviceProvider, man);

            if (typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                var t = (Task)method.Invoke(middlerAction, parameters);
                await t.ConfigureAwait(false);
            }
            else
            {
                method.Invoke(middlerAction, parameters);
            }

        }

        private async Task ExecuteResponseAction(IMiddlerAction middlerAction, IServiceProvider serviceProvider, MiddlerActionContext actionContext)
        {
            var method = middlerAction.GetType().GetMethod("ExecuteResponseAsync") ?? middlerAction.GetType().GetMethod("ExecuteResponse");
            if (method == null)
            {
                return;
            }

            var man = new Dictionary<Type, object>
            {
                { typeof(IMiddlerActionContext), actionContext }
            };

            var parameters = BuildExecuteMethodParameters(method, serviceProvider, man);

            if (typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                var t = (Task)method.Invoke(middlerAction, parameters);
                await t.ConfigureAwait(false);
            }
            else
            {
                method.Invoke(middlerAction, parameters);
            }

        }


        private object[] BuildExecuteMethodParameters(MethodInfo methodInfo, IServiceProvider serviceProvider, Dictionary<Type, object> manualCreateParameters) {

            return methodInfo.GetParameters().Select(p => {
                if (manualCreateParameters.TryGetValue(p.ParameterType, out var manualCreated))
                    return manualCreated;

                return serviceProvider.GetRequiredService(p.ParameterType);
            }).ToArray();

        }


        private async Task WriteToAspNetCoreResponseBody(HttpContext context, IMiddlerActionContext? actionContext, Stream? originalBody) {
            if (actionContext == null || originalBody == null)
                return;

            context.Response.Body = originalBody;

            actionContext.ResponseBody.Seek(0, SeekOrigin.Begin);
            await actionContext.ResponseBody.CopyToAsync(context.Response.Body, 131072, context.RequestAborted).ConfigureAwait(false);
            if (context.Response.Body.CanWrite) {
                await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
            }

            await actionContext.ResponseBody.DisposeAsync();
        }


        private MiddlerRuleMatch FindMatchingEndpoint(IMiddlerOptions middlerOptions, List<MiddlerRule> rule, HttpContext context) {

            for (int i = 0; i < rule.Count; i++) {
                var match = CheckMatch(middlerOptions, rule[i], context);
                if (match != null) {

                    if (match.AccessMode != AccessMode.Ignore) {
                        match.RemainingEndpointInfos = rule.Skip(i + 1).ToList();
                        return match;
                    }
                }

            }

            return null;
        }


        private MiddlerRuleMatch CheckMatch(IMiddlerOptions middlerOptions, MiddlerRule rule, HttpContext context) {

            var allowedHttpMethods = rule.HttpMethods?.IgnoreNullOrWhiteSpace().Any() == true ? rule.HttpMethods.IgnoreNullOrWhiteSpace() : middlerOptions.DefaultHttpMethods;

            if (!allowedHttpMethods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase))
                return null;

            var uri = new Uri(context.Request.GetEncodedUrl());

            var allowedSchemes = rule.Scheme?.IgnoreNullOrWhiteSpace().Any() == true ? rule.Scheme.IgnoreNullOrWhiteSpace() : middlerOptions.DefaultScheme;

            if (!allowedSchemes.Any(scheme => Wildcard.Match(uri.Scheme, scheme)))
                return null;

            if (!Wildcard.Match(uri.Host, rule.Hostname))
                return null;


            var parsedTemplate = TemplateParser.Parse($"{rule.Path}");

            var defaults = parsedTemplate.Parameters.Where(p => p.DefaultValue != null)
                .Aggregate(new RouteValueDictionary(), (current, next) => {
                    current.Add(next.Name, next.DefaultValue);
                    return current;
                });

            var matcher = new TemplateMatcher(parsedTemplate, defaults);
            var router = context.GetRouteData().Routers.FirstOrDefault() ?? new RouteCollection();

            var rd = context.GetRouteData();
            if (matcher.TryMatch(context.Request.Path, rd.Values)) {
                var constraints = GetConstraints(context.RequestServices.GetRequiredService<IInlineConstraintResolver>(), parsedTemplate, null);
                if (RouteConstraintMatcher.Match(constraints, rd.Values, context, router, RouteDirection.IncomingRequest, ConstraintLogger)) {


                    return new MiddlerRuleMatch {
                        MiddlerRule = rule,
                        RouteData = GetRouteData(context, constraints),
                        AccessMode = rule.AccessAllowed(context) ?? middlerOptions.DefaultAccessMode
                    };

                }
            }

            return null;
        }


        private static IDictionary<string, IRouteConstraint> GetConstraints(IInlineConstraintResolver inlineConstraintResolver, RouteTemplate parsedTemplate, IDictionary<string, object> constraints) {

            var constraintBuilder = new RouteConstraintBuilder(inlineConstraintResolver, parsedTemplate.TemplateText);

            if (constraints != null) {
                foreach (var kvp in constraints) {
                    constraintBuilder.AddConstraint(kvp.Key, kvp.Value);
                }
            }

            foreach (var parameter in parsedTemplate.Parameters) {

                if (parameter.IsOptional) {
                    constraintBuilder.SetOptional(parameter.Name);
                }

                foreach (var inlineConstraint in parameter.InlineConstraints) {
                    constraintBuilder.AddResolvedConstraint(parameter.Name, inlineConstraint.Constraint);
                }

            }

            return constraintBuilder.Build();
        }

        private void EnsureLoggers(HttpContext context) {

            if (Logger == null) {
                var factory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                Logger = factory.CreateLogger(typeof(RouteBase).FullName);
                ConstraintLogger = factory.CreateLogger(typeof(RouteConstraintMatcher).FullName);
            }

        }

        internal Dictionary<string, object> GetRouteData(HttpContext context, IDictionary<string, IRouteConstraint> constraints) {
            var routeData = new Dictionary<string, object>();

            var uri = new Uri(context.Request.GetDisplayUrl());

            routeData["HOST"] = uri.Host;
            routeData["SCHEME"] = uri.Scheme;

            var rd = context.GetRouteData();

            foreach (var key in rd.Values.Keys) {
                var val = rd.Values[key]?.ToString();

                if (val == null) {
                    routeData.Add(key.ToLower(), null);

                    continue;
                }
                
                if (constraints.ContainsKey(key)) {

                    var constraint = constraints[key];
                    IRouteConstraint ic;
                    if (constraint is OptionalRouteConstraint optionalRouteConstraint) {
                        ic = optionalRouteConstraint.InnerConstraint;
                    } else {
                        ic = constraint;
                    }

                    object value;
                    if (ic is IntRouteConstraint) {
                        value = val.ToInt();
                    } else if (ic is BoolRouteConstraint) {
                        value = val.ToBoolean();
                    } else if (ic is DateTimeRouteConstraint) {
                        value = val.ToDateTime();
                    } else if (ic is DecimalRouteConstraint) {
                        value = val.ToDecimal();
                    } else if (ic is DoubleRouteConstraint) {
                        value = val.ToDouble();
                    } else if (ic is FloatRouteConstraint) {
                        value = val.ToFloat();
                    } else if (ic is GuidRouteConstraint) {
                        value = new Guid(val);
                    } else if (ic is LongRouteConstraint) {
                        value = val.ToLong();
                    } else {
                        value = val;
                    }

                    routeData.Add(key.ToLower(), value);
                } else {
                    routeData.Add(key.ToLower(), val);
                }

            }


            return routeData;
        }
        
    }
}
