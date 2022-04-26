using System.Collections.Immutable;
using Shouldly;
using Xunit;

namespace Liversen.DependencyCop
{
    public static class DagTest
    {
        [Fact]
        static void GivenNoVertices_WhenAddingVertex_ThenVertexAdded()
        {
            var sut = new Dag();

            sut.AddVertex("A", "B");

            sut.DirectVertices().Count.ShouldBe(2);
            sut.DirectVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.DirectVertices()["B"].ShouldBeEmpty();
            sut.TransitiveVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.TransitiveVertices()["B"].ShouldBeEmpty();
        }

        [Fact]
        static void GivenSingleVertex_WhenTryAddingOppositeVertex_ThenCircle()
        {
            var sut = new Dag();
            sut.AddVertex("A", "B");

            var circle = sut.TryAddVertex("B", "A");

            sut.DirectVertices().Count.ShouldBe(2);
            sut.DirectVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.DirectVertices()["B"].ShouldBeEmpty();
            sut.TransitiveVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.TransitiveVertices()["B"].ShouldBeEmpty();
            circle.ShouldBe(ImmutableList.Create("A", "B", "A"));
        }

        [Fact]
        static void GivenTwoConnectedVerticesAddedInConnectedOrder_WhenTryAddingCircle_ThenCircle()
        {
            var sut = new Dag();
            sut.AddVertex("C", "A");
            sut.AddVertex("A", "B");

            var circle = sut.TryAddVertex("B", "C");

            sut.DirectVertices().Count.ShouldBe(3);
            sut.DirectVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.DirectVertices()["B"].ShouldBeEmpty();
            sut.DirectVertices()["C"].ShouldBe(ImmutableHashSet.Create("A"), true);
            sut.TransitiveVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.TransitiveVertices()["B"].ShouldBeEmpty();
            sut.TransitiveVertices()["C"].ShouldBe(ImmutableHashSet.Create("A", "B"), true);
            circle.ShouldBe(ImmutableList.Create("C", "A", "B", "C"));
        }

        [Fact]
        static void GivenTwoConnectedVerticesAddedInDisconnectedOrder_WhenTryAddingCircle_ThenCircle()
        {
            var sut = new Dag();
            sut.AddVertex("A", "B");
            sut.AddVertex("C", "A");

            var circle = sut.TryAddVertex("B", "C");

            sut.DirectVertices().Count.ShouldBe(3);
            sut.DirectVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.DirectVertices()["B"].ShouldBeEmpty();
            sut.DirectVertices()["C"].ShouldBe(ImmutableHashSet.Create("A"), true);
            sut.TransitiveVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.TransitiveVertices()["B"].ShouldBeEmpty();
            sut.TransitiveVertices()["C"].ShouldBe(ImmutableHashSet.Create("A", "B"), true);
            circle.ShouldBe(ImmutableList.Create("C", "A", "B", "C"));
        }

        [Fact]
        static void GivenThreeConnectedVerticesAddedInDisconnectedOrder_WhentryAddingCircle_ThenCircle()
        {
            var sut = new Dag();
            sut.AddVertex("A", "B");
            sut.AddVertex("C", "D");
            sut.AddVertex("D", "A");

            var circle = sut.TryAddVertex("B", "C");

            sut.DirectVertices().Count.ShouldBe(4);
            sut.DirectVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.DirectVertices()["B"].ShouldBeEmpty();
            sut.DirectVertices()["C"].ShouldBe(ImmutableHashSet.Create("D"), true);
            sut.DirectVertices()["D"].ShouldBe(ImmutableHashSet.Create("A"), true);
            sut.TransitiveVertices()["A"].ShouldBe(ImmutableHashSet.Create("B"), true);
            sut.TransitiveVertices()["B"].ShouldBeEmpty();
            sut.TransitiveVertices()["C"].ShouldBe(ImmutableHashSet.Create("A", "B", "D"), true);
            sut.TransitiveVertices()["D"].ShouldBe(ImmutableHashSet.Create("A", "B"), true);
            circle.ShouldBe(ImmutableList.Create("C", "D", "A", "B", "C"));
        }
    }
}
