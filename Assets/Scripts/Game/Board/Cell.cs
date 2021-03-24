using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree
{
    /// <summary>
    /// The cell is a container for the gems
    /// </summary>
    public class Cell : MonoBehaviour
    {
        private Gem _myGem;
        private Vector2 _myPositionOnTheBoard;

        public Gem MyGem { get => _myGem; }
        public Vector2 MyPositionOnTheBoard { get => _myPositionOnTheBoard; }

        /// <summary>
        /// Set the gem on this cell and what position this cell, and the gem, have one the board
        /// </summary>
        public void PopulateCell(Gem gem, Vector2 myPositionOnTheBoard)
        {
            _myGem = Instantiate(gem, this.transform.position, Quaternion.identity, this.transform);
            _myPositionOnTheBoard = myPositionOnTheBoard;
            _myGem.SetGemPosition(_myPositionOnTheBoard, this.transform.position);
        }

        /// <summary>
        /// Changes the gem set on the Cell
        /// </summary>
        public void UpdateMyGem(Gem gem)
        {
            _myGem = gem;
            _myGem.transform.SetParent(this.transform);
            _myGem.SetGemPosition(_myPositionOnTheBoard, this.transform.position);
        }

        /// <summary>
        /// Destroy the logical reference of the gem in the cell
        /// </summary>
        public void DestroyGem()
        {
            _myGem = null;
        }
    }
}