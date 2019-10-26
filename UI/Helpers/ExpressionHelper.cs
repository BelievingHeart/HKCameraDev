using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UI.Helpers
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// Gets the property that the expression is wrapping
        /// </summary>
        /// <param name="expression">The expression to compile and invoke</param>
        /// <typeparam name="T">The type of the wrapped property</typeparam>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this Expression<Func<T>> expression)
        {
            return expression.Compile().Invoke();
        }

        public static void SetPropertyValue<T>(this Expression<Func<T>> expression, T value)
        {
            // Convert ()=> owner.Property to owner.Property
            var ownerDotProperty = (MemberExpression) expression.Body;
            // Get the property
            var propertyInfo = (PropertyInfo) ownerDotProperty.Member;
            // Get the owner of the property
            var owner = Expression.Lambda(ownerDotProperty.Expression).Compile().DynamicInvoke();
            // Set the property value of the owner
            propertyInfo.SetValue(owner, value);
        }
    }
}