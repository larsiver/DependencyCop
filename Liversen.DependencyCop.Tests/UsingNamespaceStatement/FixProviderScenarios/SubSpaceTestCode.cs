using UsingNamespaceStatementAnalyzer.Account.SubSpace;

namespace UsingNamespaceStatementAnalyzer.Account
{
    class Id
    {
        public string Value { get; set; }
    }

    class Item
    {
        public Id Id { get; set; }

        public string Name { get; set; }
    }
}

namespace UsingNamespaceStatementAnalyzer.Account.SubSpace
{
    class AnotherClass
    {
        public string Value { get; set; }
    }
}

namespace UsingNamespaceStatementAnalyzer.Transaction
{
    class Current
    {
        public AnotherClass AccountId { get; set; }

        public Account.Item Item { get; set; }

        public string Text { get; set; }
    }
}
