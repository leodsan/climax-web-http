using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Climax.Web.Http.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckModelStateAttribute : ActionFilterAttribute
    {
        private readonly ModelNullCheckType _checkType;
        private readonly Func<Dictionary<string, object>, ModelNullCheckType, bool> _validate;

        public CheckModelStateAttribute(ModelNullCheckType checkType = ModelNullCheckType.All)
            : this(checkType, (arguments, check) =>
            {
                switch (check)
                {
                    case ModelNullCheckType.All:
                        return arguments.All(i => i.Value == null);
                    case ModelNullCheckType.Any:
                        return arguments.ContainsValue(null);
                }

                return true;
            })
        {
        }

        protected CheckModelStateAttribute(ModelNullCheckType checkType,
            Func<Dictionary<string, object>, ModelNullCheckType, bool> checkCondition)
        {
            _checkType = checkType;
            _validate = checkCondition;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (_validate(actionContext.ActionArguments, _checkType))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Argument cannot be null");
            }

            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    actionContext.ModelState);
            }
        }
    }
}