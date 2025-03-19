using System;

namespace RepetierSharp.Internal
{
    public abstract class PrinterEventArgs : EventArgs
    {
        public PrinterEventArgs(string printer)
        {
            Printer = printer;
        }
        public string Printer { get; }
    }
}
