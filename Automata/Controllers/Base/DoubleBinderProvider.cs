﻿
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automata.Controllers.Base
{
    public class DoubleBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(double?))
            {
                return new DoubleModelBinder();
            }

            if (context.Metadata.ModelType == typeof(double))
            {
                return new DoubleModelBinder();
            }

            return null;
        }
    }
}
