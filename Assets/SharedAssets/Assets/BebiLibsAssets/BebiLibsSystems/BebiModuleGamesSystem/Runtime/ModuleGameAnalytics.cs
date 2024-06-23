using BebiLibs.Analytics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using BebiLibs.Analytics.GameEventLogger;
using BebiLibs.ModulesGameSystem;

namespace BebiLibs
{
    public class ModuleGameAnalytics : AnalyticsHelperBase
    {
        public const string GAME_ID = "game_id";
        public const string SETTING_PARAMETERS = "game_type";
        public const string GAME_LOAD_SOURCE = "start_type";
        public const string GAMEPLAY_DURATION = "sec";
        public const string IDLE_GAMEPLAY_DURATION = "sec_idle";
        public const string IDLE_STATE_COUNT = "idle_count";
        public const string ICON_ID = "icon_id";
        public const string COUNT = "count";
        public const string CLICK_COUNT = "click_count";
        public const string LOCKED_CD_COUNT = "locked_cd_count";
        public const string ID = "id";
        public const string MODULE = "module";
        public const string SUB_MODULE = "sub_module";

        public static long LastTimeStampInner { get; private set; }
        public static string InnerGameID { get; private set; }
        public static string CurrentGameID { get; private set; }
        public static string ModuleName { get; private set; }
        public static string SubModuleName { get; private set; }
        public static long LastTimeStamp { get; private set; }

