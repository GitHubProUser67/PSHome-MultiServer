namespace CastleLibrary.SharpCompress
{
    public class SharpCompressException : Exception
    {
        public SharpCompressException() { }

        public SharpCompressException(string message)
            : base(message) { }

        public SharpCompressException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class ArchiveException(string message) : SharpCompressException(message);

    public class IncompleteArchiveException(string message) : ArchiveException(message);

    public class ExtractionException : SharpCompressException
    {
        public ExtractionException() { }

        public ExtractionException(string message)
            : base(message) { }

        public ExtractionException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class InvalidFormatException : ExtractionException
    {
        public InvalidFormatException() { }

        public InvalidFormatException(string message)
            : base(message) { }

        public InvalidFormatException(string message, Exception inner)
            : base(message, inner) { }
    }
}
