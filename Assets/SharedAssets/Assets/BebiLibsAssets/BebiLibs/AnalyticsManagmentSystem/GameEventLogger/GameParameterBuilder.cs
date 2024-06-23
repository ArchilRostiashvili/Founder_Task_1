using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.Analytics.GameEventLogger
{
    public class GameParameterBuilder
    {
        private List<IGameParameter> _gameParametersList;

        public GameParameterBuilder()
        {
            _gameParametersList = new List<IGameParameter>();
        }


        public GameParameterBuilder(int capacity)
        {
            _gameParametersList = new List<IGameParameter>(capacity);
        }

        public List<IGameParameter> GetParameters() => _gameParametersList;
        public void Clear() => _gameParametersList.Clear();

        public GameParameterBuilder Add(List<IGameParameter> gameParameters)
        {
            foreach (var gameParameter in gameParameters)
            {
                Add(gameParameter);
            }
            return this;
        }

        public GameParameterBuilder Add(IGameParameter gameParameter)
        {
            _gameParametersList.Add(gameParameter);
            return this;
        }

        public GameParameterBuilder Add(string parameter, string value)
        {
            StringParameter param = new StringParameter(parameter, value);
            _gameParametersList.Add(param);
            return this;
        }

        public GameParameterBuilder Add(Func<string> parameterName, string value)
        {
            StringParameter param = new StringParameter(parameterName, value);
            _gameParametersList.Add(param);
            return this;
        }

        public GameParameterBuilder Add(string parameter, long value)
        {
            LongParameter param = new LongParameter(parameter, value);
            _gameParametersList.Add(param);
            return this;
        }

        public GameParameterBuilder Add(Func<string> parameterName, long value)
        {
            LongParameter param = new LongParameter(parameterName, value);
            _gameParametersList.Add(param);
            return this;
        }

        public GameParameterBuilder Add(string parameter, double value)
        {
            DoubleParameter param = new DoubleParameter(parameter, value);
            _gameParametersList.Add(param);
            return this;
        }

        public GameParameterBuilder Add(Func<string> parameterName, double value)
        {
            DoubleParameter param = new DoubleParameter(parameterName, value);
            _gameParametersList.Add(param);
            return this;
        }


        public static GameParameterBuilder Empty() => new GameParameterBuilder();
        public static implicit operator List<IGameParameter>(GameParameterBuilder d) => d.GetParameters();

    }

}
