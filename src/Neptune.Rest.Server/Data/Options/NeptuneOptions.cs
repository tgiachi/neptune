using CommandLine;

namespace Neptune.Rest.Server.Data.Options;

public class NeptuneOptions
{
    [Option('r', "root", Required = false, HelpText = "Root directory for the server.")]
    public string RootDirectory { get; set; } = string.Empty;


    [Option('n', "show-header", Required = false, HelpText = "Show header in the console.")]
    public bool ShowHeader { get; set; } = true;

}
