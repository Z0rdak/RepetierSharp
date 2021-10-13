using RepetierMqtt.Models;
using RepetierMqtt.Models.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Util
{
    public class CommandManager
    {
        public static int CallbackId { get => Next(); set { _callBackId = value; } }
        private static int _callBackId;

        // CallbackId -> Command
        private static Dictionary<int, string> CallbackMap = new Dictionary<int, string>();

        public static int Next()
        {
            if (_callBackId == int.MaxValue)
            {
                _callBackId = 1;
                return _callBackId;
            }
            return _callBackId += 1;
        }

        public static RepetierBaseCommand CommandWithId(ICommandData command, string printer = "")
        {
            var callbackId = Next();
            CallbackMap.Add(callbackId, command.CommandIdentifier);
            return new RepetierBaseCommand(command, printer, callbackId);
        }

        public static string CommandIdentifierFor(int callbackId)
        {
            if (CallbackMap.TryGetValue(callbackId, out var commandIdentifier))
            {
                return commandIdentifier;
            }
            else
            {
                return string.Empty;
            }
        }

         /// <summary>
        /// See: https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
