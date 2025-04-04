
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

namespace UsingNamespaceStatementAnalyzer.Customer
{
    class Agreement
    {
        public string Id { get; set; }
    }
}

namespace UsingNamespaceStatementAnalyzer.Transaction
{
    class Current
    {
        public Account.Id AccountId { get; set; }

        public Account.Item Item { get; set; }

        public Customer.Agreement Agreement { get; set; }

        public string Text { get; set; }
    }
}