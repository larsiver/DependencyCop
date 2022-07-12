namespace DescendantNamespaceAccessAnalyzer.Bank
{
    static class Info
    {
        public static string[] TableNames => new[] { Account.Info.TableName };
    }
}

namespace DescendantNamespaceAccessAnalyzer.Bank.Account
{
    static class Info
    {
        public static string TableName => "Account";
    }
}