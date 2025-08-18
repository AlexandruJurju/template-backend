namespace Template.Common.Constants.Aspire;

public static class Components
{
    public static readonly string RabbitMq = nameof(RabbitMq).ToLowerInvariant();
    public static readonly string MailPit = nameof(MailPit).ToLowerInvariant();
    public static readonly string Postgres = nameof(Postgres).ToLowerInvariant();
    public static readonly string KeyCloak = nameof(KeyCloak).ToLowerInvariant();
    public static readonly string Redis = nameof(Redis).ToLowerInvariant();
    public static readonly string Seq = nameof(Seq).ToLowerInvariant();
    public static readonly string MongoDb = nameof(MongoDb).ToLowerInvariant();

    public static class RelationalDbs
    {
        private const string Suffix = "pg";
        public static readonly string Template = $"{nameof(Template).ToLowerInvariant()}-{Suffix}";
    }

    public static class DocumentDbs
    {
        private const string Suffix = "mongo";
        public static readonly string Template = $"{nameof(Template).ToLowerInvariant()}-{Suffix}";
    }

    public static class Azure
    {
        public static readonly string Storage = nameof(Storage).ToLowerInvariant();
        public static readonly string BlobContainer = nameof(BlobContainer).ToLowerInvariant();
    }
}
