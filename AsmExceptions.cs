using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LmcSimulator
{
    public abstract class AsmException: Exception
    {
        public AsmException() { }
        public AsmException(string message, int line, int word) : base($"Compilation failed on line {line}, word {word}: {message}") { }
    }
    public class AddressBoundsException : AsmException 
    {
        public AddressBoundsException(){ }
        public AddressBoundsException(string message, int line, int word) : base(message,line, word) { }
    }

    public class ProgramSizeException : AsmException 
    { 
        public ProgramSizeException() { }
        public ProgramSizeException(string message, int line,int word): base(message,line, word) { }
    }

    public class SyntaxException : AsmException 
    {
        public SyntaxException() { }
        public SyntaxException(string message, int line, int word) : base(message, line, word) { }
    }
    public class UndeclaredLabelException : AsmException
    {
        public UndeclaredLabelException() { }
        public UndeclaredLabelException(string message, int line, int word) : base(message, line, word) { }
    }
  
}
