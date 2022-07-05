namespace CLIzer.Design.ProgressBar;

public class ProgressBarValue
{
    public int Count { get; set; }
    public int Max { get; set; }

    public ProgressBarValue(int count, int max)
    {
        Count = count;
        Max = max;
    }
}
