namespace Vitrial.Networking
{
    /// <summary>
    /// Milestone 1 fallback authorizer: accepts local input while preserving the call shape that a
    /// headless server can make authoritative later.
    /// </summary>
    public sealed class LocalPrototypeAuthorizer : IClientCommandAuthorizer
    {
        public AuthorityDecision ValidateClientCommand(string commandName, int playerSlot)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                return AuthorityDecision.Reject("Command name is required.");
            }

            if (playerSlot < 0)
            {
                return AuthorityDecision.Reject("Player slot must be non-negative.");
            }

            return AuthorityDecision.Accept();
        }
    }
}
