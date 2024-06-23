using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BebiLibs.FacadeCommand
{
    public class ActionRunnerCommand : BaseFacadeCommand
    {
        public System.Action onCommandStarts;

        public ActionRunnerCommand(System.Action action)
        {
            this.onCommandStarts = action;
        }

        protected override void ExitCommand()
        {

        }

        protected override void StartCommand()
        {
            this.onCommandStarts?.Invoke();
        }

        protected override CommandState UpdateCommand()
        {
            return this.commandState;
        }
    }
}
