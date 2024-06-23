using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomUtils : MonoBehaviour
{
    public static T GetRandomFrom<T>(IList<T> probableElementCollection) where T : IProbable
    {
        return GetRandomFrom(probableElementCollection, out int _);
    }

    public static T GetRandomFrom<T>(IList<T> probableElementCollection, out int index) where T : IProbable
    {
        float total = 0;
        foreach (var elem in probableElementCollection)
        {
            total += elem.Probability;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < probableElementCollection.Count; i++)
        {
            T item = probableElementCollection[i];
            if (randomPoint < item.Probability)
            {
                index = i;
                return item;
            }
            else
            {
                randomPoint -= item.Probability;
            }
        }

        index = probableElementCollection.Count - 1;
        return probableElementCollection.Last();
    }
}

public interface IProbable
{
    float Probability { get; }
};
