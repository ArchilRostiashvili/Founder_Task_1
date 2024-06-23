using UnityEngine;
using TMPro;


namespace BebiLibs
{
    public class TimeViewer : MonoBehaviour
    {
        public delegate string DayCountTextDelegate(int day);

        public event System.Action OnTimerCountdownEndEvent;
        public DayCountTextDelegate DayCountTextProvider = null;

        [SerializeField] private TMP_Text _remainingDayCountDisplayText;
        [SerializeField] private TMP_Text _remainingHoursDisplayText;
        [SerializeField] private TMP_Text _remainingMinutesDisplayText;
        [SerializeField] private TMP_Text _remainingSecondsDisplayText;

        private double _lastInterval;
        private double _countDownTime;

        private int _lastHour = 0;
        private int _lastMinute = 0;
        private int _lastSecond = 0;

        private readonly char[] _lookUp = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', };
        private char[] _minutes = new char[2] { '0', '0' };



        public void Show(System.TimeSpan timeSpan)
        {
            Show(timeSpan.TotalSeconds);
        }

        public void Show(double countDownTime)
        {
            //Debug.Log("Show Time Viewer: " + countDownTime);
            gameObject.SetActive(true);
            _countDownTime = (int)countDownTime;
            int d = (int)(_countDownTime / (3600 * 24));
            if (d > 0)
            {
                _remainingDayCountDisplayText.text = DayCountTextProvider != null ? DayCountTextProvider(d) : DefaultDayCountTextProvider(d);
                ShowDayCounter();
                enabled = false;
            }
            else if (_countDownTime > 0)
            {
                ShowTimer();
                enabled = true;
            }

            if (_countDownTime < 0)
            {
                OnTimerCountdownEndEvent?.Invoke();
            }

            _lastInterval = (int)Time.realtimeSinceStartup;
        }

        public string DefaultDayCountTextProvider(int day)
        {
            return "Unlock In " + day + " Day";
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void ShowTimer()
        {
            _remainingDayCountDisplayText.gameObject.SetActive(false);
            _remainingHoursDisplayText.gameObject.SetActive(true);
            _remainingMinutesDisplayText.gameObject.SetActive(true);
            _remainingSecondsDisplayText.gameObject.SetActive(true);

            _remainingHoursDisplayText.text = "00:";
            _remainingMinutesDisplayText.text = "00:";
            _remainingSecondsDisplayText.text = "00";
            _lastHour = 0;
            _lastMinute = 0;
            _lastSecond = 0;
        }


        private void ShowDayCounter()
        {
            _remainingDayCountDisplayText.gameObject.SetActive(true);
            _remainingHoursDisplayText.gameObject.SetActive(false);
            _remainingMinutesDisplayText.gameObject.SetActive(false);
            _remainingSecondsDisplayText.gameObject.SetActive(false);
        }


        private void UpdateHourText(int hour)
        {
            if (hour >= 0 && _lastHour != hour)
            {
                _remainingHoursDisplayText.text = hour < 10 ? $"0{hour}:" : $"{hour}:";
                _lastHour = hour;
            }
        }

        private void UpdateMinute(int minute)
        {
            if (minute >= 0 && _lastMinute != minute)
            {
                _remainingMinutesDisplayText.text = minute < 10 ? $"0{minute}:" : $"{minute}:";
                _lastMinute = minute;
            }
        }

        private void UpdateSeconds(int second)
        {
            if (_lastSecond == second) return;

            if (second >= 0)
            {
                _minutes[0] = _lookUp[second / 10 % 10];
                _minutes[1] = _lookUp[second % 10];
            }

            _remainingSecondsDisplayText.SetText(_minutes);
            _lastSecond = second;
        }

        private void UpdateClock(double currentTime)
        {
            int hours = (int)(currentTime / 3600);
            UpdateHourText(hours);
            int minute = (int)(currentTime % 3600 / 60);
            UpdateMinute(minute);
            int second = (int)(currentTime % 60);
            UpdateSeconds(second);
        }


        private void Update()
        {
            double tick = Time.realtimeSinceStartupAsDouble - _lastInterval;
            double realTime = _countDownTime - tick;
            UpdateClock(realTime);
            if (realTime < 0)
            {
                OnTimerCountdownEndEvent?.Invoke();
                enabled = false;
            }
        }

    }
}