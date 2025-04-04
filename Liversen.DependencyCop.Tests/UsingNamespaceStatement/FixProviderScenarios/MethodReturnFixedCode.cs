
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

namespace UsingNamespaceStatementAnalyzer.Transaction
{
    class Current
    {
        public Account.Id GetAccountId()
        {
            return new Account.Id();
        }

        public Account.Item Item { get; set; }

        public string Text { get; set; }
    }
}
