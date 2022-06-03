namespace CLIzer.Design.Tables
{
    public class TablePointer
    {
        public int Left { get; set; }
        public int Top { get; set; }

        public TablePointer(int left, int top)
        {
            Left = left;
            Top = top;
        }
    }
}
