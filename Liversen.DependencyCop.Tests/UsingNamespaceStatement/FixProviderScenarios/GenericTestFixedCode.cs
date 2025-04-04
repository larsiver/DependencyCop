
namespace UsingNamespaceStatementAnalyzer.Account
{
    class Id<T>
    {
        public T Value { get; set; }
    }

    class Item
    {
        public Id<string> Id { get; set; }

        public string Name { get; set; }
    }
}

namespace UsingNamespaceStatementAnalyzer.Transaction
{
    class Current
    {
        public Account.Id<string> AccountId { get; set; }

        public Account.Item Item { get; set; }

        public string Text { get; set; }
    }
}
