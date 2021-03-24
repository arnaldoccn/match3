using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MatchThree
{
    /// <summary>
    /// Gem base class, have all generalized gem behaviour
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
     public abstract class Gem : MonoBehaviour
    {
        public enum GemType { Milk, Apple, Orange, Bread, Lettuce, Coconut, Carambola }

        [SerializeField]
        private ScriptableGem _scriptableGem;

        private GemType _myType;
        private Vector3 _touchPosition;
        private Vector3 _direction;
        private Vector2 _myPositionOnTheBoard;
        public Vector2 MyPositionOnTheBoard { get => _myPositionOnTheBoard; }
        private int _myScore;

        public GemType MyType { get => _myType; }

        /// <summary>
        /// On Awake it gets its type from the Scriptable Object Gem its type and its score
        /// </summary>
        private void Awake()
        {
            _myType = _scriptableGem.GemType;
            _myScore = _scriptableGem.Score;
        }

        /// <summary>
        /// On mouse up from the mouse it will try to select itself
        /// </summary>
        void OnMouseUp()
        {
            InputManager.SetSelectedGem(this);
        }

        /// <summary>
        /// SetGemPosition set its logical and transform position
        /// </summary>
        public void SetGemPosition(Vector2 myPositionOnTheBoard, Vector3 cellTransFormPosition)
        {
            _myPositionOnTheBoard = myPositionOnTheBoard;
            this.transform.position = cellTransFormPosition;
        }

        /// <summary>
        /// AddScore add the score to the player
        /// </summary>
        public void AddScore()
        {
            GameManager.Instance.AddScore(_myScore);
        }

        /// <summary>
        /// SelectMe executes the select behaviour
        /// </summary>
        public void SelectMe()
        {
            SoundManager.Instance.PlaySFXSelectedGem();
            this.GetComponent<SpriteRenderer>().color = Color.grey;
        }

        /// <summary>
        /// DeselectMe executes the deselect behaviour
        /// </summary>
        public void DeselectMe()
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
