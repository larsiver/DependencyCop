
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

        public Transaction.Item[] Transactions { get; set; }
    }
}

namespace UsingNamespaceStatementAnalyzer.Transaction
{
    class Item
    {
        public Account.Id AccountId { get; set; }

        public string Text { get; set; }
    }
}