        public static void SetStartType(string startType)
        {
            AnalyticsDictionary.StoreValue(AnalyticConstant.LoadOrigin, startType);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LobbyIconClick(string _lobbyIconID, int lobbyClickCount)
        {
            AnalyticsDictionary.StoreValue(AnalyticConstant.LoadOrigin, _lobbyIconID);

            GameParameterBuilder parameters = new GameParameterBuilder(1);
            parameters.Add(ICON_ID, _lobbyIconID);
            parameters.Add(COUNT, lobbyClickCount);
            AnalyticsManager.LogEvent("click_lobby_icon", parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void LockedIconClick(int lobbyClickCount, string gameID)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(2);
            parameters.Add(CLICK_COUNT, lobbyClickCount);
            parameters.Add(GAME_ID, gameID);
            AnalyticsManager.LogEvent(LOCKED_CD_COUNT, parameters);
        }

        public static void InnerGameClick(ModuleGameData moduleGame, List<IGameParameter> arrayExtraParameters = null)
        {
            InnerGameClick(moduleGame.ModuleName.ToLower(), moduleGame.ModuleName, moduleGame.AnalyticsID, arrayExtraParameters);
        }

        public static void InnerGameExit(ModuleGameData moduleGame, List<IGameParameter> arrayExtraParameters = null)
        {
            InnerGameExit(moduleGame.ModuleName.ToLower(), moduleGame.AnalyticsID, arrayExtraParameters);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        public static void GameStart(ModuleGameData moduleData, List<IGameParameter> arrayExtraParameters = null)
        {
            GameStart(moduleData.AnalyticsID, moduleData.ModuleName, arrayExtraParameters);
        }

        public static List<IGameParameter> GetSubModuleName(ModuleGameData moduleGameData)
        {
            List<IGameParameter> arrayExtraParameters = null;
            if (moduleGameData != null)
            {
                SubModuleName = moduleGameData.DisplayName;
                arrayExtraParameters = new List<IGameParameter> { new StringParameter(ModuleGameAnalytics.SUB_MODULE, moduleGameData.DisplayName) };
            }
            return arrayExtraParameters;
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void InnerGameClick(string gameID, string moduleName, string innerGameID, List<IGameParameter> arrayExtraParameters = null)
        {
            AnalyticsInteraction.Activate();
            LastTimeStampInner = TimeUtils.UnixTimeInSeconds();
            InnerGameID = innerGameID;
            ModuleName = moduleName;
            GameParameterBuilder parameters = new GameParameterBuilder(3);
            parameters.Add(MODULE, moduleName);
            parameters.Add(ID, InnerGameID);
            parameters.Add(SETTING_PARAMETERS, AgeAndHandPreferences());
            parameters.Add(GAME_LOAD_SOURCE, AnalyticsDictionary.GetValue(AnalyticConstant.LoadOrigin));

            if (arrayExtraParameters != null) parameters.Add(arrayExtraParameters);

            AnalyticsManager.LogEvent("click_" + gameID, parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void InnerGameExit(string gameID, string innerGameID = "", List<IGameParameter> arrayExtraParameters = null)
        {
            if (InnerGameID is null) return;
            innerGameID = innerGameID == "" ? InnerGameID : innerGameID;

            AnalyticsInteraction.Deactivate();
            long secondsTotal = TimeUtils.UnixTimeInSeconds() - LastTimeStampInner;
            GameParameterBuilder parameters = new GameParameterBuilder(6);
            parameters.Add(ID, innerGameID);
            parameters.Add(MODULE, ModuleName);
            parameters.Add(SETTING_PARAMETERS, AgeAndHandPreferences());
            parameters.Add(GAME_LOAD_SOURCE, AnalyticsDictionary.GetValue(AnalyticConstant.LoadOrigin));
            parameters.Add(GAMEPLAY_DURATION, secondsTotal);
            parameters.Add(IDLE_GAMEPLAY_DURATION, AnalyticsInteraction.IdleSeconds);
            parameters.Add(IDLE_STATE_COUNT, AnalyticsInteraction.IdleCount);

            if (arrayExtraParameters != null) parameters.Add(arrayExtraParameters);

            AnalyticsManager.LogEvent("exit_" + gameID, parameters);

            InnerGameID = null;
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void GameStart(string gameID, string moduleName, List<IGameParameter> arrayExtraParameters = null)
        {
            AnalyticsInteraction.Activate();
            CurrentGameID = gameID;
            LastTimeStamp = TimeUtils.UnixTimeInSeconds();

            ModuleName = moduleName;

            GameParameterBuilder parameters = new GameParameterBuilder(3);
            parameters.Add(MODULE, moduleName);
            parameters.Add(GAME_ID, gameID);
            parameters.Add(SETTING_PARAMETERS, AgeAndHandPreferences());
            parameters.Add(GAME_LOAD_SOURCE, AnalyticsDictionary.GetValue(AnalyticConstant.LoadOrigin));
            if (arrayExtraParameters != null) parameters.Add(arrayExtraParameters);

            AnalyticsManager.LogEvent("game_start", parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void GameExit(List<IGameParameter> arrayExtraParameters = null)
        {
            if (CurrentGameID is null) return;

            AnalyticsInteraction.Deactivate();
            long secondsTotal = TimeUtils.UnixTimeInSeconds() - LastTimeStamp;
            GameParameterBuilder parameters = new GameParameterBuilder(6);
            parameters.Add(GAME_ID, CurrentGameID);
            parameters.Add(SETTING_PARAMETERS, AgeAndHandPreferences());
            parameters.Add(GAME_LOAD_SOURCE, AnalyticsDictionary.GetValue(AnalyticConstant.LoadOrigin));

            if (!string.IsNullOrEmpty(SubModuleName))
            {
                parameters.Add(SUB_MODULE, SubModuleName);
            }

            parameters.Add(GAMEPLAY_DURATION, secondsTotal);
            parameters.Add(IDLE_GAMEPLAY_DURATION, AnalyticsInteraction.IdleSeconds);
            parameters.Add(IDLE_STATE_COUNT, AnalyticsInteraction.IdleCount);


            if (arrayExtraParameters != null) parameters.Add(arrayExtraParameters);

            AnalyticsManager.LogEvent("game_exit", parameters);

            SubModuleName = null;
            CurrentGameID = null;
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        public static void GameDone(List<IGameParameter> arrayExtraParameters = null)
        {
            if (CurrentGameID is null) return;
            AnalyticsInteraction.Deactivate();

            long secondsTotal = TimeUtils.UnixTimeInSeconds() - LastTimeStamp;

            GameParameterBuilder parameters = new GameParameterBuilder(6);
            parameters.Add(GAME_ID, CurrentGameID);
            parameters.Add(SETTING_PARAMETERS, AgeAndHandPreferences());

            if (!string.IsNullOrEmpty(ModuleName))
            {
                parameters.Add(MODULE, ModuleName);
            }

            parameters.Add(GAME_LOAD_SOURCE, AnalyticsDictionary.GetValue(AnalyticConstant.LoadOrigin));
            parameters.Add(GAMEPLAY_DURATION, secondsTotal);
            parameters.Add(IDLE_GAMEPLAY_DURATION, AnalyticsInteraction.IdleSeconds);
            parameters.Add(IDLE_STATE_COUNT, AnalyticsInteraction.IdleCount);
            if (arrayExtraParameters != null) parameters.Add(arrayExtraParameters);

            AnalyticsManager.LogEvent("game_done", parameters);
            CurrentGameID = null;
        }


        //0 for 1-2, 1 for 2-3, 2 for 3-4 ❤
        public static string AgeAndHandPreferences()
        {
            if (ManagerApp.Instance == null) return string.Empty;
            int ageGroupIndex = ManagerApp.Config.AgeGroup;
            int handSide = ManagerApp.Config.HandSide ? 2 : 1;
            string data = $"{ageGroupIndex + 1}/{handSide}";
            return data;
        }
    }
}
