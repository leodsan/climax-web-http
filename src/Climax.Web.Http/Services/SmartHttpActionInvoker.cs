using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Climax.Web.Http.Services
{
    public class SmartHttpActionInvoker : IHttpActionInvoker
    {
        public Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return InvokeActionInternal(actionContext, cancellationToken);
        }

        private static async Task<HttpResponseMessage> InvokeActionInternal(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext.ActionDescriptor == null)
            {
                throw new ArgumentNullException("actionContext.ActionDescriptor");
            }

            if (actionContext.ControllerContext == null)
            {
                throw new ArgumentNullException("actionContext.ControllerContext");
            }

            try
            {
                var result = await actionContext.ActionDescriptor.ExecuteAsync(actionContext.ControllerContext, actionContext.ActionArguments, cancellationToken);

                if (result != null)
                {
                    actionContext.Request.Properties[Constants.RuntimeReturnType] = result.GetType();
                }

                var isActionResult = typeof(IHttpActionResult).IsAssignableFrom(actionContext.ActionDescriptor.ReturnType);

                if (result == null && isActionResult)
                {
                    throw new InvalidOperationException();
                }

                if (isActionResult || actionContext.ActionDescriptor.ReturnType == typeof(object))
                {
                    var actionResult = result as IHttpActionResult;

                    if (actionResult == null && isActionResult)
                    {
                        throw new InvalidOperationException();
                    }

                    if (actionResult == null)
                        return actionContext.ActionDescriptor.ResultConverter.Convert(actionContext.ControllerContext, result);

                    var response = await actionResult.ExecuteAsync(cancellationToken);
                    if (response == null)
                    {
                        throw new InvalidOperationException();
                    }

                    if (response.RequestMessage == null)
                    {
                        response.RequestMessage = actionContext.Request;
                    }

                    return response;
                }

                return actionContext.ActionDescriptor.ResultConverter.Convert(actionContext.ControllerContext, result);
            }
            catch (HttpResponseException httpResponseException)
            {
                var response = httpResponseException.Response;
                if (response.RequestMessage == null)
                {
                    response.RequestMessage = actionContext.Request;
                }

                return response;
            }
        }
    }
}