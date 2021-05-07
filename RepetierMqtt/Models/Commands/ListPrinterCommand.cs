using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public class ListPrinterCommand : ICommandData
    {
        public string CommandIdentifier => "listPrinter";

        private ListPrinterCommand() { }

        public static ListPrinterCommand Instance => new ListPrinterCommand();
    }
}
