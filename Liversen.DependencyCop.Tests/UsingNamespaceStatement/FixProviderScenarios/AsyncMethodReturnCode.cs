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
        public async System.Threading.Tasks.Task<Id> GetAccountId()
        {
            await System.Threading.Tasks.Task.Delay(1);
            return new Id();
        }

        public Account.Item Item { get; set; }

        public string Text { get; set; }
    }
}
