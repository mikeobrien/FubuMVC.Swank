using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Swank.Extensions;
using NSubstitute;
using NUnit.Framework;
using Should;
using System.Linq;

namespace Tests.ExtensionTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void should_get_resource_from_pattern()
        {
            var route = Substitute.For<IRouteDefinition>();
            route.Pattern.Returns("{Yada}/books/{Id}/categories/{CategoryId}/classification/{ClassId}");
            route.GetRouteResource().ShouldEqual("/books/categories/classification");

            route.Pattern.Returns("{Yada}");
            route.GetRouteResource().ShouldEqual("/");
        }

        [Test]
        public void should_return_null_when_no_resource_found()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionTests.EmbeddedResource").ShouldBeNull();
        }

        [Test]
        public void should_return_text_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionTests.EmbeddedTextResource").ShouldEqual("Some text yo!");
        }

        [Test]
        public void should_return_html_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionTests.EmbeddedHtmlResource").ShouldEqual("<b>Some html yo!</b>");
        }

        [Test]
        public void should_return_markdown_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionTests.EmbeddedMarkdownResource").ShouldEqual("<p><strong>Some markdown yo!</strong></p>");
        }

        [Test]
        [TestCase(typeof(int))]
        [TestCase(typeof(int?))]
        [TestCase(typeof(Dictionary<string, int>))]
        [TestCase(typeof(IDictionary<string, int>))]
        [TestCase(typeof(Dictionary<string, Dictionary<string, int>>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(IList<int>))]
        [TestCase(typeof(List<List<int>>))]
        public void should_unwrap_types(Type wrappedType)
        {
            wrappedType.UnwrapType().ShouldEqual(typeof(int));
        }

        private class Widgets : List<string> { }
        private class Widgets<T> : List<string> { }

        [Test]
        public void should_determine_if_an_type_is_a_list_type()
        {
            typeof(List<string>).IsListType().ShouldBeTrue();
            typeof(IList<string>).IsListType().ShouldBeTrue();
            typeof(Widgets).IsListType().ShouldBeFalse();
            typeof(Widgets<string>).IsListType().ShouldBeFalse();
        }

        [Test]
        public void should_determine_if_an_type_implements_a_list_type()
        {
            typeof(List<string>).ImplementsListType().ShouldBeTrue();
            typeof(IList<string>).ImplementsListType().ShouldBeFalse();
            typeof(Widgets).ImplementsListType().ShouldBeTrue();
            typeof(Widgets<string>).ImplementsListType().ShouldBeTrue();
        }

        [Test]
        public void should_determine_if_an_type_inherits_from_a_list()
        {
            typeof(List<string>).InheritsFromListType().ShouldBeFalse();
            typeof(IList<string>).InheritsFromListType().ShouldBeFalse();
            typeof(Widgets).InheritsFromListType().ShouldBeTrue();
            typeof(Widgets<string>).InheritsFromListType().ShouldBeTrue();
        }

        [Test]
        public void should_determine_if_a_type_is_a_list()
        {
            typeof(List<string>).IsList().ShouldBeTrue();
            typeof(IList<string>).IsList().ShouldBeTrue();
            typeof(Widgets).IsList().ShouldBeTrue();
            typeof(Widgets<string>).IsList().ShouldBeTrue();
        }

        [Test]
        public void should_return_types_list_interface()
        {
            typeof(List<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(IList<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(Widgets).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(Widgets<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
        }

        [Test]
        public void should_return_list_element_type()
        {
            typeof(List<string>).GetListElementType().ShouldEqual(typeof(string));
            typeof(IList<string>).GetListElementType().ShouldEqual(typeof(string));
            typeof(Widgets).GetListElementType().ShouldEqual(typeof(string));
            typeof(Widgets<string>).GetListElementType().ShouldEqual(typeof(string));
            typeof(string[]).GetListElementType().ShouldEqual(typeof(string));
            typeof(string).GetListElementType().ShouldBeNull();
        }

        [Test]
        public void should_outer_join()
        {
            var result = new[] {1, 2, 3}.OuterJoin(new[] {2, 3, 4}, x => x, (k, i) => i).ToList();

            result.Count.ShouldEqual(4);

            result[0].Count().ShouldEqual(1);
            result[0].First().ShouldEqual(1);

            result[1].Count().ShouldEqual(2);
            result[1].All(x => x == 2).ShouldBeTrue();

            result[2].Count().ShouldEqual(2);
            result[2].All(x => x == 3).ShouldBeTrue();

            result[3].Count().ShouldEqual(1);
            result[3].First().ShouldEqual(4);
        }

        class Node { 
            public Node Parent { get; set; } 
            public int Index { get; set; } 
        }

        [Test]
        public void should_traverse()
        {
            var tip = new Node { Index = 2, Parent = new Node { Index = 1, Parent = new Node { Index = 0 }}};
            var traversal = tip.Traverse(x => x.Parent).ToList();
            traversal.Count.ShouldEqual(3);
            traversal[0].Index.ShouldEqual(2);
            traversal[1].Index.ShouldEqual(1);
            traversal[2].Index.ShouldEqual(0);
        }

        class NodeWithChildren
        {
            public int Index { get; set; } 
            public NodeWithChildren Child { get; set; }
            public List<NodeWithChildren> Children { get; set; }
        }

        [Test]
        public void should_traverse_many()
        {
            var tip = new NodeWithChildren
            {
                Child = new NodeWithChildren { Index = 1, Child = new NodeWithChildren { Index = 2 } },
                Children = new List<NodeWithChildren>
                {
                    new NodeWithChildren { 
                        Index = 3, 
                        Child = new NodeWithChildren { Index = 4 },
                        Children = new List<NodeWithChildren>
                        {
                            new NodeWithChildren { Index = 5 },
                            new NodeWithChildren { Index = 6 }
                        }
                    },
                    new NodeWithChildren { 
                        Index = 7, 
                        Child = new NodeWithChildren { Index = 8 },
                        Children = new List<NodeWithChildren>
                        {
                            new NodeWithChildren { Index = 9 },
                            new NodeWithChildren { Index = 10 }
                        }
                    }
                }
            };

            var traversal = tip.TraverseMany(x =>
            {
                var results = new List<NodeWithChildren>();
                if (x.Child != null) results.Add(x.Child);
                if (x.Children != null) results.AddRange(x.Children);
                return results;
            }).OrderBy(x => x.Index).ToList();

            traversal.Count.ShouldEqual(10);
            for (var i = 0; i < 9; i++) traversal[i].Index.ShouldEqual(i + 1);
        }

        [Test]
        public void should_concat()
        {
            new List<string> { "oh" }.Concat("hai").ShouldEqual(new List<string> { "oh", "hai" });
        }

        [Test]
        public void should_concat_with_null_source()
        {
            ((List<string>)null).Concat("hai").ShouldEqual(new List<string> { "hai" });
        }

        [Test]
        [TestCase(typeof(String), "string")]
        [TestCase(typeof(Boolean), "boolean"), TestCase(typeof(Boolean?), "boolean")]
        [TestCase(typeof(Decimal), "decimal"), TestCase(typeof(Decimal?), "decimal")]
        [TestCase(typeof(Double), "double"), TestCase(typeof(Double?), "double")]
        [TestCase(typeof(Single), "float"), TestCase(typeof(Single?), "float")]
        [TestCase(typeof(Byte), "unsignedByte"), TestCase(typeof(Byte?), "unsignedByte")]
        [TestCase(typeof(SByte), "byte"), TestCase(typeof(SByte?), "byte")]
        [TestCase(typeof(Int16), "short"), TestCase(typeof(Int16?), "short")]
        [TestCase(typeof(UInt16), "unsignedShort"), TestCase(typeof(UInt16?), "unsignedShort")]
        [TestCase(typeof(Int32), "int"), TestCase(typeof(Int32?), "int")]
        [TestCase(typeof(UInt32), "unsignedInt"), TestCase(typeof(UInt32?), "unsignedInt")]
        [TestCase(typeof(Int64), "long"), TestCase(typeof(Int64?), "long")]
        [TestCase(typeof(UInt64), "unsignedLong"), TestCase(typeof(UInt64?), "unsignedLong")]
        [TestCase(typeof(DateTime), "dateTime"), TestCase(typeof(DateTime?), "dateTime")]
        [TestCase(typeof(TimeSpan), "duration"), TestCase(typeof(TimeSpan?), "duration")]
        [TestCase(typeof(Guid), "uuid"), TestCase(typeof(Guid?), "uuid")]
        [TestCase(typeof(Char), "char"), TestCase(typeof(Char?), "char")]
        [TestCase(typeof(Uri), "anyURI")]
        [TestCase(typeof(byte[]), "base64Binary")]
        [TestCase(typeof(int[]), "ArrayOfInt")]
        [TestCase(typeof(List<int>), "ArrayOfInt")]
        [TestCase(typeof(List<List<int>>), "ArrayOfArrayOfInt")]
        [TestCase(typeof(Dictionary<string, int>), "DictionaryOfInt")]
        [TestCase(typeof(Dictionary<string, Dictionary<string, int>>), "DictionaryOfDictionaryOfInt")]
        [TestCase(typeof(ArgumentException), "ArgumentException")]
        public void should_return_xml_name(Type type, string name)
        {
            type.GetXmlName(false).ShouldEqual(name);
        }

        [TestCase(typeof(ConsoleColor), true, "string"), TestCase(typeof(ConsoleColor?), true, "string")]
        [TestCase(typeof(ConsoleColor), false, "int"), TestCase(typeof(ConsoleColor?), false, "int")]
        public void should_return_enum_xml_name(Type type, bool enumAsString, string name)
        {
            type.GetXmlName(enumAsString).ShouldEqual(name);
        }

        enum SomeEnum { Oh, Hai }

        [Test]
        public void should_get_enum_values()
        {
            var values = typeof(SomeEnum).GetEnumOptions();
            values.Count().ShouldEqual(2);
            values[0].Name.ShouldEqual("Oh");
            values[1].Name.ShouldEqual("Hai");
        }

        [Test]
        public void should_get_nullable_enum_values()
        {
            var values = typeof(SomeEnum?).GetEnumOptions();
            values.Count().ShouldEqual(2);
            values[0].Name.ShouldEqual("Oh");
            values[1].Name.ShouldEqual("Hai");
        }

        public class MultipartKeyedType
        {
            public List<string> LongKey { get; set; }
            public List<string> ShortKey { get; set; }
        }

        [Test]
        public void should_shrink_multipart_key_right()
        {
            var types = new List<MultipartKeyedType>
            {
                new MultipartKeyedType { LongKey = new List<string> { "Request", "Properties", "List", "Form" } },
                new MultipartKeyedType { LongKey = new List<string> { "Request", "Properties", "Attributes", "List", "Form" } },
                new MultipartKeyedType { LongKey = new List<string> { "Request", "Properties", "Form" } },
                new MultipartKeyedType { LongKey = new List<string> { "Request", "Properties", "Form", "Form" } }
            };

            types.ShrinkMultipartKeyRight(x => x.LongKey, (t, k) => t.ShortKey = k);

            types[0].ShortKey.ToArray().ShouldEqual(new[] { "Properties", "List", "Form" });
            types[0].LongKey.ToArray().ShouldEqual(new[] { "Request", "Properties", "List", "Form" });

            types[1].ShortKey.ToArray().ShouldEqual(new[] { "Attributes", "List", "Form" });
            types[1].LongKey.ToArray().ShouldEqual(new[] { "Request", "Properties", "Attributes", "List", "Form" });

            types[2].ShortKey.ToArray().ShouldEqual(new[] { "Properties", "Form" });
            types[2].LongKey.ToArray().ShouldEqual(new[] { "Request", "Properties", "Form" });

            types[3].ShortKey.ToArray().ShouldEqual(new[] { "Form" });
            types[3].LongKey.ToArray().ShouldEqual(new[] { "Request", "Properties", "Form", "Form" });
        }

        public class KeyedType
        {
            public string Key { get; set; }
        }

        [Test]
        public void should_return_multiplicates()
        {
            var multiplicates = new List<KeyedType>
            {
                new KeyedType { Key = "Hai" },
                new KeyedType { Key = "Oh" },
                new KeyedType { Key = "Hai" }
            }.Multiplicates(x => x.Key);

            multiplicates.Count().ShouldEqual(2);
            multiplicates.All(x => x.Key == "Hai").ShouldBeTrue();
        }

        [Test]
        public void should_return_items_hash_code()
        {
            new List<string> { "hai" }.GetItemsHashCode()
                .ShouldEqual(new List<string> { "hai" }.GetItemsHashCode());
        }

        [Test]
        public void should_indicate_if_list_ends_with_value()
        {
            ((IEnumerable<string>)null).EndsWith("hai").ShouldBeFalse();
            new List<string>().EndsWith("hai").ShouldBeFalse();
            new List<string> { "oh" }.EndsWith("hai").ShouldBeFalse();
            new List<string> { "hai" }.EndsWith("hai").ShouldBeTrue();
        }

        [Test]
        public void should_shorten_list()
        {
            ((IEnumerable<string>)null).Shorten(1).ShouldBeNull();
            new List<string>().Shorten(1).ShouldBeEmpty();
            new List<string> { "oh" }.Shorten(1).ShouldBeEmpty();
            new List<string> { "oh" }.Shorten(2).ShouldBeEmpty();
            new List<string> { "oh", "hai" }.Shorten(1).ToArray().ShouldEqual(new [] {"oh"});
        }
    }
}