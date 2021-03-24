using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree
{
    /// <summary>
    /// All the input must be managed by this class
    /// </summary>
    public static class InputManager 
    {
        private static Gem _selectedGem;

        /// <summary>
        /// Check if a Gem can be selected and if do so
        /// </summary>
        public static void SetSelectedGem(Gem gem)
        {
            if (!GameManager.Instance.GameIsPaused())
            {
                if (_selectedGem == null)
                {
                    _selectedGem = gem;
                    _selectedGem.SelectMe();
                }
                else if (Board.Instance.CheckRelateGems(_selectedGem, gem))
                {
                    SoundManager.Instance.PlaySFXSwapSound();
                    _selectedGem.DeselectMe();
                    Board.Instance.MoveGemsFromCells(_selectedGem.MyPositionOnTheBoard, gem.MyPositionOnTheBoard);
                    _selectedGem = null;

                }
                else
                {
                    SoundManager.Instance.PlaySFXSelectedGem();
                    _selectedGem.DeselectMe();
                    _selectedGem = gem;
                    _selectedGem.SelectMe();
                }
            }
        }
    }
}