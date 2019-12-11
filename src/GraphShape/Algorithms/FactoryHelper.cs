using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphShape.Algorithms.Layout;

namespace GraphShape.Algorithms
{
    public static class FactoryHelper
    {
        public static TParam CreateNewParameter<TParam>(this ILayoutParameters oldParameters)
            where TParam : class,ILayoutParameters,new()
        {
            return !(oldParameters is TParam) 
                ? new TParam() : 
                (TParam)(oldParameters as TParam).Clone();
        }
    }
}
