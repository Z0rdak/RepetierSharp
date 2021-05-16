using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public class ListPrinterCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.LIST_PRINTER;

        private ListPrinterCommand() { }

        public static ListPrinterCommand Instance => new ListPrinterCommand();
    }
}
