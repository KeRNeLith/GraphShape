using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphSharp.Algorithms.Layout;

namespace GraphSharp.Algorithms
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
