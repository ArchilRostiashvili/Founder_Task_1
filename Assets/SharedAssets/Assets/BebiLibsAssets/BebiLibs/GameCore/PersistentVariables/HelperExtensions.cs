using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public static class HelperExtensions
    {
        public static void Add<T1, T2>(this ICollection<RandomValue<T1>> target, T1 item1, float item2)
        {
            if (target == null)
                throw new System.ArgumentNullException(nameof(target));

            target.Add(new RandomValue<T1>(item1, item2));
        }

    }
}
