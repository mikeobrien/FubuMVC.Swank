using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FubuMVC.Core;
using FubuMVC.Media.Projections;
using FubuMVC.Swank.Description;

namespace Tests.Specification.SpecificationService.TypeTests
{
    namespace ExcludedHandlers
    {
        public class Request {} public class Response {}
        public class PostHandler { public Response Execute(Request request) { return null; } }

        namespace InModule
        {
            public class Module : ModuleDescription { public Module() { Name = "Some Module"; } }
            public class Request { } public class Response { }
            public class PostHandler { public Response Execute(Request request) { return null; } }
        }

        namespace InResource
        {
            public class Request { } public class Response { }
            [Resource("Some Resource")]
            public class PostHandler { public Response Execute(Request request) { return null; } }
        }
    }

    namespace HiddenHandlers
    {
        public class HiddenRequest { } public class HiddenResponse { }
        [Hide]
        public class HiddenPostHandler { public HiddenResponse Execute(HiddenRequest request) { return null; } }

        public class Request { } public class Response { }
        public class PostHandler { public Response Execute(Request request) { return null; } }
    }

    namespace CyclicReferences
    {
        public class Node { public List<Node> Children { get; set; } }
        public class Request { public Node Tree { get; set; } }
        public class Response { public Node Tree { get; set; } }

        public class PostHandler { public Response Execute(Request request) { return null; } }
    }

    namespace HandlerVerb
    {
        public class GetRequest { } 
        public class GetHandler { public object Execute(GetRequest request) { return null; } }
        public class PostRequest { } 
        public class PostHandler { public object Execute(PostRequest request) { return null; } }
        public class PutRequest { } 
        public class PutHandler { public object Execute(PutRequest request) { return null; } }
        public class UpdateRequest { } 
        public class UpdateHandler { public object Execute(UpdateRequest request) { return null; } }
        public class DeleteRequest { } 
        public class DeleteHandler { public object Execute(DeleteRequest request) { return null; } }
    }

    namespace TypeEnumeration
    {
        public class Response { } public class Request { }
        public class PostHandler { public Response Execute(Request request) { return null; } }
        public class PutHandler { public Response Execute(Request request) { return null; } }

        public class ZeeResponse { } public class ARequest { } 
        public class UpdateHandler { public ZeeResponse Execute(ARequest request) { return null; } }
    }

    namespace TypeOrder
    {
        public class YourResponse { } public class MyRequest { }
        public class PostHandler { public YourResponse Execute(MyRequest request) { return null; } }
        public class AResponse { } public class ZeeRequest { }
        public class UpdateHandler { public AResponse Execute(ZeeRequest request) { return null; } }
    }

    namespace SameInputOutputType
    {
        public class Data { }
        public class PutHandler { public Data Execute(Data request) { return null; } }
    }

    namespace TypeDescription
    {
        public class Request { }
        [Comments("This is a nice response type.")]
        public class Response { }
        public class PostHandler { public Response Execute(Request request) { return null; } }
    }

    namespace RecursingTypes
    {
        public class PrinterClassId { }
        public class PrinterClass { public PrinterClassId ClassId { get; set; } }
        public class PrinterInfo 
        { 
            [Hide]
            public PrinterClass Class { get; set; }
        }
        public class Printer { public PrinterInfo Info { get; set; } }
        public class Request { public Printer Device { get; set; } }

        [Hide]
        public class DeviceClassId { }
        public class DeviceId {}
        public enum Options { Option }

        public class DeviceClass 
        { 
            public DeviceClassId ClassId { get; set; } 
            public Options Options { get; set; }
            public Uri Url { get; set; }
            public List<DeviceId> Ids { get; set; }
        }
        public class DeviceInfo { public DeviceClass Class { get; set; } }
        public class Device { public DeviceInfo Info { get; set; } }
        public class Response { public Device Device { get; set; } }
        public class PostHandler { public Response Execute(Request request) { return null; } }
    }

    namespace MemberEnumeration
    {
        public class Request
        {
            public Guid Id { get; set; }
            [QueryString]
            public int Sort { get; set; }
            public string UserAgent { get; set; }
            public string Name { get; set; }
            public DateTime Birthday { get; set; }
            [Hide]
            public string Code { get; set; }
            [XmlIgnore]
            public int Key { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
        }

        public class PutHandler { public Response Execute_Id(Request request) { return null; } }
    }

    namespace ProjectionMemberEnumeration
    {
        public class Model
        {
            public Guid Id { get; set; }
            public string Key { get; set; }
            public string Name { get; set; }
            public int Sort { get; set; }
            public string UserAgent { get; set; }
        }

        public class Request : Projection<Model>
        {
            public Request()
            {
                Value(x => x.Id);
                Value(x => x.Key);
            }
        }

        public class Response : Projection<Model>
        {
            public Response()
            {
                Value(x => x.Name);
                Value(x => x.Sort);
            }
        }

        public class PutHandler { public Response Execute_Id(Request request) { return null; } }
    }

    namespace MemberDescription
    {
        public enum Status
        {
            Inactive,
            [Description("Active yo!", "This is a very nice status.")]
            Active,
            [Comments("Very active yo!")]
            HyperActive
        }

        public class HyperDrive { }

        public class Request
        {
            [DefaultValue("John Joseph Dingleheimer Smith")]
            public string Name { get; set; }
            [Comments("This is da birfday yo.")]
            public DateTime? Birthday { get; set; }
            [XmlElement("R2D2")]
            public int C3P0 { get; set; }
            public HyperDrive Drive { get; set; }
            public List<int> Ids { get; set; }
            [XmlArrayItem("Id")]
            public List<int> IdsWithCustomItemName { get; set; }
            public List<HyperDrive> Drives { get; set; }
            [XmlArrayItem("Drive")]
            public List<HyperDrive> DrivesWithCustomItemName { get; set; }
            [DefaultValue(Status.Active)]
            public Status Status { get; set; }
            [DefaultValue(5), Optional]
            public int Id { get; set; }
            public Status? NullableStatus { get; set; }
            public int? NullableInt { get; set; }
        }

        public class Response { }

        public class PutHandler { public Response Execute(Request request) { return null; } }
        public class PostHandler { public Response Execute(List<int> request) { return null; } }
    }
}