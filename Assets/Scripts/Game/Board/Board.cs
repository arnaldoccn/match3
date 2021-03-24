using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MatchThree
{
    /// <summary>
    /// The board class.
    /// This have all the logic that the Gems and its containers, the Cells, have.
    /// </summary>
    public class Board : Singleton<Board>
    {
        [SerializeField]
        private Cell _cell;
        [SerializeField]
        private List<Gem> _gemList = new List<Gem>();

        private List<Cell> _matchCellsListUp = new List<Cell>();
        private List<Cell> _matchCellsListDown = new List<Cell>();
        private List<Cell> _matchCellsListLeft = new List<Cell>();
        private List<Cell> _matchCellsListRight = new List<Cell>();

        private List<Gem> _predictMatchGemListUp = new List<Gem>();
        private List<Gem> _predictMatchGemListDown = new List<Gem>();
        private List<Gem> _predictMatchGemListLeft = new List<Gem>();
        private List<Gem> _predictMatchGemListRight = new List<Gem>();

        private Cell[,] _cellGrid = new Cell[6, 6];

        private Cell _cellToCheck;
        private Gem _gem;
        private const int _minimumBoardLimit = 0;
        private const int _maximumBoardLimit = 6;
        private const int _row = 6;
        private const int _col = 6;
        private const int _neighbourGem = 1;
        private const int _neighbourOfNeighbourGem = 2;
        private const int _possibleCombinationGem = 3;
        private const float _generalOffset = -0.7f;
        private const float _widthOffset = 2.5f;
        private const float _heightOffset = 2f;
        private const string _cellName = "Cell ";
        private const string _separator = "/";


        /// <summary>
        /// On the Start it generates the board
        /// </summary>
        private void Start()
        {
            GenerateBoard();
        }

        /// <summary>
        /// GenerateBoard Instantiate the grid with a predetermined numer of rows and columns and checks if it already have a combination or if dont have a possible combination, in either case it clear the board.
        /// If not the game can be played
        /// </summary>
        private void GenerateBoard()
        {
            for (int x = 0; x < _row; x++)
            {
                for (int y = 0; y < _col; y++)
                {
                    Cell newCell = Instantiate(_cell, new Vector3((x - _widthOffset) * _generalOffset, (y - _heightOffset) * _generalOffset, 0), Quaternion.identity, this.transform);
                    newCell.transform.name = _cellName + x + _separator + y;
                    SetCell(newCell, new Vector2(x, y));
                    _cellGrid[x, y] = newCell;
                }
            }

            if (CheckIfBoardHaveMatch())
            {
                ClearBoard();
            }
            else if (!CheckIfBoardHaveAPossibleCombination())
            {
                ClearBoard();
            }
        }

        /// <summary>
        /// SetCell sends to a cell its position on the grid and draft what kind of gem it will be
        /// </summary>
        private void SetCell(Cell cell, Vector2 positionOnTheBoard)
        {
            cell.PopulateCell(_gemList.ElementAt(Random.Range(0, _gemList.Count())), positionOnTheBoard);
        }

        /// <summary>
        /// MoveGemsFromCells changes the position between two Gems
        /// </summary>
        public void MoveGemsFromCells(Vector2 firstSelected, Vector2 secondSelected)
        {
            Gem cellOfTheFirstGemSelected = _cellGrid[(int)firstSelected.x, (int)firstSelected.y].MyGem;
            Gem cellOfTheSecondGemSelected = _cellGrid[(int)secondSelected.x, (int)secondSelected.y].MyGem;

            _cellGrid[(int)firstSelected.x, (int)firstSelected.y].UpdateMyGem(cellOfTheSecondGemSelected);
            _cellGrid[(int)secondSelected.x, (int)secondSelected.y].UpdateMyGem(cellOfTheFirstGemSelected);

            MatchAndCheckTheBoard(_cellGrid[(int)secondSelected.x, (int)secondSelected.y], _cellGrid[(int)firstSelected.x, (int)firstSelected.y]);
        }

        /// <summary>
        /// MatchAndCheckTheBoard controls the board behaviour when occours a match. It will create the list of the matchs from the two gems moves by the player. It will reposition all the gems when all the matched ones is destroyed and do the board checks
        /// </summary>
        private void MatchAndCheckTheBoard(Cell cellMovedOne, Cell cellMovedTwo)
        {
            CreateMatchList(cellMovedOne);
            CreateMatchList(cellMovedTwo);
            RepositioningGems();
            CheckIfTheresCombinationOnTheBoard();
            if (!CheckIfBoardHaveAPossibleCombination())
            {
                ClearBoard();
            }
        }

        /// <summary>
        /// CheckIfTheresCombinationOnTheBoard checks recursively if there's a combination on the board
        /// </summary>
        private void CheckIfTheresCombinationOnTheBoard()
        {
            if (CheckIfBoardHaveMatch())
            {
                foreach (var cell in _cellGrid)
                {
                    CreateMatchList(_cellGrid[(int)cell.MyPositionOnTheBoard.x, (int)cell.MyPositionOnTheBoard.y]);
                }
                RepositioningGems();
                CheckIfTheresCombinationOnTheBoard();
            }
        }

        /// <summary>
        /// CreateMatchList create in all four directions lists from possible matches.
        /// </summary>
        private void CreateMatchList(Cell cellMoved)
        {
            CheckAdjacentCellRight(cellMoved);
            CheckAdjacentCellLeft(cellMoved);
            CheckAdjacentCellUp(cellMoved);
            CheckAdjacentCellDown(cellMoved);

            /// <summary>
            /// Send two directions of matches Up with Down and Right with Up to separe rows from columns to be tested
            /// </summary>
            DestroyMacthGems(_matchCellsListDown, _matchCellsListUp, cellMoved);
            DestroyMacthGems(_matchCellsListRight, _matchCellsListLeft, cellMoved);

            ClearCellLists();
        }

        /// <summary>
        /// DestroyMacthGems combine two directions of match and checks if theres a combination. If theres is it destroy all the matched gems 
        /// </summary>
        private void DestroyMacthGems(List<Cell> listToCheckA, List<Cell> listToCheckB, Cell movedCell)
        {
            List<Cell> listToCheck = new List<Cell>();
            listToCheck.AddRange(listToCheckA);
            listToCheck.AddRange(listToCheckB);

            if (listToCheck.Count > 1)
            {
                listToCheck.Add(movedCell);
                foreach (var cell in listToCheck)
                {
                    DestroyGems(cell);
                }
            }
            listToCheck.Clear();
        }

        /// <summary>
        /// DestroyGems destroy a gem of a cell
        /// </summary>
        private void DestroyGems(Cell cell)
        {
            if (cell.MyGem != null)
            {
                cell.MyGem.AddScore();
                Destroy(cell.MyGem.gameObject);
                cell.DestroyGem();
            }
        }

        /// <summary>
        /// FilltheBoard Checks which cell don't have a gem and atributes one to it
        /// </summary>
        private void FilltheBoard()
        {
            for (int x = _minimumBoardLimit; x < _row; x++)
            {
                for (int y = _minimumBoardLimit; y < _col; y++)
                {
                    if (_cellGrid[x, y].MyGem == null)
                    {
                        SetCell(_cellGrid[x, y], _cellGrid[x, y].MyPositionOnTheBoard);
                    }
                }
            }
        }

        /// <summary>
        /// CheckPredictiveCombination combine two directions of possible matches and verify if this is enough to the game continues
        /// </summary>
        private bool CheckPredictiveCombination(List<Gem> listToCheckA, List<Gem> listToCheckB)
        {
            List<Gem> listToCheck = new List<Gem>();
            listToCheck.AddRange(listToCheckA);
            listToCheck.AddRange(listToCheckB);

            if (listToCheck.Count >= 2)
            {
                listToCheck.Clear();
                return true;
            }
            else
            {
                listToCheck.Clear();
                return false;
            }
        }

        /// <summary>
        /// ClearCellLists clear lists used to match gems
        /// </summary>
        private void ClearCellLists()
        {
            _matchCellsListLeft.Clear();
            _matchCellsListUp.Clear();
            _matchCellsListRight.Clear();
            _matchCellsListDown.Clear();
        }

        /// <summary>
        /// ClearPredictiveCellLists clear lists used to check if theres a possible match
        /// </summary>
        private void ClearPredictiveCellLists()
        {
            _predictMatchGemListDown.Clear();
            _predictMatchGemListLeft.Clear();
            _predictMatchGemListRight.Clear();
            _predictMatchGemListUp.Clear();
        }

        /// <summary>
        /// CheckAdjacentCellRight check recursively and lists all the gems of the same type of a determined gem on its right
        /// </summary>
        private void CheckAdjacentCellRight(Cell cellToCheck)
        {
            if (cellToCheck.MyPositionOnTheBoard.x - _neighbourGem >= _minimumBoardLimit &&
                _cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x - _neighbourGem, (int)cellToCheck.MyPositionOnTheBoard.y].MyGem != null &&
                cellToCheck.MyGem != null)
            {
                if (cellToCheck.MyGem.MyType == _cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x - _neighbourGem, (int)cellToCheck.MyPositionOnTheBoard.y].MyGem.MyType)
                {
                    _matchCellsListRight.Add(_cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x - _neighbourGem, (int)cellToCheck.MyPositionOnTheBoard.y]);
                    CheckAdjacentCellRight(_cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x - _neighbourGem, (int)cellToCheck.MyPositionOnTheBoard.y]);
                }
            }
        }

        /// <summary>
        /// CheckAdjacentCellRight check recursively and lists all the gems of the same type of a determined gem on its left
        /// </summary>
        private void CheckAdjacentCellLeft(Cell cellToCheck)
        {
            if (cellToCheck.MyPositionOnTheBoard.x + _neighbourGem < _maximumBoardLimit &&
               _cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x + _neighbourGem, (int)cellToCheck.MyPositionOnTheBoard.y].MyGem != null &&
                cellToCheck.MyGem != null)
            {
                if (cellToCheck.MyGem.MyType == _cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x + _neighbourGem, (int)cellToCheck.MyPositionOnTheBoard.y].MyGem.MyType)
                {
                    _matchCellsListLeft.Add(_cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x + _neighbourGem, (int)cellToCheck.MyPositionOnTheBoard.y]);
                    CheckAdjacentCellLeft(_cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x + _neighbourGem, (int)cellToCheck.MyPositionOnTheBoard.y]);
                }
            }
        }

        /// <summary>
        /// CheckAdjacentCellRight check recursively and lists all the gems of the same type of a determined gem on its up
        /// </summary>
        private void CheckAdjacentCellUp(Cell cellToCheck)
        {
            if (cellToCheck.MyPositionOnTheBoard.y - _neighbourGem >= _minimumBoardLimit &&
                 _cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x, (int)cellToCheck.MyPositionOnTheBoard.y - _neighbourGem].MyGem != null &&
                cellToCheck.MyGem != null)
            {
                if (cellToCheck.MyGem.MyType == _cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x, (int)cellToCheck.MyPositionOnTheBoard.y - _neighbourGem].MyGem.MyType)
                {
                    _matchCellsListUp.Add(_cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x, (int)cellToCheck.MyPositionOnTheBoard.y - _neighbourGem]);
                    CheckAdjacentCellUp(_cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x, (int)cellToCheck.MyPositionOnTheBoard.y - _neighbourGem]);
                }
            }
        }

        /// <summary>
        /// CheckAdjacentCellRight check recursively and lists all the gems of the same type of a determined gem on its down
        /// </summary>
        private void CheckAdjacentCellDown(Cell cellToCheck)
        {
            if (cellToCheck.MyPositionOnTheBoard.y + _neighbourGem < _maximumBoardLimit &&
                _cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x, (int)cellToCheck.MyPositionOnTheBoard.y + _neighbourGem].MyGem != null &&
                cellToCheck.MyGem != null)
            {
                if (cellToCheck.MyGem.MyType == _cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x, (int)cellToCheck.MyPositionOnTheBoard.y + _neighbourGem].MyGem.MyType)
                {
                    _matchCellsListDown.Add(_cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x, (int)cellToCheck.MyPositionOnTheBoard.y + _neighbourGem]);
                    CheckAdjacentCellDown(_cellGrid[(int)cellToCheck.MyPositionOnTheBoard.x, (int)cellToCheck.MyPositionOnTheBoard.y + _neighbourGem]);
                }
            }
        }

        /// <summary>
        /// ClearBoard destroy all the Gems GameObjects and its references from the Cells
        /// </summary>
        public void ClearBoard()
        {
            for (int x = _minimumBoardLimit; x < _row; x++)
            {
                for (int y = _minimumBoardLimit; y < _col; y++)
                {
                    Destroy(_cellGrid[x, y].transform.gameObject);
                }
            }

            for (int x = _minimumBoardLimit; x < _row; x++)
            {
                for (int y = _minimumBoardLimit; y < _col; y++)
                {
                    _cellGrid[x, y] = null;
                }
            }

            GenerateBoard();
        }

        /// <summary>
        /// CheckIfBoardHaveAPossibleCombination check if the Board have a combination possible from a single movement
        /// </summary>
        private bool CheckIfBoardHaveAPossibleCombination()
        {
            ClearPredictiveCellLists();

            for (int x = _minimumBoardLimit; x < _row; x++)
            {
                for (int y = _minimumBoardLimit; y < _col; y++)
                {
                    for (int cx = _minimumBoardLimit; cx < _row; cx++)
                    {
                        for (int cy = _minimumBoardLimit; cy < _col; cy++)
                        {
                            if (CheckRelateGems(_cellGrid[x, y].MyGem, _cellGrid[cx, cy].MyGem))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// CheckRelateGems check if two gems are different, if one is adjacent to the other, but not diagonally, if they have the same type and simulates the inversion of position between them
        /// </summary>
        public bool CheckRelateGems(Gem gemA, Gem gemB)
        {
            if (!IsTheSameGem(gemA, gemB))
            {
                if (IsAdjacentCell(gemA, gemB))
                {
                    if (gemA.MyType != gemB.MyType)
                    {
                        if ((CheckIfGemHaveAPossibleCombination(gemA, gemB.MyType, gemB.MyPositionOnTheBoard) ||
                             CheckIfGemHaveAPossibleCombination(gemB, gemA.MyType, gemA.MyPositionOnTheBoard)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// CheckIfGemHaveAPossibleCombination check if inverting two gems of position it can generate a match
        /// </summary>
        private bool CheckIfGemHaveAPossibleCombination(Gem gemToChangeThePosition, Gem.GemType gemType, Vector2 myActualPosition)
        {
            if (gemToChangeThePosition.MyPositionOnTheBoard.x - _neighbourGem >= _minimumBoardLimit)
            {
                if (gemType.ToString() == _cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x - _neighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyGem.MyType.ToString())
                {
                    if (_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x - _neighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyPositionOnTheBoard != myActualPosition)
                    {
                        _predictMatchGemListRight.Add(_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x - _neighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyGem);
                        if (gemToChangeThePosition.MyPositionOnTheBoard.x - _neighbourOfNeighbourGem >= _minimumBoardLimit)
                        {
                            if (gemType.ToString() == _cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x - _neighbourOfNeighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyGem.MyType.ToString())
                            {
                                _predictMatchGemListRight.Add(_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x - _neighbourOfNeighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyGem);
                            }
                        }
                    }
                }
            }
            if (gemToChangeThePosition.MyPositionOnTheBoard.x + _neighbourGem < _maximumBoardLimit)
            {
                if (gemType.ToString() == _cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x + _neighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyGem.MyType.ToString())
                {
                    if (_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x + _neighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyPositionOnTheBoard != myActualPosition)
                    {
                        _predictMatchGemListLeft.Add(_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x + _neighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyGem);
                        if (gemToChangeThePosition.MyPositionOnTheBoard.x + _neighbourOfNeighbourGem < _maximumBoardLimit)
                        {
                            if (gemType.ToString() == _cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x + _neighbourOfNeighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyGem.MyType.ToString())
                            {
                                _predictMatchGemListLeft.Add(_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x + _neighbourOfNeighbourGem, (int)gemToChangeThePosition.MyPositionOnTheBoard.y].MyGem);
                            }
                        }
                    }
                }
            }

            if (CheckPredictiveCombination(_predictMatchGemListLeft, _predictMatchGemListRight))
            {
                ClearPredictiveCellLists();
                return true;
            }

            if (gemToChangeThePosition.MyPositionOnTheBoard.y - _neighbourGem >= _minimumBoardLimit)
            {
                if (gemType.ToString() == _cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y - _neighbourGem].MyGem.MyType.ToString())
                {
                    if (_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y - _neighbourGem].MyPositionOnTheBoard != myActualPosition)
                    {
                        _predictMatchGemListUp.Add(_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y - _neighbourGem].MyGem);
                        if (gemToChangeThePosition.MyPositionOnTheBoard.y - _neighbourOfNeighbourGem >= _minimumBoardLimit)
                        {
                            if (gemType.ToString() == _cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y - _neighbourOfNeighbourGem].MyGem.MyType.ToString())
                            {
                                _predictMatchGemListUp.Add(_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y - _neighbourOfNeighbourGem].MyGem);
                            }
                        }
                    }
                }
            }
            if (gemToChangeThePosition.MyPositionOnTheBoard.y + _neighbourGem < _maximumBoardLimit)
            {
                if (gemType.ToString() == _cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y + _neighbourGem].MyGem.MyType.ToString())
                {
                    if (_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y + _neighbourGem].MyPositionOnTheBoard != myActualPosition)
                    {
                        _predictMatchGemListDown.Add(_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y + _neighbourGem].MyGem);
                        if (gemToChangeThePosition.MyPositionOnTheBoard.y + _neighbourOfNeighbourGem < _maximumBoardLimit)
                        {
                            if (gemType.ToString() == _cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y + _neighbourOfNeighbourGem].MyGem.MyType.ToString())
                            {
                                _predictMatchGemListDown.Add(_cellGrid[(int)gemToChangeThePosition.MyPositionOnTheBoard.x, (int)gemToChangeThePosition.MyPositionOnTheBoard.y + _neighbourOfNeighbourGem].MyGem);
                            }
                        }
                    }
                }
            }

            if (CheckPredictiveCombination(_predictMatchGemListUp, _predictMatchGemListDown))
            {
                ClearPredictiveCellLists();
                return true;
            }

            ClearPredictiveCellLists();
            return false;
        }

        /// <summary>
        /// RepositioningGems check if have gems without gems under it and if theres send them down recursively
        /// </summary>
        private void RepositioningGems()
        {
            for (int x = _minimumBoardLimit; x < _row; x++)
            {
                for (int y = _minimumBoardLimit; y < _col; y++)
                {
                    if (y + _neighbourGem < _col)
                    {
                        if (_cellGrid[x, y] != null &&
                        _cellGrid[x, y + _neighbourGem] != null)
                        {
                            if (_cellGrid[x, y].MyGem != null &&
                            _cellGrid[x, y + _neighbourGem].MyGem == null)
                            {
                                _cellGrid[x, y + _neighbourGem].UpdateMyGem(_cellGrid[x, y].MyGem);
                                _cellGrid[x, y].DestroyGem();
                                x = _minimumBoardLimit;
                                y = _minimumBoardLimit;
                            }
                        }
                    }
                }
            }
            FilltheBoard();
        }

        /// <summary>
        /// CheckIfBoardHaveCombination check if the board have a match
        /// </summary>
        private bool CheckIfBoardHaveMatch()
        {
            for (int x = _minimumBoardLimit; x < _row; x++)
            {
                for (int y = _minimumBoardLimit; y < _col; y++)
                {
                    if (x + _neighbourGem < _maximumBoardLimit && x + _neighbourOfNeighbourGem < _maximumBoardLimit)
                    {
                        if (_cellGrid[x, y].MyGem.ToString() == _cellGrid[x + _neighbourGem, y].MyGem.ToString())
                        {
                            if (_cellGrid[x, y].MyGem.ToString() == _cellGrid[x + _neighbourOfNeighbourGem, y].MyGem.ToString())
                            {
                                return true;
                            }
                        }
                    }
                    if (x - _neighbourGem > _minimumBoardLimit && x - _neighbourOfNeighbourGem > _minimumBoardLimit)
                    {
                        if (_cellGrid[x, y].MyGem.ToString() == _cellGrid[x - _neighbourGem, y].MyGem.ToString())
                        {
                            if (_cellGrid[x, y].MyGem.ToString() == _cellGrid[x - _neighbourOfNeighbourGem, y].MyGem.ToString())
                            {
                                return true;
                            }
                        }
                    }
                    if (y + _neighbourGem < _maximumBoardLimit && y + _neighbourOfNeighbourGem < _maximumBoardLimit)
                    {
                        if (_cellGrid[x, y].MyGem.ToString() == _cellGrid[x, y + _neighbourGem].MyGem.ToString())
                        {
                            if (_cellGrid[x, y].MyGem.ToString() == _cellGrid[x, y + _neighbourOfNeighbourGem].MyGem.ToString())
                            {
                                return true;
                            }
                        }
                    }
                    if (y - _neighbourGem > _maximumBoardLimit && y - _neighbourOfNeighbourGem > _minimumBoardLimit)
                    {
                        if (_cellGrid[x, y].MyGem.ToString() == _cellGrid[x, y - _neighbourGem].MyGem.ToString())
                        {
                            if (_cellGrid[x, y].MyGem.ToString() == _cellGrid[x, y - _neighbourOfNeighbourGem].MyGem.ToString())
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// IsTheSameGem check if two gems are the same
        /// </summary>
        private bool IsTheSameGem(Gem gemA, Gem gemB)
        {
            if (gemA.MyPositionOnTheBoard.x == gemB.MyPositionOnTheBoard.x &&
                gemA.MyPositionOnTheBoard.y == gemB.MyPositionOnTheBoard.y)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// IsAdjacentCell check if two gems are adjacent to the other, but not diagonally
        /// </summary>
        private bool IsAdjacentCell(Gem gemA, Gem gemB)
        {
            if (gemA.MyPositionOnTheBoard.x == gemB.MyPositionOnTheBoard.x + _neighbourGem &&
                gemA.MyPositionOnTheBoard.y == gemB.MyPositionOnTheBoard.y)
            {
                return true;
            }
            else if (gemA.MyPositionOnTheBoard.x == gemB.MyPositionOnTheBoard.x - _neighbourGem &&
                gemA.MyPositionOnTheBoard.y == gemB.MyPositionOnTheBoard.y)
            {
                return true;
            }
            else if (gemA.MyPositionOnTheBoard.y == gemB.MyPositionOnTheBoard.y + _neighbourGem &&
                gemA.MyPositionOnTheBoard.x == gemB.MyPositionOnTheBoard.x)
            {
                return true;
            }
            else if (gemA.MyPositionOnTheBoard.y == gemB.MyPositionOnTheBoard.y - _neighbourGem &&
                gemA.MyPositionOnTheBoard.x == gemB.MyPositionOnTheBoard.x)
            {
                return true;
            }
            return false;
        }
    }
}