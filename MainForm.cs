using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace project_01_task_02
{
    public partial class MainForm : Form
    {
        private Board board;
        private CellView[,] cellViews;

        public MainForm()
        {
            InitializeComponent();

            board = new Board();

            cellViews = new CellView[board.Size, board.Size];
            for (int y = 0; y < board.Size; y++)
            {
                for (int x = 0; x < board.Size; x++)
                {
                    int cell = board[x, y];
                    if (cell != 0)
                    {
                        cellViews[y, x] = new CellView(cell, x, y);
                    }
                }
            }

            board.CellCreated += (x, y) =>
            {
                // TODO
            };
            board.CellMoved += (fromX, fromY, toX, toY) =>
            {
                // TODO
            };
            board.CellsMerged += (fromX, fromY, toX, toY, mergedCell) =>
            {
                // TODO
            };
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            int cellSize = (int)(Math.Min(ClientSize.Width, ClientSize.Height) / (float)board.Size * 0.7);
            int boardScrSize = board.Size * cellSize;
            int ctrShiftX = (ClientSize.Width - boardScrSize) / 2;
            int ctrShiftY = (ClientSize.Height - boardScrSize) / 2;
            int boardCorRad = (int)(cellSize * 0.1f);
            int cellCorRad = (int)(cellSize * 0.04f);
            int pad = (int) (cellSize * 0.1f);
            int hPad = (int) (pad / 2.0f);

            var cellBackBrush = new SolidBrush(Color.FromArgb(189, 172, 159));
            FillRoundedRectangle(
                g, cellBackBrush,
                ctrShiftX - hPad, ctrShiftY - hPad,
                boardScrSize + pad, boardScrSize + pad,
                boardCorRad
            );

            var emptyCellBrush = new SolidBrush(Color.FromArgb(205, 192, 180));
            for (int y = 0; y < board.Size; y++)
            {
                for (int x = 0; x < board.Size; x++)
                {
                    int scrX = ctrShiftX + x * cellSize;
                    int scrY = ctrShiftY + y * cellSize;
                    FillRoundedRectangle(
                        g, emptyCellBrush,
                        scrX + hPad, scrY + hPad,
                        cellSize - pad, cellSize - pad,
                        cellCorRad
                    );
                }
            }

            for (int y = 0; y < board.Size; y++)
            {
                for (int x = 0; x < board.Size; x++)
                {
                    int cell = board[x, y];
                    if (cell != 0)
                    {
                        cellViews[y, x].Paint(g, cellSize, ctrShiftX, ctrShiftY, pad, cellCorRad);
                    }
                }
            }
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);

                return path;
            }

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        public static void FillRoundedRectangle(Graphics graphics, Brush brush, int x, int y, int width, int height, int cornerRadius)
        {
            using GraphicsPath path = RoundedRect(new Rectangle(x, y, width, height), cornerRadius);
            graphics.FillPath(brush, path);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    board.MoveUp();
                    Invalidate();
                    break;
                case Keys.Down:
                    board.MoveDown();
                    Invalidate();
                    break;
                case Keys.Left:
                    board.MoveLeft();
                    Invalidate();
                    break;
                case Keys.Right:
                    board.MoveRight();
                    Invalidate();
                    break;
            }
        }
    }
}