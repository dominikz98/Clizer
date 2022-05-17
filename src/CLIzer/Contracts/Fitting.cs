namespace CLIzer.Core.Contracts
{
    public enum Fitting
    {
        Border, Content, Custom
    }

    internal class Position
    {
        public int LX { get; }
        public int LY { get; }
        public int BX { get; }
        public int BY { get; }

        public Position(int lX, int lY, int bx, int by)
        {
            LX = lX;
            LY = lY;
            BX = bx;
            BY = by;
        }
    }
}
