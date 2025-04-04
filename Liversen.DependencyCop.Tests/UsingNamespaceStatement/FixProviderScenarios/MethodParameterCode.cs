using UsingNamespaceStatementAnalyzer.Account;

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
        public void GetAccountId(Id id, out Item item)
        {
            item = new Item();
        }

        public Account.Item Item { get; set; }

        public string Text { get; set; }
    }
}