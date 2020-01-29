namespace Clizer.Console.Services
{
    public class TestService : ITestService
    {
        public string GetMessage(int value, bool force) => "Running! Value:" + value + "; Force: " + force;
    }

    public interface ITestService
    {
        string GetMessage(int value, bool force);
    }
}
