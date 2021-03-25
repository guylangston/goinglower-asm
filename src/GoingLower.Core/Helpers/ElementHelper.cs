using System;

namespace GoingLower.Core.Helpers
{
    public static class ElementHelper
    {
        public static bool TryFindByModelRecursive<TModel>(this IElement el, Func<TModel, bool> where, out IElement found)
        {
            foreach (var kid in el.ChildrenRecursive())
            {
                if (kid.Model is TModel model && where(model))
                {
                    found = kid;
                    return true;
                }
            }

            found = default;
            return false;
        }
    }
}