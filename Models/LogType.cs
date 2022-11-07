namespace RepetierSharp.Models
{
    /// <summary>
    /// Type indicates one of the following: 
    /// 1 : Commands
    /// 2 : ACK responses like ok, wait, temperature
    /// 4 : Other responses
    /// 8 : Non maskable messages
    /// </summary>
    public enum LogType
    {
        Commands = 1,
        ACK = 2,
        Others = 4,
        NonMaskable = 8
    }
}
