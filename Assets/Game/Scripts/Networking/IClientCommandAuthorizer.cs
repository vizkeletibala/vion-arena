namespace Vitrial.Networking
{
    /// <summary>
    /// Narrow validation seam for future server-authoritative command handling.
    /// Local prototype code can accept immediately; the dedicated server should replace this with
    /// validation that owns movement, combat, loot pickup, and inventory/equip decisions.
    /// </summary>
    public interface IClientCommandAuthorizer
    {
        AuthorityDecision ValidateClientCommand(string commandName, int playerSlot);
    }
}
