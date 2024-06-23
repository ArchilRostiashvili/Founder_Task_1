using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using BebiLibs;

public static class Utils
{
    /////////////////////////////////////////////////////////// SORTED ///////////////////////////////////////////////////////////////////

    public static void Clear(this InputField input) => input.text = string.Empty;
    public static void Clear(this Text text) => text.text = string.Empty;
    public static void Clear(this TMP_Text text) => text.text = string.Empty;

    public static void SetAlpha(this SpriteRenderer rend, float alpha) =>
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);

    public static void SetAlpha(this Image image, float alpha) =>
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

    public static void SetAlpha(this TMP_Text text, float alpha) =>
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

    public static void SetAlpha(this SpriteRenderer rend, byte alpha) =>
        rend.color = new Color32((byte)(rend.color.r * 255f), (byte)(rend.color.g), (byte)(rend.color.b), alpha);

    public static int GetPercent(this int original, int percent) => (original * percent) / 100;
    public static float GetPercent(this float original, int percent) => (original * percent) / 100;

    public static Vector2 GetPercent(this Vector2 vector, int percent)
    {
        float x = vector.x;
        float y = vector.y;

        x = x * percent / 100;
        y = y * percent / 100;

        return new Vector2(x, y);
    }

    public static Vector3 GetPercent(this Vector3 vector, int percent)
    {
        float x = vector.x;
        float y = vector.y;
        float z = vector.z;

        x = x * percent / 100;
        y = y * percent / 100;
        z = z * percent / 100;

        return new Vector3(x, y, z);
    }

    public static T GetRandomUniqueFromLists<T>(List<T> arrayPool, List<T> arrayExclude)
    {
        if (arrayExclude.Count == 0) return arrayPool.GetRandomElement();
        else return arrayPool.FindAll(x => !arrayExclude.Contains(x)).GetRandomElement();
    }

    public static T GetRandomElement<T>(this IEnumerable<T> container) => container.ElementAt(Random.Range(0, container.Count()));

    public static T GetRandomElementAndRemove<T>(this List<T> container)
    {
        int ind = Random.Range(0, container.Count);
        T obj = container[ind];

        container.RemoveAt(ind);

        return obj;
    }

    public static void DOKillChildren(this Transform transform)
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null)
            {
                children[i].DOKill();
            }
        }
    }

    public static Vector3 GetRandomVector() => new Vector3(Random.value * 360, Random.value * 360, Random.value * 360);
    public static Vector3 GetRandomVector(float min, float max)
        => new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));

    public static Vector3 GetRandomVectorInRange(Vector3 minValue, Vector3 maxValue)
    {
        return new Vector3(Random.Range(minValue.x, maxValue.x), Random.Range(minValue.y, maxValue.y), Random.Range(minValue.z, maxValue.z));
    }


    public static void Shuffle<T>(ref List<T> list)
    {
        System.Random _rng = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Shuffle<T>(ref T[] array)
    {
        System.Random _rng = new System.Random();

        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }

    public static List<T> DistinctSubtractArrays<T>
    (List<T> baseList, List<T> listToSubstract)
    {
        List<T> res = new List<T>();

        baseList.ForEach(elem =>
        {
            if (!listToSubstract.Contains(elem))
            {
                res.Add(elem);
            }
        });

        return res;
    }

    public static List<T> DistinctSubtractArrays<T>
        (IEnumerable<T> baseList, IEnumerable<T> listToSubstract)
    {
        List<T> res = new List<T>();

        baseList.MyForeach(elem =>
        {
            if (!listToSubstract.Contains(elem))
            {
                res.Add(elem);
            }
        });

        return res;
    }

    public static void Disable(this GameObject gameObject) => gameObject.SetActive(false);
    public static void Enable(this GameObject gameObject) => gameObject.SetActive(true);

    public static bool GetRandomBoolean() => Random.Range(0, 2) == 0;
    public static bool GetRandomBoolean(int percent) => Random.Range(0, 101) < percent;

    public static void Foreach(this Transform transform, System.Action<Transform> action)
    {
        foreach (Transform child in transform)
        {
            action(child);
        }
    }

    public static Vector2 SetX(this Vector2 vector, float x) => new Vector2(x, vector.y);
    public static Vector2 SetY(this Vector2 vector, float y) => new Vector2(vector.x, y);

    public static Vector2 AddX(this Vector2 vector, float x) => new Vector2(vector.x + x, vector.y);
    public static Vector2 AddY(this Vector2 vector, float y) => new Vector2(vector.x, vector.y + y);

    public static Vector2 GetClosestVector2From(this Vector2 vector, Vector2[] otherVectors)
    {
        if (otherVectors.Length == 0) Debug.LogWarning("The list of other vectors is empty");
        float minDistance = Vector2.Distance(vector, otherVectors[0]);
        Vector2 minVector = otherVectors[0];
        for (int i = otherVectors.Length - 1; i > 0; i--)
        {
            float newDistance = Vector2.Distance(vector, otherVectors[i]);
            if (newDistance < minDistance)
            {
                minDistance = newDistance;
                minVector = otherVectors[i];
            }
        }
        return minVector;
    }

    public static Vector2 GetClosestVector2From(this Vector2 vector, List<Vector2> otherVectors)
    {
        if (otherVectors.Count == 0) Debug.LogWarning("The list of other vectors is empty");
        float minDistance = Vector2.Distance(vector, otherVectors[0]);
        Vector2 minVector = otherVectors[0];
        for (int i = otherVectors.Count - 1; i > 0; i--)
        {
            float newDistance = Vector2.Distance(vector, otherVectors[i]);
            if (newDistance < minDistance)
            {
                minDistance = newDistance;
                minVector = otherVectors[i];
            }
        }
        return minVector;
    }

    public static void DestroyChildren(this Transform transform)
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public static void ResetTransformation(
        this Transform transform, bool resetPosition = true,
        bool resetRotation = true, bool resetScale = true)
    {
        if (resetPosition) transform.position = Vector3.zero;
        if (resetRotation) transform.localRotation = Quaternion.identity;
        if (resetScale) transform.localScale = Vector3.one;
    }

    public static void ResetChildTransformations(this Transform transform, bool isRecursive = false, bool resetPosition = true,
        bool resetRotation = true, bool resetScale = true)
    {
        foreach (Transform child in transform)
        {
            child.ResetTransformation(resetPosition, resetRotation, resetScale);

            if (isRecursive) child.ResetChildTransformations(isRecursive, resetPosition, resetRotation, resetScale);
        }
    }

    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return (layerMask == (layerMask | (1 << layer)));
    }

    /////////////////////////////////////////////////////////// NOT SORTED ///////////////////////////////////////////////////////////////////

    public static int RandomDirection => Random.Range(0, 2) == 0 ? 1 : -1;
    public static string NormalDeltaFps => $"normal fps: {1f / Time.deltaTime}";
    public static string FixedDeltaFps => $"fixed fps: {1f / Time.fixedDeltaTime}";

    public static void Add<T1, T2>(this ICollection<KeyValuePair<T1, T2>> target, T1 item1, T2 item2)
    {
        if (target == null)
            throw new System.ArgumentNullException(nameof(target));

        target.Add(new KeyValuePair<T1, T2>(item1, item2));
    }


    static Vector3[] _CornersArray = new Vector3[4];
    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        rectTransform.GetWorldCorners(_CornersArray);
        Vector3 position = _CornersArray[0];
        Vector2 size = _CornersArray[2] - _CornersArray[0];
        return new Rect(position, size);
    }

    /// <summary>
    /// This must be called in OnGui unity method
    /// </summary>
    public static void DisplayFps()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect2 = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(0f, 1f, 0.0f, 1.0f);
        GUI.Label(rect2, NormalDeltaFps, style);
    }

    public static Bounds GetBoundsWithCollider(Collider2D obj) => obj.bounds;
    public static Bounds GetBoundsWithCollider(Collision2D obj) => obj.collider.bounds;
    public static Bounds GetBoundsWithRenderer(Renderer obj) => obj.bounds;
    public static Bounds GetBounds(this Renderer obj) => obj.bounds;

    /// <summary>
    /// My approach
    /// </summary>
    public static Vector2 ScreenBorders2D => new Vector2(
       Camera.main.orthographicSize * Screen.width / Screen.height,
        Camera.main.orthographicSize
    );

    /// <summary>
    /// Tedos approach
    /// </summary>
    public static Bounds ScreenBounds2D => new Bounds(Vector2.zero, 2 * Camera.main.ViewportToWorldPoint(Vector2.one));

    public static Bounds ScreenBounds2DSafe
    {
        get
        {
            Camera camera = Camera.main;
            Vector3 safeOffset = camera.ScreenToWorldPoint(Screen.safeArea.center);
            Bounds bounds = new Bounds(safeOffset, 2 * camera.ScreenToWorldPoint(Screen.safeArea.max));
            return bounds;
        }
    }

    public static void UnloadAssets<T>(IEnumerable<T> array) where T : UnityEngine.Object
    {
        if (array == null) return;
        foreach (var item in array)
        {
            if (item != null)
            {
                Resources.UnloadAsset(item);
            }
        }

    }

    public static Vector2 GetRandomPoint(this Bounds bounds) => new Vector2(
        Random.Range(bounds.min.x, bounds.max.x),
        Random.Range(bounds.min.y, bounds.max.y)
    );

    public static float Remap(this float value, float from1, float to1)
    {
        return (value - from1) / (to1 - from1);
    }

    public static Sprite ToSprite(this Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100);
    }


    public static bool HasComponent<T>(this GameObject obj) where T : Component => obj.GetComponent<T>() != null;
    public static bool HasComponent<T>(this Transform obj) where T : Component => obj.GetComponent<T>() != null;

    public static bool IsEven(this int i) => i % 2 == 0;

    public static void ShakePositionXY(this Transform tr, float maxX, float maxY, float time, bool looping)
    {
        Vector3 savedPosition = tr.localPosition;

        tr.DOLocalMove(
            new Vector3(savedPosition.x + Random.Range(0, maxX), savedPosition.y + Random.Range(0, maxY), savedPosition.z),
            time / 2).OnComplete(() =>
              {
                  tr.DOLocalMove(savedPosition, time / 2).OnComplete(() =>
                  {
                      tr.localPosition = savedPosition;
                      if (looping) tr.ShakePositionXY(maxX, maxY, time, looping);
                  });
              });
    }

    public static void ShakeRotationXY(this Transform tr, float maxX, float maxY, float time, bool looping)
    {
        Vector3 savedRotation = tr.localRotation.eulerAngles;

        tr.DOLocalRotate(
            new Vector3(savedRotation.x + Random.Range(0, maxX), savedRotation.y + Random.Range(0, maxY), savedRotation.z),
            time / 2).OnComplete(() =>
            {
                tr.DOLocalRotate(savedRotation, time / 2).OnComplete(() =>
                {
                    tr.localRotation = Quaternion.Euler(savedRotation);
                    if (looping) tr.ShakeRotationXY(maxX, maxY, time, looping);
                });
            });
    }

    public static void MyForeach<T>(this IEnumerable<T> myContainer, System.Action<T> action)
    {
        foreach (T item in myContainer)
        {
            action(item);
        }
    }

    public static void MyForeach<T>(this IEnumerable<T> myContainer, System.Action<T> action, bool @break)
    {
        foreach (T item in myContainer)
        {
            action(item);
            if (@break)
                break;
        }
    }

    /// <summary>
    /// return children array from start(inclusive) to end(exclusive)
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static IEnumerable<Transform> GetChildrenInRange(this Transform parent, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            yield return parent.GetChild(i);
        }
    }

    public static IEnumerable<Transform> GetAllChild(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            yield return child;
        }
    }

    public static int Mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }


    public static void RestartCurrentScene() =>
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );

    public static T GetComponent<T>(this RaycastHit2D hit) => hit.collider.GetComponent<T>();
    public static GameObject SelectedUiObject => EventSystem.current.currentSelectedGameObject;
    public static T GetComponentFromSelectedUiElement<T>() where T : Component => SelectedUiObject.GetComponent<T>();

    public static RaycastHit2D HitFromMouseClick
    {
        get
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            return Physics2D.Raycast(mousePos2D, Vector2.zero);
        }
    }

    public static int GetRandomInt(int precision = 10) => (int)(Random.value * precision);

    public static bool IsEmpty(this string s) => s.Length <= 0;
    public static bool IsNumber(this string s) => int.TryParse(s, out _);

    public static void Add<T, E>(this Dictionary<T, E> dict, KeyValuePair<T, E> pair)
        => dict.Add(pair.Key, pair.Value);
    public static string ToString<T, E>(this KeyValuePair<T, E> kvp)
        => $"key: {kvp.Key}, value: {kvp.Value}";

    //this is madness
    public static System.Func<IEnumerator, Coroutine> StartCoroutine
        => Object.FindObjectOfType<MonoBehaviour>().StartCoroutine;

    public static Vector3[] GetQuadraticBezierPoints(Vector3 startpoint, Vector3 endPoint, float curveHeigh)
    {
        Vector3 heighPoint = startpoint + (endPoint - startpoint) / 2 + Vector3.up * curveHeigh;

        Vector3[] res = new Vector3[100];
        int maxT = 1;
        int index = 0;

        for (float t = 0; t <= maxT; t += 0.01f)
        {
            Vector3 newPoint = (Mathf.Pow(1 - t, 2) * startpoint) + (2 * (1 - t) * t * heighPoint) + (t * t * endPoint);
            try
            {
                res[index++] = newPoint;
            }
            catch
            {
                break;
            }
        }
        return res;
    }

    public static T[] GetMergedArray<T>(T[] arr1, T[] arr2)
    {
        T[] newArray = new T[arr1.Length + arr2.Length];
        System.Array.Copy(arr1, newArray, arr1.Length);
        System.Array.Copy(arr2, 0, newArray, arr1.Length, arr2.Length);

        return newArray;
    }



    public static float GetShapeArea(this MeshFilter meshFilter)
    {
        Vector3[] mVertices = meshFilter.mesh.vertices;
        float result = 0;
        for (int p = mVertices.Length - 1, q = 0; q < mVertices.Length; p = q++)
        {
            result += (Vector3.Cross(mVertices[q], mVertices[p])).magnitude;
        }
        return result * 0.5f;

    }

    public static List<T> GetElementsDistinctRandomly<T>(this List<T> targetList, int count)
    {
        List<T> res = new List<T>();

        while (true)
        {
            if (res.Count == count)
                return res;

            T elem = targetList.GetRandomElement();
            if (!res.Contains(elem))
            {
                res.Add(elem);
            }

        }
    }

    public static GameObject Find(string search)
    {
        var scene = SceneManager.GetActiveScene();
        var sceneRoots = scene.GetRootGameObjects();
        GameObject result = null;
        foreach (var root in sceneRoots)
        {
            if (root.name.Equals(search)) return root;
            result = FindRecursive(root, search);
            if (result) break;
        }
        return result;
    }

    private static GameObject FindRecursive(GameObject obj, string search)
    {
        GameObject result = null;
        foreach (Transform child in obj.transform)
        {
            if (child.name.Equals(search))
            {
                return child.gameObject;
            }
            result = FindRecursive(child.gameObject, search);
            if (result) break;
        }
        return result;
    }

    public static void LookToward(this Transform objToRotate, Transform targetObj)
    {
        objToRotate.rotation = Quaternion.FromToRotation(Vector3.up, targetObj.position - objToRotate.position);
    }

    public static MobileDeviceType GetDeviceType()
    {
        int width = Screen.width;
        int height = Screen.height;
        float aspectRatio = Mathf.Max(width, height) / Mathf.Min(width, height);
        bool isTablet = DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f;
        //bool isTablet = DeviceDiagonalSizeInInches() > 6.5f;
        if (isTablet)
        {
            return MobileDeviceType.TABLET;
        }
        else
        {
            return MobileDeviceType.PHONE;
        }
    }

    private static float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
        return diagonalInches;
    }
}

