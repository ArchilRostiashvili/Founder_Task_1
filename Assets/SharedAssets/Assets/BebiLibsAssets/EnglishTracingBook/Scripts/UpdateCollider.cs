using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using IndieStudio;
using IndieStudio.EnglishTracingBook;
using IndieStudio.EnglishTracingBook.Game;
using IndieStudio.EnglishTracingBook.Utility;
#if UNITY_EDITOR
namespace GameSmartTool
{
    using UnityEngine;
    using UnityEditor;
    using IndieStudio.EnglishTracingBook.Game;
    using System.Linq;

    public class UpdateCollider : EditorWindow
    {

        [MenuItem("Modules/ABC_Old/UpdateCollider")]
        private static void ShowWindow()
        {
            var window = GetWindow<UpdateCollider>();
            window.titleContent = new GUIContent("UpdateCollider");
            window.Show();
        }

        public static float Tolerance = 1f;
        public static float Offset = 0.3f;
        private static readonly List<List<Vector2>> _arrayOriginalPaths = new List<List<Vector2>>();
        private void OnGUI()
        {
            DrawUI();
        }


        public static void DrawUI()
        {
            Tolerance = EditorGUILayout.FloatField("Tolerance", Tolerance);
            Offset = EditorGUILayout.FloatField("Offset", Offset);

            if (GUILayout.Button("Update Collider"))
            {
                Find();
            }

            if (GUILayout.Button("Replace By Prefab"))
            {
                FindPrefubs();
            }

        }

        private static void FindPrefubs()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                var gameObject = Selection.gameObjects[i];
                Undo.RecordObject(gameObject, "Modify Collider");
                CompoundShape shape = gameObject.GetComponent<CompoundShape>();
                if (shape != null)
                {
                    ReplaceElements(shape);
                }

                EditorUtility.SetDirty(gameObject);
            }


            void ReplaceElements(CompoundShape chape)
            {
                List<Shape> shapes = chape.shapes;
                for (int i = 0; i < shapes.Count; i++)
                {
                    string path = "Assets/English Tracing Book/Prefabs/Uppercase/" + shapes[i].name + ".prefab";
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    Transform content = shapes[i].transform.GetChild(0);
                    if (obj != null)
                    {
                        GameObject newobj = PrefabUtility.InstantiatePrefab(obj, chape.transform) as GameObject;
                        Shape s = UpdatePositions(newobj, shapes[i].transform.position, content.localScale);
                        GameObject.DestroyImmediate(shapes[i].gameObject, true);
                        shapes[i] = s;
                    }
                }
            }

