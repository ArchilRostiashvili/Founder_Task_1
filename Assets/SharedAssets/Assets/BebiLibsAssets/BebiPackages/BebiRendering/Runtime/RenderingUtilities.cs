using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//TODO: rename to Rendering Utilities
public static class GameUtility
{
    private static Mesh _fullscreenMesh;

    public static GameObject CreateGameobject(string name = "render quad", Transform parent = null)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = parent;
        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        meshRenderer.lightProbeUsage = LightProbeUsage.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        return obj;
    }

    public static Rect ScaleRect(Rect rect, Vector2 scale)
    {
        rect.Set(rect.x * scale.x, rect.y * scale.y, rect.width * scale.x, rect.height * scale.y);
        return rect;
    }


    public static void CopyRectTransformData(RectTransform src, RectTransform dest)
    {
        dest.anchorMin = src.anchorMin;
        dest.anchorMax = src.anchorMax;
        dest.anchoredPosition = src.anchoredPosition;
        dest.sizeDelta = src.sizeDelta;
    }

    private static Texture2D _whiteTexture;
    public static Texture2D whiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
                return _whiteTexture;
            }
            return _whiteTexture;
        }
    }

    private static Texture2D _blackTexture;
    public static Texture2D blackTexture
    {
        get
        {
            if (_blackTexture == null)
            {
                _blackTexture = new Texture2D(1, 1);
                _blackTexture.SetPixel(0, 0, Color.black);
                _blackTexture.Apply();
                return _blackTexture;
            }
            return _blackTexture;
        }
    }

    private static Texture2D _clearTexture;
    public static Texture2D clearTexture
    {
        get
        {
            if (_clearTexture == null)
            {
                _clearTexture = new Texture2D(1, 1);
                _clearTexture.SetPixel(0, 0, Color.clear);
                _clearTexture.Apply();
                return _clearTexture;
            }
            return _clearTexture;
        }
    }

    public static Vector2 GetRectWorldSize(RectTransform rt)
    {
        Vector3[] c = new Vector3[4];
        rt.GetWorldCorners(c);
        return new Vector2(Mathf.Abs(c[0].x - c[3].x), Mathf.Abs(c[0].y - c[1].y));
    }


    public static void AddMask(this Camera camera, LayerMask mask)
    {
        camera.cullingMask |= mask;
    }

    public static void RemoveMask(this Camera camera, LayerMask mask)
    {
        camera.cullingMask &= ~mask;
    }

    public static void SetLayer(this GameObject obj, LayerMask mask)
    {
        obj.layer = (int)(Mathf.Log(mask, 2));
    }

    public static Renderer GetRenderer(this GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) return renderer;
        else
        {
            Debug.Log("Renderer Not Attached To a Gameobject: " + obj.name);
            return null;
        }
    }

    public static MeshRenderer GetElementQuad(Bounds bounds, GameObject obj)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        Vector2 min = bounds.min;
        Vector2 max = bounds.max;
        Vector3[] vertices = new Vector3[4]
        {
                min, new Vector3(max.x, min.y), new Vector3(min.x, max.y), max
        };

        int[] tries = new int[6]
        {
                0, 2, 1, 2, 3, 1
        };

        Vector2[] uv = new Vector2[4]
        {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = tries;
        mesh.uv = uv;
        meshFilter.mesh = mesh;
        return obj.GetComponent<MeshRenderer>();
    }

    public static void FindMinMax(Vector2[] uvs, out Vector2 min, out Vector2 max)
    {
        float minX = 1;
        float minY = 1;
        float maxX = 0;
        float maxY = 0;

        for (int i = 0; i < uvs.Length; i++)
        {
            Vector2 uv = uvs[i];
            minX = uv.x < minX ? uv.x : minX;
            minY = uv.y < minY ? uv.y : minY;
            maxX = uv.x > maxX ? uv.x : maxX;
            maxY = uv.y > maxY ? uv.y : maxY;
        }

        min = new Vector2(minX, minY);
        max = new Vector2(maxX, maxY);
    }

    private static Mesh _simpleMesh;
    private static int[] tries = new int[6]
       {
                0, 2, 1,
                2, 3, 1
       };

    private static Vector2[] uv = new Vector2[4]
    {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
    };

    private static Vector3[] vertices = new Vector3[4]
    {
            new Vector2(-1,  -1),
            new Vector2( 1, -1),
            new Vector2(-1,  1),
            new Vector2( 1,  1),
    };

    public static Mesh GenerateMesh(float size, bool newMesh = false)
    {
        if (_simpleMesh == null)
        {
            _simpleMesh = new Mesh();
        }

        vertices[0].x = -size;
        vertices[0].y = -size;

        vertices[1].x = size;
        vertices[1].y = -size;

        vertices[2].x = -size;
        vertices[2].y = size;

        vertices[3].x = size;
        vertices[3].y = size;

        if (newMesh)
        {
            Mesh m = new Mesh();
            m.vertices = vertices;
            m.triangles = tries;
            m.uv = uv;
            return m;
        }
        else
        {
            _simpleMesh.vertices = vertices;
            _simpleMesh.triangles = tries;
            _simpleMesh.uv = uv;
            return _simpleMesh;
        }

    }


    private static Vector2 GetViewPort(Vector2 vec, Camera camera)
    {
        return camera.WorldToViewportPoint(vec);
    }

    private static Vector2 GetScreenPoints(Vector2 vec, Camera camera)
    {
        return camera.WorldToScreenPoint(vec);
    }

    public static void Shuffle<T>(this System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public static Rect GetRectFromBound(Bounds bounds, Camera camera)
    {
        Vector2 min = GetScreenPoints(bounds.min, camera);
        Vector2 size = GetScreenPoints(bounds.max, camera) - min;
        Rect r = new Rect(min, size);
        return r;
    }


    public static Vector2 ScreenSpaceCords(Vector2 pos, Camera camera)
    {
        return camera.ViewportToWorldPoint(pos);
    }


    public static Mesh ScreenSpaceMesh(Camera camera)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4]
        {
                    ScreenSpaceCords(new Vector2(0, 0), camera),
                    ScreenSpaceCords(new Vector2(1, 0), camera),
                    ScreenSpaceCords(new Vector2(0, 1), camera),
                    ScreenSpaceCords(new Vector2(1, 1), camera),
        };
        mesh.vertices = vertices;

        int[] tries = new int[6]
        {
                0, 2, 1,
                2, 3, 1
        };
        mesh.triangles = tries;


        Vector2[] uv = new Vector2[4]
        {
                  new Vector2(0, 0),
                  new Vector2(1, 0),
                  new Vector2(0, 1),
                  new Vector2(1, 1)
        };
        mesh.uv = uv;
        return mesh;
    }



    public static Mesh fullscreenMesh
    {
        get
        {
            if (_fullscreenMesh != null)
                return _fullscreenMesh;

            float topV = 1.0f;
            float bottomV = 0.0f;

            _fullscreenMesh = new Mesh { name = "Fullscreen Quad" };
            _fullscreenMesh.SetVertices(new List<Vector3>
                {
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector3(-1.0f,  1.0f, 0.0f),
                    new Vector3(1.0f, -1.0f, 0.0f),
                    new Vector3(1.0f,  1.0f, 0.0f)
                });

            _fullscreenMesh.SetUVs(0, new List<Vector2>
                {
                    new Vector2(0.0f, bottomV),
                    new Vector2(0.0f, topV),
                    new Vector2(1.0f, bottomV),
                    new Vector2(1.0f, topV)
                });

            _fullscreenMesh.SetIndices(new[] { 0, 1, 2, 2, 1, 3 }, MeshTopology.Triangles, 0, false);
            _fullscreenMesh.UploadMeshData(true);
            return _fullscreenMesh;
        }
    }


    public static Vector2[] PointForCollider(Bounds bounds, float size = 1)
    {
        bounds.size *= size;
        Vector2 min = bounds.min;
        Vector2 max = bounds.max;

        Vector2[] vertices = new Vector2[4]
        {
                (new Vector2(min.x, min.y)),
                (new Vector2(max.x, min.y)),
                (new Vector2(max.x, max.y)),
                (new Vector2(min.x, max.y)),
        };
        return vertices;
    }

    public static Mesh GenerateMeshFromBound(Mesh mesh, Bounds bounds)
    {
        Vector2 min = bounds.min;
        Vector2 max = bounds.max;

        Vector3[] vertices = new Vector3[4]
        {
                (new Vector2(min.x, min.y)),
                (new Vector2(max.x, min.y)),
                (new Vector2(min.x, max.y)),
                (new Vector2(max.x, max.y)),
        };


        int[] tries = new int[6]
        {
                    0, 2, 1,
                    2, 3, 1
        };

        Vector2[] uv = new Vector2[4]
        {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = tries;
        mesh.uv = uv;
        return mesh;
    }


    public static Bounds GetSpriteBounds(Transform myRoot, bool includeDisabledGameObjects = true)
    {
        var b = new Bounds();
        GetMaxRect(myRoot, ref b, includeDisabledGameObjects);
        return b;
    }


    public static Bounds GetCameraBounds(Camera camera)
    {
        Vector2 size = camera.ViewportToWorldPoint(new Vector2(1, 1)) * 2;
        Bounds b = new Bounds(camera.transform.position, size);
        return b;
    }

    private static void GetMaxRect(Transform myRoot, ref Bounds bounds, bool includeDisabledGameObjects = true)
    {
        if (myRoot || includeDisabledGameObjects)
        {
            var spr = myRoot.GetComponent<Renderer>();
            if (spr != null)
            {
                bounds.Encapsulate(spr.bounds);
            }
        }

        for (int i = 0; i < myRoot.childCount; i++)
        {
            GetMaxRect(myRoot.GetChild(i), ref bounds);
        }
    }
}
