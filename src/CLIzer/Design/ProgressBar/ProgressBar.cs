using CLIzer.Extensions;

namespace CLIzer.Design.ProgressBar
{
    public class ProgressBar
    {
        public string? Title { get; }
        public ConsoleColor Color { get; set; } = Console.ForegroundColor;

        private ConsolePointer _start;

        public ProgressBar() : this(null) { }

        public ProgressBar(string? title)
        {
            Title = title;
            _start = ConsolePointer.CreateByStart();
            Step(0, 100);
        }

        public void Step(int count, int max)
        {
            // set and store cursor position 
            var currentPosition = ConsolePointer.CreateByCurrent();
            Console.SetCursorPosition(_start.Left, _start.Top);

            // draw prefix
            var valueInPercent = max / 100 * count;
            var prefix = string.Empty;
            if (!string.IsNullOrWhiteSpace(Title))
                prefix += $" {Title}";

            prefix = $"{prefix} {valueInPercent,3}%";
            Console.Write(prefix);

            // get filled length in percent
            var valueLength = Console.WindowWidth - (_start.Left + prefix.Length) - 4;
            var filled = (int)((double)valueLength / 100 * valueInPercent);

            // draw bar
            Console.Write(" |");

            var filledValue = "".PadLeft(filled, '=');
            ConsoleExtensions.Write(filledValue, Color);

            var emptyValue = "".PadLeft(valueLength - filledValue.Length, '-');
            Console.Write(emptyValue);

            Console.Write("| ");

            // set cursor position
            if (currentPosition.Top == _start.Top)
                return;

            Console.SetCursorPosition(currentPosition.Left, currentPosition.Top);
        }
    }
}
