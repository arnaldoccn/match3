using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThree
{
    /// <summary>
    /// GameManager manages the cycle of life and win/lose condition of the game
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private Board _board;

        [SerializeField]
        private Text _clockText, _scoreText, _levelText, _buttonText;

        [SerializeField]
        private Button _nextLevelOrRestartButton;

        private float _currCountdownValue;
        private int _level, _scoreToNextLevel, _score;
        private const int _startLevel = 1;
        private const int _levelAddition = 1;
        private const int _startScore = 0;
        private const int _startLevelGoal = 100;
        private const int _secondsOnMinute = 60;
        private const int _roundTotalSeconds = 120;
        private const string _separator = "/";
        private const string _restart = "Restart";
        private const string _nextLevel = "Next Level";
        private const string _levelLabelText = "Level";
        private const string _clockFormat = "00";
        private const string _clockSeparator = ":";

        /// <summary>
        /// Start the clock of the game, format the labels and set the initial level and score
        /// </summary>
        private void Start()
        {
            SetStartLevel();
            StartCoroutine(StartCountdown(_roundTotalSeconds));
            _scoreText.text = _score + _separator + _scoreToNextLevel;
        }

        /// <summary>
        /// SetStartLevel set the game to the initial state
        /// </summary>
        private void SetStartLevel()
        {
            _level = _startLevel;
            _score = _startScore;
            _scoreToNextLevel = _startLevelGoal * _level;
        }

        /// <summary>
        /// UpdateScoreAndLevel update the labels with the actual value of the score, score to goal and the level
        /// </summary>
        private void UpdateScoreAndLevel(int score, int scoreToGoal, int level)
        {
            _scoreText.text = _score + _separator + _scoreToNextLevel;
            _levelText.text = _levelLabelText + _level;
        }

        /// <summary>
        /// StartCountdown initiate the countdown and check if the game is over
        /// </summary>
        public IEnumerator StartCountdown(float countdownValue)
        {
            _currCountdownValue = countdownValue;
            while (_currCountdownValue > 0)
            {
                string minutes = Mathf.Floor(_currCountdownValue / _secondsOnMinute).ToString(_clockFormat);
                string seconds = Mathf.Floor(_currCountdownValue % _secondsOnMinute).ToString(_clockFormat);
                if (Mathf.Floor(_currCountdownValue / _secondsOnMinute) >= 1)
                {
                    _clockText.text = minutes + _clockSeparator + seconds;
                }
                else
                {
                    _clockText.text = seconds;
                }

                yield return new WaitForSeconds(1.0f);
                _currCountdownValue--;

            }
            if (_currCountdownValue <= 0)
            {
                _clockText.text = _clockFormat;
                _nextLevelOrRestartButton.onClick.RemoveAllListeners();
                _nextLevelOrRestartButton.onClick.AddListener(() => Restart());
                _nextLevelOrRestartButton.gameObject.SetActive(true);
                _buttonText.text = _restart;
            }

        }

        /// <summary>
        /// AddScore add score to the total and check if the player achieve  the score goal
        /// </summary>
        public void AddScore(int scoreToAdd)
        {
            _score += scoreToAdd;
            _scoreText.text = _score + _separator + _scoreToNextLevel;
            if (_score >= _scoreToNextLevel)
            {
                SoundManager.Instance.PlayClearedLevel();
                StopAllCoroutines();
                _nextLevelOrRestartButton.onClick.RemoveAllListeners();
                _nextLevelOrRestartButton.onClick.AddListener(() => NextLevel());
                _nextLevelOrRestartButton.gameObject.SetActive(true);
                _buttonText.text = _nextLevel;
            }
        }

        /// <summary>
        /// Restart the game to the initial level
        /// </summary>
        public void Restart()
        {
            SetStartLevel();
            UpdateScoreAndLevel(_startScore, _startLevelGoal, _startLevel);
            Board.Instance.ClearBoard();

            StartCoroutine(StartCountdown(_roundTotalSeconds));
            _nextLevelOrRestartButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// NextLevel set the score to goal to the next level, reset the score and add the level
        /// </summary>
        public void NextLevel()
        {
            _level += _levelAddition;
            _score = _startScore;
            _scoreToNextLevel = _startLevelGoal * _level;
            UpdateScoreAndLevel(_level, _score, _scoreToNextLevel);
            Board.Instance.ClearBoard();

            StartCoroutine(StartCountdown(_roundTotalSeconds));
            _nextLevelOrRestartButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// GameIsPaused check if the game is waiting to the Player restar the game or go to the next level
        /// </summary>
        public bool GameIsPaused()
        {
            if (_score >= _scoreToNextLevel || _currCountdownValue <= 0)
            {
                return true;
            }
            return false;
        }
    }
}