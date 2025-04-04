using UsingNamespaceStatementAnalyzer.Account;
using UsingNamespaceStatementAnalyzer.Customer;

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
        public Id AccountId { get; set; }

        public Item Item { get; set; }

        public Agreement Agreement { get; set; }

        public string Text { get; set; }
    }
}