using System;
using System.Collections;

namespace MustHave.Utilities
{
    public static class CoroutineExtensionMethods
    {
        public static void MoveThrough(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                if (current is IEnumerator)
                {
                    (current as IEnumerator).MoveThrough();
                }
            }
        }
    }
}
