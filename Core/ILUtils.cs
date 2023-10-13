using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MorePortableStorages.Core; 

public static class ILUtils {
    public static MethodInfo GetMethod(LambdaExpression expression) {
        if (expression.Body is MethodCallExpression meth) {
            return meth.Method;
        }

        throw new InvalidOperationException($"Body of {nameof(expression)} should be a method call");
    }
}