            Shape UpdatePositions(GameObject spawnObject, Vector2 position, Vector3 scale)
            {
                Shape shape = spawnObject.GetComponent<Shape>();
                shape.transform.position = position;
                Transform content = shape.transform.GetChild(0);
                content.transform.localScale = scale;
                return shape;
            }
        }

        private static void Find()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                var gameObject = Selection.gameObjects[i];
                Undo.RecordObject(gameObject, "Modify Collider");
                ModifyCollider(gameObject);
                EditorUtility.SetDirty(gameObject);
            }

        }

        public static PolygonCollider2D ModifyCollider(GameObject gameObject)
        {
            Shape shape = gameObject.GetComponent<Shape>();
            Transform content = shape.transform.GetChild(0);
            Image image = content.GetComponent<Image>();
            Sprite elementSprite = image.sprite;

            PolygonCollider2D oldCollider = gameObject.GetComponentInChildren<PolygonCollider2D>();

            oldCollider.pathCount = 0;
            if (oldCollider != null)
            {
                oldCollider.transform.localScale = Vector3.one;
                List<Vector2> arrayPath = new List<Vector2>(500);
                oldCollider.pathCount = elementSprite.GetPhysicsShapeCount();
                for (int j = 0; j < elementSprite.GetPhysicsShapeCount(); j++)
                {
                    int count = elementSprite.GetPhysicsShape(j, arrayPath);

                    List<Vector2> newArrayPath = new List<Vector2>(count);
                    for (int i = 0; i < count; i++)
                    {
                        newArrayPath.Add(arrayPath[i]);
                    }
                    oldCollider.SetPath(j, newArrayPath);
                }

                for (int i = 0; i < oldCollider.pathCount; i++)
                {
                    Vector2[] data = oldCollider.GetPath(i);
                    data = data.Reverse().ToArray();
                    List<Vector2> normals = VertexProcessor.GetVertexNormals(data);
                    for (int j = 0; j < data.Length; j++)
                    {
                        data[j] = data[j] * 100;
                        data[j] = data[j] + normals[j] * Offset;
                    }
                    oldCollider.SetPath(i, data);
                }

                _arrayOriginalPaths.Clear();
                for (int j = 0; j < oldCollider.pathCount; j++)
                {
                    List<Vector2> arrayColPath = new List<Vector2>(oldCollider.GetPath(j));
                    _arrayOriginalPaths.Add(arrayColPath);
                }

                for (int j = 0; j < _arrayOriginalPaths.Count; j++)
                {
                    List<Vector2> arrayPathNew = _arrayOriginalPaths[j];
                    arrayPathNew = ShapeOptimizationHelper.arrayDouglasPeuckerReduction(arrayPathNew, Tolerance);
                    oldCollider.SetPath(j, arrayPathNew.ToArray());
                }

                for (int i = 0; i < oldCollider.pathCount; i++)
                {
                    Vector2[] data = oldCollider.GetPath(i);
                    oldCollider.SetPath(i, data);
                }

                return oldCollider;
            }
            return null;
        }
    }

    public static class VertexProcessor
    {
        public static Vector2 UnitDirectionBetween(Vector2 from, Vector2 to)
        {
            return (to - from).normalized;
        }

        public static Vector2 UnitAverageVector(Vector2 vec1, Vector2 vec2)
        {
            return (vec1 + vec2).normalized;
        }

        // public static List<Vector2> RemoveOverlapingPoints(Vector2[] vertexes)
        // {
        //     int n = vertexes.Length;
        //     for(int i = 0; i < n; i++)
        //     {
        //         int j = (i + 1) % n;


        //     }
        // }



        public static List<Vector2> GetVertexNormals(Vector2[] vertexes)
        {
            int n = vertexes.Length;
            List<Vector2> weightedNormal = new List<Vector2>(n);
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                Vector2 edge_vec = vertexes[j] - vertexes[i];
                weightedNormal.Add(new Vector2(-edge_vec.y, edge_vec.x));
            }
            List<Vector2> vertexNormals = new List<Vector2>(n);

            for (int i = 0; i < n; i++)
            {
                int j = (n + i - 1) % n;
                vertexNormals.Add((weightedNormal[i] + weightedNormal[j]).normalized);
            }
            return vertexNormals;
        }

        public static bool AreLinesIntersecting(Vector2 l1_p1, Vector2 l1_p2, Vector2 l2_p1, Vector2 l2_p2, bool shouldIncludeEndPoints)
        {
            //To avoid floating point precision issues we can add a small value
            float epsilon = 0.00001f;

            bool isIntersecting = false;

            float denominator = (l2_p2.y - l2_p1.y) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.y - l1_p1.y);

            //Make sure the denominator is > 0, if not the lines are parallel
            if (denominator != 0f)
            {
                float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.y - l2_p1.y) - (l2_p2.y - l2_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;
                float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.y - l2_p1.y) - (l1_p2.y - l1_p1.y) * (l1_p1.x - l2_p1.x)) / denominator;

                //Are the line segments intersecting if the end points are the same
                if (shouldIncludeEndPoints)
                {
                    //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                    if (u_a >= 0f + epsilon && u_a <= 1f - epsilon && u_b >= 0f + epsilon && u_b <= 1f - epsilon)
                    {
                        isIntersecting = true;
                    }
                }
                else
                {
                    //Is intersecting if u_a and u_b are between 0 and 1
                    if (u_a > 0f + epsilon && u_a < 1f - epsilon && u_b > 0f + epsilon && u_b < 1f - epsilon)
                    {
                        isIntersecting = true;
                    }
                }
            }

            return isIntersecting;
        }

    }


    public static class EdgeHelpers
    {
        public struct Edge
        {
            public int v1;
            public int v2;
            public int triangleIndex;
            public Edge(int aV1, int aV2, int aIndex)
            {
                v1 = aV1;
                v2 = aV2;
                triangleIndex = aIndex;
            }
        }

        public static List<Edge> GetEdges(int[] aIndices)
        {
            List<Edge> result = new List<Edge>();
            for (int i = 0; i < aIndices.Length; i += 3)
            {
                int v1 = aIndices[i];
                int v2 = aIndices[i + 1];
                int v3 = aIndices[i + 2];
                result.Add(new Edge(v1, v2, i));
                result.Add(new Edge(v2, v3, i));
                result.Add(new Edge(v3, v1, i));
            }
            return result;
        }

        public static List<Edge> FindBoundary(this List<Edge> aEdges)
        {
            List<Edge> result = new List<Edge>(aEdges);
            for (int i = result.Count - 1; i > 0; i--)
            {
                for (int n = i - 1; n >= 0; n--)
                {
                    if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                    {
                        // shared edge so remove both
                        result.RemoveAt(i);
                        result.RemoveAt(n);
                        i--;
                        break;
                    }
                }
            }
            return result;
        }
        public static List<Edge> SortEdges(this List<Edge> aEdges)
        {
            List<Edge> result = new List<Edge>(aEdges);
            for (int i = 0; i < result.Count - 2; i++)
            {
                Edge E = result[i];
                for (int n = i + 1; n < result.Count; n++)
                {
                    Edge a = result[n];
                    if (E.v2 == a.v1)
                    {
                        // in this case they are already in order so just continoue with the next one
                        if (n == i + 1)
                            break;
                        // if we found a match, swap them with the next one after "i"
                        result[n] = result[i + 1];
                        result[i + 1] = a;
                        break;
                    }
                }
            }
            return result;
        }
    }
}

#endif