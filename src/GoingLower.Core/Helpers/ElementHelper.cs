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
        
        
        public static bool TryFind<TElement, TModel>(this IElement el, Func<TElement, TModel, bool> where,  out TElement mElement, out TModel mModel)
        {
            foreach (var kid in el.ChildrenRecursive())
            {
                if (kid is TElement te && kid.Model is TModel tm && where(te, tm))
                {
                    mElement = te;
                    mModel   = tm;
                    return true;
                }
            }

            mElement = default;
            mModel   = default;
            return false;
        }
    }
}