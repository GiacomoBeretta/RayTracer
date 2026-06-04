namespace TracerLib;
/*
//dal python di Tomasi
public class InputStream
{
    private Stream _stream;
    private SourceLocation _location;
    private char? _savedChar;
    private SourceLocation _savedLocation;
    private int _tabulations;

    public InputStream(string filename, int tabulations)
    {
        _stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        _location = new SourceLocation(filename, 0, 0);
        _savedChar = null;
        _savedLocation = _location;
        _tabulations = tabulations;
    }

    public void UnreadChar(char c)
    {
        {
            _savedChar = c;
            _location = _savedLocation;
        }
    }

    public char? ReadChar()
    {
        char? c;
        if (_savedChar == null)
        {
            int b = _stream.ReadByte();
            if (b == -1) c = null;
            else c = (char)b;
        }
        else
        {
            c = _savedChar.Value;
            _savedChar = null;
        }

        _savedLocation = _location;
        UpdateLocation(c);
        return c;
    }

    //E se si raggiunge la fine del file che valore ha il char?
    public void UpdateLocation(char? c)
    {
        if (c.HasValue)
        {
            switch (c.Value)
            {
                case '\n':
                    _location.line += 1;
                    _location.column = 0;
                    break;
                case '\t':
                    _location.column += _tabulations;
                    break;
                default:
                    _location.column += 1;
                    break;
            }
        }
    }
}*/

//prova senza savedLocation
public class InputStream
{
    public Stream Stream;
    public SourceLocation Location;
    public SourceLocation Savedlocation;
    public char? SavedChar;
    public readonly int Tabulations;
    public Token? SavedToken;

    public InputStream(string filename, int tabulations = 8)
    {
        Stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        Location = new SourceLocation(filename, 0, 0);
        Savedlocation = Location;
        SavedChar = null;
        Tabulations = tabulations;
        SavedToken = null;
    }

    public void UnreadChar(char c)
    {
        {
            SavedChar = c;
            var savedLocation = Savedlocation;
            Location = savedLocation;
        }
    }

    public char? ReadChar()
    {
        char c;
        if (SavedChar == null)
        {
            int b = Stream.ReadByte();
            if (b == -1) return null; // if it has reached the end of file
            else
            {
                c = (char)b;
            }
        }
        else
        {
            c = SavedChar.Value;
            SavedChar = null;
        }
        var location = Location;
        Savedlocation = location;
        UpdateLocation(c);
        return c;
    }

    public void SkipLine()
    {
        var newLineChar = new List<char>
        {
            '\r',
            '\n'
        };

        char? ch;
        while (true)
        {
            ch = ReadChar();
            if (ch == null) return;
            if (newLineChar.Contains(ch.Value))
            {
                break;
            }
        }
    }

    public void SkipWhitespacesAndComments()
    {
        const string whitespace = " \t \n \r";
        var newLineChar = new List<char>
        {
            '\r',
            '\n'
        };

        var ch = ReadChar();

        if (ch == null) return;

        while (whitespace.Contains(ch.Value) || ch == '#')
        {
            if (ch == '#') SkipLine();
            ch = ReadChar();
            if (ch == null) return;
        }

        UnreadChar(ch.Value);
    }

    //Start Parse_token methods

    public StringToken _ParseStringToken(SourceLocation tokenLocation)
    {
        var token = "";

        while (true)
        {
            var ch = ReadChar();

            if (ch.HasValue && ch.Value == '"')
            {
                break;
            }
            else if (ch == null)
            {
                throw new GrammarError("unterminated string");
            }

            token += ch;
        }

        return new StringToken(tokenLocation, token);
    }

    public LiteralNumberToken _ParseFloatToken(string firstChar, SourceLocation tokenLocation)
    {
        var token = firstChar;
        const string exp = "eE";
        float value;

        while (true)
        {
            var ch = ReadChar();

            if (ch.HasValue && !float.TryParse(Convert.ToString(ch), out _) || ch.Value != '.' ||
                !exp.Contains(ch.Value))
            {
                UnreadChar(ch.Value);
                break;
            }

            token += ch;
        }

        try
        {
            value = float.Parse(token);
        }
        catch (Exception)
        {
            throw new GrammarError($"{token} is an invalid floating point number");
        }

        return new LiteralNumberToken(tokenLocation, value: value);
    }

    public Token _ParseKeywordIdentifierToken(string firstChar, SourceLocation tokenLocation)
    {
        var token = firstChar;

        while (true)
        {
            var ch = ReadChar();

            if (ch.HasValue && !char.IsLetterOrDigit(ch.Value) || ch.Value != '_')
            {
                UnreadChar(ch.Value);
                break;
            }

            token += ch;
        }

        if (Keywords.Map.TryGetValue(token, out var keyword))
        {
            return new KeywordToken(tokenLocation, keyword);
        }

        return new IdentifierToken(tokenLocation, token);
    }

    //End Parse_Token methods 

    public Token ReadToken()
    {
        const string symbol = "()<>[],*";
        const string op = "+-.";

        if (SavedToken != null)
        {
            var result = SavedToken;
            SavedToken = null;
            return result;
        }
            
        SkipWhitespacesAndComments();

        var ch = ReadChar();
        
        //mettere controllo eof

        var tokenLocation = Location;

        if (ch.HasValue && symbol.Contains(ch.Value))
        {
            return new SymbolToken(tokenLocation, ch.Value.ToString());
        }
        else if (ch == '\"')
        {
            return _ParseStringToken(tokenLocation);
        } 
        else if (ch.HasValue && char.IsDigit(ch.Value) || op.Contains(ch.Value))
        {
            return _ParseFloatToken(ch.Value.ToString(), tokenLocation);
        }
        else if (char.IsLetter(ch.Value) || ch.Value == '_') //???
        {
            return _ParseKeywordIdentifierToken(ch.Value.ToString(), tokenLocation);
        }
        else
        {
            throw new GrammarError($"invalid character {ch}");
        }
    }

    //E se si raggiunge la fine del file che valore ha il char?
    public void UpdateLocation(char c)
    {
        switch (c)
        {
            case '\n':
            case '\r':
                Location.line += 1;
                Location.column = 0;
                break;
            case '\t':
                Location.column += Tabulations;
                break;
            default:
                Location.column += 1;
                break;
        }
    }
}


/*

//prova con stream.Position e senza unreadChar
public class InputStream
{
    private Stream _stream;
    private SourceLocation _location;

   //public InputStream(Stream stream)
    {
        _stream = stream;
        _location = new SourceLocation();
    }

   // public InputStream(Stream stream, SourceLocation location)
    {
        _stream = stream;
        _location = location;
    }

   public InputStream(string fileName)
   {
       _stream = new  FileStream(fileName, FileMode.Open, FileAccess.Read);
       _location = new SourceLocation(fileName, 0,0);
   }

   public char ReadChar()
   {
       if (_savedChar == null)
       {
           return (char)_stream.ReadByte();
       }
       else
       {
           char c = _savedChar.Value;
           _savedChar = null;
           return c;
       }
   }
}*/