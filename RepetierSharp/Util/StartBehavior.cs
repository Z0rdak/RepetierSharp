namespace RepetierSharp.Util
{
    /// <summary>
    ///     See https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/direct?id=upload for reference.
    /// </summary>
    public enum StartBehavior
    {
        StartOnEmptyQueue = -1,
        InsertInQueue = 0,
        Autostart = 1
    }
}
