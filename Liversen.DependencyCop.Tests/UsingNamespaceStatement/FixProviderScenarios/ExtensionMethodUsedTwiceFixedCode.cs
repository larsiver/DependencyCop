using static UsingNamespaceStatementAnalyzer.Account.ItemExtensions;

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

    static class ItemExtensions
    {
        public static string GetName(this Item item) => item.Name;
    }
}

namespace UsingNamespaceStatementAnalyzer.Transaction
{
    class Current
    {
        public Account.Id[] AccountIds { get; set; }

        public Account.Item Item { get; set; }

        public string Text { get; set; }

        public string GetTheName() => Item.GetName();

        public string GetTheName2() => Item.GetName();
    }
}
