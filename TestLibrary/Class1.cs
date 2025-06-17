namespace TestLibrary;

public class TestClass
{
    public static string GetFrameworkInfo()
    {
        var info = new System.Text.StringBuilder();
        info.AppendLine($"Running on: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        
#if NET8_OR_GREATER
        info.AppendLine("NET8_OR_GREATER is defined");
#endif

#if NET9_OR_GREATER
        info.AppendLine("NET9_OR_GREATER is defined");
#endif

        return info.ToString();
    }
}
