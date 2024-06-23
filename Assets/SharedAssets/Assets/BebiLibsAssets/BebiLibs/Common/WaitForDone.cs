using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public sealed class WaitForDone : CustomYieldInstruction
    {
        private Func<bool> m_Predicate;
        private float m_timeout;


        public override bool keepWaiting
        {
            get
            {
                m_timeout -= Time.deltaTime;
                return !(m_timeout <= 0f || m_Predicate());
            }
        }

        public WaitForDone(float timeout, Func<bool> predicate)
        {
            m_Predicate = predicate;
            m_timeout = timeout;
        }
    }

    public sealed class WaitForDoneRealtime : CustomYieldInstruction
    {
        private Func<bool> _predicate;
        private double _timeTargetInRealTime;

        public override bool keepWaiting
        {
            get
            {
                return !(Time.realtimeSinceStartupAsDouble >= _timeTargetInRealTime || _predicate());
            }
        }

        public WaitForDoneRealtime(float realtimeInSeconds, Func<bool> predicate)
        {
            _predicate = predicate;
            _timeTargetInRealTime = Time.realtimeSinceStartupAsDouble + realtimeInSeconds;
        }
    }
}
