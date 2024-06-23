using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.FacadeCommand
{
    public class FacadeCommandInvoker : MonoBehaviour
    {
        public static FacadeCommandInvoker Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                GameObject.Destroy(this);
            }
        }

        public Stack<BaseFacadeCommand> arrayFacadeCommands = new Stack<BaseFacadeCommand>();
        public BaseFacadeCommand _activeFacadeCommand;

        public static void InjectCommandLast(BaseFacadeCommand facadeCommand)
        {
            Instance.arrayFacadeCommands.Push(facadeCommand);
        }

        public void Update()
        {
            if (this.arrayFacadeCommands.Count > 0 && _activeFacadeCommand == null)
            {
                _activeFacadeCommand = arrayFacadeCommands.Pop();
            }

            if (_activeFacadeCommand != null)
            {
                if (_activeFacadeCommand.UpdateCommandState() != CommandState.Running)
                {
                    _activeFacadeCommand = null;
                }
            }
        }

    }
}
