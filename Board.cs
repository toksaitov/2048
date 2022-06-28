using System.Text;

namespace project_01_task_02
{
    class Board
    {
        class Coords
        {
            public int X { get; }
            public int Y { get; }

            public Coords(int x, int y)
            {
                X = x; Y = y;
            }
        }

        private const int DefaultSize = 4;
        private const int NewCellsAtStart = 2;
        private const int NewCellsAfterMove = 1;

        private const int Nowhere = 0;
        private const int Up = -1;
        private const int Down = 1;
        private const int Left = -1;
        private const int Right = 1;

        private int[,] cells;
        private int maxCell;

        public delegate void CellCreatedEventHandler(int x, int y);

        public event CellCreatedEventHandler CellCreated;

        public delegate void CellMovedEventHandler(int fromX, int fromY, int toX, int toY);

        public event CellMovedEventHandler CellMoved;

        public delegate void CellsMergedEventHandler(int fromX, int fromY, int toX, int toY, int mergedCell);

        public event CellsMergedEventHandler CellsMerged;

        public Board()
            : this(DefaultSize) { }

        public Board(int size)
            : this(new int[size, size])
        {
            CreateRandomCells(NewCellsAtStart);
        }

        public Board(int[,] cells)
        {
            this.cells = cells;

            maxCell = 0;
            for (int y = 0; y < cells.GetLength(0); y++)
            {
                for (int x = 0; x < cells.GetLength(1); x++)
                {
                    if (cells[y, x] > maxCell) maxCell = cells[y, x];
                }
            }
        }

        public int this[int x, int y]
        {
            get => cells[y, x];
            private set => cells[y, x] = value;
        }

        public int Size
        {
            get => cells.GetLength(0);
        }

        public int MaxCell
        {
            get => maxCell;
        }

        public bool HasMoreMoves()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int cell = this[x, y];
                    if (cell == 0 ||
                        AreCoordsInside(x, y - 1) && this[x, y - 1] == cell ||
                        AreCoordsInside(x + 1, y) && this[x + 1, y] == cell ||
                        AreCoordsInside(x, y + 1) && this[x, y + 1] == cell ||
                        AreCoordsInside(x - 1, y) && this[x - 1, y] == cell)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void MoveUp()
        {
            MoveUp(NewCellsAfterMove);
        }

        public void MoveUp(int cellsToCreate)
        {
            for (int x = 0; x < Size; x++)
            {
                MergeOnCol(x, Up);
                MoveOnCol(x, Up);
            }

            CreateRandomCells(cellsToCreate);
        }

        public void MoveDown()
        {
            MoveDown(NewCellsAfterMove);
        }

        public void MoveDown(int cellsToCreate)
        {
            for (int x = 0; x < Size; x++)
            {
                MergeOnCol(x, Down);
                MoveOnCol(x, Down);
            }

            CreateRandomCells(cellsToCreate);
        }

        public void MoveLeft()
        {
            MoveLeft(NewCellsAfterMove);
        }

        public void MoveLeft(int cellsToCreate)
        {
            for (int y = 0; y < Size; y++)
            {
                MergeOnRow(y, Left);
                MoveOnRow(y, Left);
            }

            CreateRandomCells(cellsToCreate);
        }

        public void MoveRight()
        {
            MoveRight(NewCellsAfterMove);
        }

        public void MoveRight(int cellsToCreate)
        {
            for (int y = 0; y < Size; y++)
            {
                MergeOnRow(y, Right);
                MoveOnRow(y, Right);
            }

            CreateRandomCells(cellsToCreate);
        }

        public override string ToString()
        {
            var result = new StringBuilder("");
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    result.Append(this[x, y]);
                    if (x < Size - 1) result.Append(" ");
                }
                result.Append("\n");
            }

