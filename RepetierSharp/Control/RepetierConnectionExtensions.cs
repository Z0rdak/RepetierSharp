using System.Threading.Tasks;
using RepetierSharp.Models.Commands;

namespace RepetierSharp.Control
{
    public static class RepetierConnectionExtensions
    {
        /// <summary>
        ///     Activates a printer. Only an activated printer will try to connect to a printer if a port becomes visible.
        /// </summary>
        /// <param name="printerSlug"> Printer to activate </param>
        public static async Task ActivatePrinter(this RepetierConnection rc, string printerSlug)
        {
            await rc.SendServerCommand(new ActivateCommand(printerSlug));
        }

        /// <summary>
        ///     Deactivates a printer. Only activated printer will try to connect to a printer.
        ///     Deactivate it if you need to connect to the printer with a different program, e.g. for uploading a new firmware.
        /// </summary>
        /// <param name="printerSlug"> Printer to deactivate </param>
        public static async Task DeactivatePrinter(this RepetierConnection rc, string printerSlug)
        {
            await rc.SendServerCommand(new DeactivateCommand(printerSlug));
        }
        
        /// <summary>
        ///     Send a single "listPrinters" message to the repetier rerver.
        ///     The response to a "listPrinters" command contains the current print progress.
        /// </summary>
        public static async Task QueryPrinterList(this RepetierConnection rc)
        {
            await rc.SendServerCommand(ListPrinterCommand.Instance);
        }

        /// <summary>
        ///     Send a single "stateList" message to the repetier server.
        ///     The response to a "stateList" command contains information regarding the printer state.
        /// </summary>
        public static async Task QueryPrinterStateList(this RepetierConnection rc, bool includeHistory = false)
        {
            await rc.SendServerCommand(new StateListCommand(includeHistory));
        }

        /// <summary>
        ///     Logout current active session.
        /// </summary>
        /// <param name="rc"></param>
        public static async Task Logout(this RepetierConnection rc)
        {
            await rc.SendServerCommand(LogoutCommand.Instance);
        }

        public static async Task CreateUser(this RepetierConnection rc, string user, string password, int permission)
        {
            await rc.SendServerCommand(new CreateUserCommand(user, password, permission));
        }

        public static async Task UpdateUser(this RepetierConnection rc, string user, int permission,
            string password = "")
        {
            await rc.SendServerCommand(new UpdateUserCommand(user, permission, password));
        }

        public static async Task DeleteUser(this RepetierConnection rc, string user)
        {
            await rc.SendServerCommand(new DeleteUserCommand(user));
        }
    }
}
