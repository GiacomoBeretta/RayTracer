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
    private Stream _stream;
    private SourceLocation _location;
    private char? _savedChar;
    private readonly int _tabulations;

    public InputStream(string filename, int tabulations)
    {
        _stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        _location = new SourceLocation(filename, 0, 0);
        _savedChar = null;
        _tabulations = tabulations;
    }

    public void UnreadChar(char c)
    {
        {
            _savedChar = c;
        }
    }

    public char? ReadChar()
    {
        if (_savedChar == null)
        {
            int b = _stream.ReadByte();
            if (b == -1) return null; // if it has reached the end of file
            else
            {
                char c = (char)b;
                UpdateLocation(c);
                return c;
            }
        }
        else
        {
            char c = _savedChar.Value;
            _savedChar = null;
            return c;
        }
    }

    public void SkipWhitespacesAndComments()
    {
        const string whitespace = " \t\n\r";
        var ch = ReadChar();

        var c = new List<char>
        {
            '\r',
            '\n',
            Convert.ToChar("")
        };

        while (ch.HasValue && whitespace.Contains(ch.Value) || ch == '#')
        {
            if (ch == '#')
            {
                char? comment;
                while ((comment=ReadChar()).HasValue && !c.Contains(comment.Value))
                {
                    
                }
            }

            ch = ReadChar();
            //Chiedere a Tomasi se si può fare qualcosa coi nullable type
            if (ch == Convert.ToChar(""))
            {
                return;
            }
        }

        if (ch.HasValue)
        {
            UnreadChar(ch.Value);
        }
    }

    public Token ReadToken()
    {
        const string symbol = "()<>[],*";
        SkipWhitespacesAndComments();

        var ch = ReadChar();
        
        //mettere controllo eof

        var tokenLocation = _location;

        if (ch.HasValue && symbol.Contains(ch.Value))
        {
            return new SymbolToken(tokenLocation, ch.Value.ToString());
        }
        else if (ch == '"')
        {
            return new StringToken(tokenLocation, ch.Value.ToString());
        }

        return new IdentifierToken(tokenLocation, ch.Value.ToString());
    }
    
    //Aggiungere funzioni parse keyword

    //E se si raggiunge la fine del file che valore ha il char?
    public void UpdateLocation(char c)
    {
        switch (c)
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