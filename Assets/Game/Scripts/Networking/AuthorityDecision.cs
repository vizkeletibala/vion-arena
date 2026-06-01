namespace Vitrial.Networking
{
    public readonly struct AuthorityDecision
    {
        public AuthorityDecision(bool accepted, string reason)
        {
            Accepted = accepted;
            Reason = reason;
        }

        public bool Accepted { get; }
        public string Reason { get; }

        public static AuthorityDecision Accept() => new AuthorityDecision(true, string.Empty);
        public static AuthorityDecision Reject(string reason) => new AuthorityDecision(false, reason);
    }
}
