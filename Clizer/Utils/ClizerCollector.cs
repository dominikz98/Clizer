using System.ComponentModel;

namespace Clizer.Utils
{
    public static class ClizerCollector
    {
        //public static TData GetValidInput<TData>(string prefix, List<ValidationAttribute> validations)
        //{
        //    var isnullable = Nullable.GetUnderlyingType(typeof(TData)) != null;
        //    prefix += $"{(!isnullable ? "*" : string.Empty)}: ";
        //    TData value;
        //    bool valid;

        //    do
        //    {
        //        Console.Write(prefix);
        //        var input = Console.ReadLine();
        //        string? errormessage = null;

        //        if (string.IsNullOrEmpty(input) && !GenericTryParse(input, out value) && isnullable)
        //            valid = true;
        //        else
        //        {
        //            valid = true;
        //            foreach (var validation in validations)
        //                if (!validation.IsValid(value))
        //                {
        //                    valid = false;
        //                    errormessage = validation.ErrorMessage;
        //                    break;
        //                }
        //        }

        //        if (valid)
        //            ConsoleExtensions.AppendToLine(ConsoleColor.Green, prefix.Length + (input?.Length ?? 0), " ✓");
        //        else
        //            ConsoleExtensions.AppendToLine(ConsoleColor.Red, prefix.Length + (input?.Length ?? 0), $" X ({errormessage ?? "invalid"})");

        //    } while (!valid);

        //    return value;
        //}

        private static bool GenericTryParse<TData>(string input, out TData value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(TData));

            if (converter != null && converter.IsValid(input))
            {
                value = (TData)converter.ConvertFromString(input);
                return true;
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            value = default;
#pragma warning restore CS8601 // Possible null reference assignment.
            return false;
        }
    }
}
