using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Climax.Web.Http.Binders
{
    public class StringArrayModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var key = bindingContext.ModelName;
            var val = bindingContext.ValueProvider.GetValue(key);
            if (val != null)
            {
                var s = val.AttemptedValue;
                if (s != null && s.IndexOf(",", StringComparison.Ordinal) > 0)
                {
                    var stringArray = s.Split(new[] { "," }, StringSplitOptions.None);
                    bindingContext.Model = stringArray;
                }
                else
                {
                    bindingContext.Model = new[] { s };
                }

                return true;
            }

            return false;
        }
    }
}
