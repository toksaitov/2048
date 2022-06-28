namespace project_01_task_02
{
    class CellView
    {
        private static readonly SolidBrush CellBrush = new SolidBrush(Color.FromArgb(238, 228, 218));
        private static readonly SolidBrush CellFontBrush = new SolidBrush(Color.FromArgb(119, 110, 101));

        public int Cell { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public CellView(int cell, int x, int y)
        {
            Cell = cell;
            X = x;
            Y = y;
        }

        public void Paint(Graphics g, int cellSize, int ctrShiftX, int ctrShiftY, int pad, int cellCorRad)
        {
            int hPad = (int) (pad / 2.0f);
            int scrX = ctrShiftX + X * cellSize;
            int scrY = ctrShiftY + Y * cellSize;
            MainForm.FillRoundedRectangle(
                g, CellBrush,
                scrX + hPad, scrY + hPad,
                cellSize - pad, cellSize - pad,
                cellCorRad
            );

            string cellAsText = Cell.ToString();
            float digitCount = (cellAsText.Length - 1.0f) / 4.0f;
            float floatScale = 1.0f / (digitCount * 0.6f + 1.2f) - 0.2f;
            int fontSize = (int) (cellSize * 0.28f * floatScale);
            var cellFont = new Font(FontFamily.GenericMonospace, fontSize, FontStyle.Bold);

            var textSize = g.MeasureString(cellAsText, cellFont);
            g.DrawString(
                cellAsText,
                cellFont,
                CellFontBrush,
                scrX + cellSize / 2.0f - textSize.Width / 2.0f,
                scrY + cellSize / 2.0f - textSize.Height / 2.35f
            );
        }
    }
}
