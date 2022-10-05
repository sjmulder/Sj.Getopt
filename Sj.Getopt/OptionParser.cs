namespace Sj.Getopt;

public abstract record OptionBase;
public abstract record OptionError(string Message) : OptionBase;

public record Flag(char Char) : OptionBase;
public record Option(char Char, string Argument) : OptionBase;
public record OptionMissingArgument(char Char)
    : OptionError("Missing argument -" + Char);
public record UnknownOption(char Char)
    : OptionError("Unknown option: -" + Char);

public class OptionParser
{
    readonly string[] _arguments;
    bool _done;
    int _arg; // current argument index
    int _pos; // position in current argument

    public OptionParser(string[] arguments)
    {
        _arguments = arguments;
    }

    public OptionBase? GetNext(string options)
    {
        if (_done)
            return null;
        
        while (_pos == 0)
        {
            if (_arg >= _arguments.Length ||        // end of arguments
                _arguments[_arg] == "-" ||          // '-' counts as positional
                !_arguments[_arg].StartsWith("-"))  // not an option
            {
                _done = true;
                return null;
            }
            
            // end of options flagged with '--'?
            if (_arguments[_arg] == "--")
            {
                _arg++;
                _done = true;
                return null;
            }
            
            _pos++; // skip '-'
        }

        char option = _arguments[_arg][_pos];
        int optionIdx = options.IndexOf(option);

        // proceed past option
        _pos++;
        if (_pos == _arguments[_arg].Length)
        {
            _arg++;
            _pos = 0;
        }

        if (optionIdx == -1)
            return new UnknownOption(option);
        if (optionIdx + 1 >= options.Length || options[optionIdx + 1] != ':')
            return new Flag(option);
        if (_arg == _arguments.Length)
            return new OptionMissingArgument(option);

        string optionArgument = _arguments[_arg].Substring(_pos);

        // proceed past option argument
        _arg++;
        _pos = 0;
                
        return new Option(option, optionArgument);
    }

    public bool IsDone => _done;

    public IEnumerable<string> RemainingArguments
    {
        get
        {
            if (_arg == _arguments.Length)
                return Enumerable.Empty<string>();
            if (_pos == 0)
                return _arguments.Skip(_arg);
            
            // argument partially processed, yield the rest
            return new[] {_arguments[_arg][_pos..]}
                .Concat(_arguments.Skip(_arg + 1));
        }
    }
}
