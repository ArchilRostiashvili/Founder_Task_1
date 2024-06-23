using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.FacadeCommand
{
    public abstract class BaseFacadeCommand
    {
        public CommandState commandState = CommandState.Failed;

        public CommandState UpdateCommandState()
        {
            if (this.commandState != CommandState.Running)
            {
                this.StartCommand();
                return CommandState.Running;
            }
            else
            {
                CommandState newState = this.UpdateCommand();
                if (newState != CommandState.Running)
                {
                    this.ExitCommand();
                    return newState;
                }
            }

            return CommandState.Running;
        }

        protected abstract void StartCommand();
        protected abstract void ExitCommand();
        protected abstract CommandState UpdateCommand();
    }

    public enum CommandState
    {
        Running, Succeeded, Failed
    }
}
