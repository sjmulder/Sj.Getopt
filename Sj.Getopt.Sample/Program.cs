using Sj.Getopt;

int verbosity = 0;

// base command takes just -v
var parser = new OptionParser(args);
while (!parser.IsDone)
{
    switch (parser.GetNext("v"))
    {
        case Flag('v'): verbosity++; break;
        
        case OptionError(var message):
            Console.Error.WriteLine(message);
            Environment.Exit(64);
            break;
    }
}

var rest = parser.RemainingArguments.ToArray();
if (rest.Length == 0)
{
    Console.Error.WriteLine("Usage:");
    Console.Error.WriteLine("  sample [-v] foo ...");
    Console.Error.WriteLine("  sample [-v] bar [-n] [-t type] ...");
    Environment.Exit(64);
}

// subcommands (each with options of their own)
switch (rest[0])
{
    case "foo":
        // 'foo' has no options, but parse to yield any 'unknown option' errors
        parser = new OptionParser(args);
        while (!parser.IsDone)
        {
            switch (parser.GetNext(""))
            {
                case OptionError(var message):
                    Console.Error.WriteLine(message);
                    Environment.Exit(64);
                    break;
            }
        }
        
        Console.WriteLine("Subcommand 'foo'");
        Console.WriteLine($" verbosity = {verbosity}");
        Console.WriteLine(" arguments = " +
            string.Join(" ", parser.RemainingArguments));
        break;
    
    case "bar":
        // 'bar' takes optional -t and -f options
        string type = "(none)";
        bool force = false;

        parser = new OptionParser(args);
        while (!parser.IsDone)
        {
            switch (parser.GetNext("ft:"))
            {
                case Flag('f'): force = true; break;
                case Option('t', var value): type = value; break;
                
                case OptionError(var message):
                    Console.Error.WriteLine(message);
                    Environment.Exit(64);
                    break;
            }
        }
        
        Console.WriteLine("Subcommand 'bar'");
        Console.WriteLine($" verbosity = {verbosity}");
        Console.WriteLine($" type = {type}");
        Console.WriteLine($" force = {force}");
        Console.WriteLine(" arguments = " +
            string.Join(" ", parser.RemainingArguments));
        break;
    
    default:
        Console.Error.WriteLine("Unknown subcommand: " + rest[0]);
        break;
}