            return result.ToString();
        }

        private void CreateRandomCells(int count)
        {
            var unoccupiedCells = new List<Coords>();
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (this[x, y] == 0) unoccupiedCells.Add(new Coords(x, y));
                }
            }

            var randomGen = new Random();
            for (int i = 0; i < count && unoccupiedCells.Count > 0; i++)
            {
                int randomIndex = randomGen.Next(unoccupiedCells.Count);
                Coords coords = unoccupiedCells[randomIndex];
                unoccupiedCells.RemoveAt(randomIndex);

                int newCell = randomGen.Next(4) == 3 ? 4 : 2;
                this[coords.X, coords.Y] = newCell;
                if (newCell > maxCell) maxCell = newCell;

                if (CellCreated != null)
                {
                    CellCreated(coords.X, coords.Y);
                }
            }
        }

        private void MergeOnCol(int x, int dy)
        {
            int mergeTarget = -1;
            if (dy < 0)
                for (int y = 0; y < Size; y++)
                    mergeTarget = DoMergeOnCol(x, y, mergeTarget);
            else
                for (int y = Size - 1; y >= 0; y--)
                    mergeTarget = DoMergeOnCol(x, y, mergeTarget);
        }

        private int DoMergeOnCol(int x, int y, int mergeTarget)
        {
            int cell = this[x, y];
            if (cell != 0)
            {
                if (mergeTarget == -1 || this[x, mergeTarget] != cell)
                {
                    mergeTarget = y;
                }
                else
                {
                    int mergedCell = this[x, mergeTarget] + cell;
                    this[x, mergeTarget] = mergedCell;
                    this[x, y] = 0;
                    if (mergedCell > maxCell)
                    {
                        maxCell = mergedCell;
                    }
                    mergeTarget = -1;

                    if (CellsMerged != null)
                    {
                        CellsMerged(x, y, x, mergeTarget, mergedCell);
                    }
                }
            }
            return mergeTarget;
        }

        private void MergeOnRow(int y, int dx)
        {
            int mergeTarget = -1;
            if (dx < 0)
                for (int x = 0; x < Size; x++)
                    mergeTarget = DoMergeOnRow(x, y, mergeTarget);
            else
                for (int x = Size - 1; x >= 0; x--)
                    mergeTarget = DoMergeOnRow(x, y, mergeTarget);
        }

        private int DoMergeOnRow(int x, int y, int mergeTarget)
        {
            int cell = this[x, y];
            if (cell != 0)
            {
                if (mergeTarget == -1 || this[mergeTarget, y] != cell)
                {
                    mergeTarget = x;
                }
                else
                {
                    int mergedCell = this[mergeTarget, y] + cell;
                    this[mergeTarget, y] = mergedCell;
                    this[x, y] = 0;
                    if (mergedCell > maxCell)
                    {
                        maxCell = mergedCell;
                    }
                    mergeTarget = -1;

                    if (CellsMerged != null)
                    {
                        CellsMerged(x, y, mergeTarget, y, mergedCell);
                    }
                }
            }
            return mergeTarget;
        }

        private void MoveOnCol(int x, int dy)
        {
            if (dy < 0)
                for (int y = 0; y < Size; y++)
                    Move(x, y, Nowhere, dy);
            else
                for (int y = Size - 1; y >= 0; y--)
                    Move(x, y, Nowhere, dy);
        }

        private void MoveOnRow(int y, int dx)
        {
            if (dx < 0)
                for (int x = 0; x < Size; x++)
                    Move(x, y, dx, Nowhere);
            else
                for (int x = Size - 1; x >= 0; x--)
                    Move(x, y, dx, Nowhere);
        }

        private void Move(int x, int y, int dx, int dy)
        {
            if (this[x, y] == 0) return;

            int fromX = x;
            int fromY = y;

            int nx = x + dx;
            int ny = y + dy;
            while (AreCoordsInside(nx, ny) && this[nx, ny] == 0)
            {
                this[nx, ny] = this[x, y];
                this[x, y] = 0;

                x = nx; y = ny;
                nx += dx; ny += dy;
            }

            if (fromX != x || fromY != y)
            {
                if (CellMoved != null)
                {
                    CellMoved(fromX, fromY, x, y);
                }
            }
        }

        private bool AreCoordsInside(int x, int y)
        {
            return x >= 0 && x < Size &&
                   y >= 0 && y < Size;
        }
    }
}
