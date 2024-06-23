
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * English Tracing Book Package
 *
 * @license		    Unity Asset Store EULA https://unity3d.com/legal/as_terms
 * @author		    Indie Studio - Baraa Nasser
 * @Website		    https://indiestd.com
 * @Asset Store     https://assetstore.unity.com/publishers/9268
 * @Unity Connect   https://connect.unity.com/u/5822191d090915001dbaf653/column
 * @email		    info@indiestd.com
 *
 */

namespace IndieStudio.EnglishTracingBook.Game
{
    [DisallowMultipleComponent]
    public class Shape : MonoBehaviour
    {
        /// <summary>
        /// The paths of the shape.
        /// </summary>
        public List<TracingPath> tracingPaths = new List<TracingPath>();

        /// <summary>
        /// Whether the shape is completed or not.
        /// </summary>
        [HideInInspector]
        public bool completed;

        /// <summary>
        /// Whether to enable the priority order or not.
        /// </summary>
        [HideInInspector]
        public bool enablePriorityOrder = true;

        /// <summary>
        /// The animator of the shape
        /// </summary>
        public Animator animator;

        /// <summary>
        /// The content gameobject under the shape
        /// </summary>
        [HideInInspector]
        public Transform _content;
        public Transform content
        {
            get
            {
                if (_content == null)
                {
                    _content = this.transform.Find("Content");
                }
                return _content;
            }
        }

        /// <summary>
        /// The id of the shape
        /// </summary>
        [HideInInspector]
        public int ID;

        private ITracingExtension SC_GameTracingNumbers;

        // Use this for initialization
        void Awake()
        {
            this.SC_GameTracingNumbers = FindObjectsOfType<MonoBehaviour>().OfType<ITracingExtension>().ToArray().FirstOrDefault();

            if (_content == null)
            {
                _content = this.transform.Find("Content");
            }

            if (this.animator == null)
            {
                this.animator = this.GetComponent<Animator>();
            }
        }

        /// <summary>
        /// Show the numbers of the path .
        /// </summary>
        /// <param name="index">Index.</param>
        public void ShowPathNumbers(int index)
        {
            for (int i = 0; i < this.tracingPaths.Count; i++)
            {
                if (i != index)
                {
                    this.tracingPaths[i].SetNumbersStatus(false);
                    if (this.enablePriorityOrder || this.tracingPaths[i].completed)
                    {
                        this.tracingPaths[i].curve.gameObject.SetActive(false);
                    }
                }
                else
                {
                    this.tracingPaths[i].SetNumbersStatus(true);
                    this.tracingPaths[i].curve.gameObject.SetActive(true);
                }
            }
        }

        public void ShowCurrentFirstNumber(int index)
        {
            if (-1 < index && index < tracingPaths.Count && this.tracingPaths[index] != null)
                this.tracingPaths[index].SetFirstNumberVisible(true);
        }

        /// <summary>
        /// Get the index of the current path.
        /// </summary>
        /// <returns>The current path index.</returns>
        public int GetCurrentPathIndex()
        {
            int index = -1;
            for (int i = 0; i < this.tracingPaths.Count; i++)
            {
                if (this.tracingPaths[i].completed)
                {
                    continue;
                }

                //current if all of previous paths of it is completed
                bool isCurrentPath = true;
                for (int j = 0; j < i; j++)
                {
                    if (!this.tracingPaths[j].completed)
                    {
                        isCurrentPath = false;
                        break;
                    }
                }

                if (isCurrentPath)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Determine whether the given tracing path instance is the current path or not.
        /// </summary>
        /// <returns><c>true</c> if this instance is current path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Tracing Path.</param>
        public bool IsCurrentPath(TracingPath tracingPath)
        {
            bool isCurrentPath = false;

            if (!this.enablePriorityOrder)
            {
                return true;
            }

            if (tracingPath == null)
            {
                return isCurrentPath;
            }

            isCurrentPath = true;
            for (int i = 0; i < this.tracingPaths.Count; i++)
            {
                if (this.tracingPaths[i].GetInstanceID() == tracingPath.GetInstanceID())
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (!this.tracingPaths[j].completed)
                        {
                            isCurrentPath = false;
                            break;
                        }
                    }
                    break;
                }
            }

            return isCurrentPath;
        }

        /// <summary>
        /// Get index of the tracing path
        /// </summary>
        /// <param name="tracingPath">Tracing Path Reference</param>
        /// <returns>Index of the Tracing Path in the List</returns>
        public int GetPathIndex(TracingPath tracingPath)
        {
            if (tracingPath == null)
                return -1;

            for (int i = 0; i < this.tracingPaths.Count; i++)
            {
                if (this.tracingPaths[i] == null)
                    continue;

                if (this.tracingPaths[i].GetInstanceID() == tracingPath.GetInstanceID())
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Get the title of the shape.
        /// </summary>
        /// <returns>The title.</returns>
        public string GetTitle()
        {
            if (GameManager.instance != null)
            {
                if (GameManager.instance.compoundShape == null)
                {
                    return this.name.Split('-')[0];
                }
                return GameManager.instance.compoundShape.name.Split('-')[0];
            }
            else if (this.SC_GameTracingNumbers != null)
            {
                if (this.SC_GameTracingNumbers.CompoundShape == null)
                {
                    return this.name.Split('-')[0];
                }
                return this.SC_GameTracingNumbers.CompoundShape.name.Split('-')[0];
            }
            return this.name.Split('-')[0];
        }

        /// <summary>
        /// Get the shape instance in the shapes manager
        /// </summary>
        /// <returns></returns>
        public ShapesManager.Shape GetShapeFromShapesManager()
        {
            if (ShapesManager.GetCurrentShapesManager() == null)
                return null;
            if (this.ID < 0 || this.ID > ShapesManager.GetCurrentShapesManager().shapes.Count - 1)
                return null;

            return ShapesManager.GetCurrentShapesManager().shapes[this.ID];
        }
    }
}