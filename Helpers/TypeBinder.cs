using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Movies_API.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        // Assit IMapper to bind as we're using FromForm
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(propertyName);

            if(value == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            else
            {
                try
                {
                    var deserializedValue = JsonConvert.DeserializeObject<T>(value.FirstValue);
                    bindingContext.Result = ModelBindingResult.Success(deserializedValue);
                }
                catch(Exception exc)
                {
                    bindingContext.ModelState.AddModelError(propertyName, "The given value is not of the correct type");
                }

                return Task.CompletedTask;
                
            }
        }
    }
}

