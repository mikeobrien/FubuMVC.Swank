﻿using System;
using System.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description.EndpointConventionTests
{
    [TestFixture]
    public class Tests
    {
        private BehaviorGraph _graph;
        private EndpointConvention _endpointConvention;

        [SetUp]
        public void Setup()
        {
            _graph = Behavior.BuildGraph().AddActionsInThisNamespace();
            _endpointConvention = new EndpointConvention();
        }

        [Test]
        public void should_not_pull_description_from_embedded_resource_named_as_handler_when_handler_has_resource_attribute()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<ControllerResource.Controller>().ParentChain());
            description.Name.ShouldBeNull();
            description.Comments.ShouldBeNull();

            Assembly.GetExecutingAssembly().FindTextResourceNamed<ControllerResource.Controller>()
                .ShouldEqual("<p><strong>This is a resource</strong></p>");
        }

        [Test]
        public void should_use_attribute_description_over_embedded_description()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<AttributePriority.GetHandler>().ParentChain());
            
            description.Name.ShouldEqual("Some action name");
            description.Comments.ShouldEqual("Some action description");

            Assembly.GetExecutingAssembly().FindTextResourceNamed<AttributePriority.GetHandler>()
                .ShouldEqual("<p><strong>This is an endpoint</strong></p>");
        }

        [Test]
        public void should_not_set_handler_description_to_default_for_endpoint_with_no_description()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.NoDescriptionGetHandler>().ParentChain());
            description.Name.ShouldBeNull();
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_embedded_handler_text_description()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.HandlerDescription.EmbeddedDescriptionGetHandler>().ParentChain());
            description.Name.ShouldEqual("Some embedded handler name");
            description.Comments.ShouldEqual("<b>An embedded handler text description</b>");
        }

        [Test]
        public void should_set_handler_description()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.HandlerDescription.GetHandler>().ParentChain());
            description.Name.ShouldEqual("Some handler name");
            description.Comments.ShouldEqual("Some handler description");
        }

        [Test]
        public void should_set_handler_comments()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.HandlerDescription.AttrbuteCommentsGetHandler>().ParentChain());
            description.Name.ShouldBeNull();
            description.Comments.ShouldEqual("Some handler comments");
        }

        [Test]
        public void should_set_action_description()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.ActionDescription.GetHandler>().ParentChain());
            description.Name.ShouldEqual("Some action name");
            description.Comments.ShouldEqual("Some action description");
        }

        [Test]
        public void should_set_action_comments()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.ActionDescription.AttrbuteCommentsGetHandler>().ParentChain());
            description.Name.ShouldBeNull();
            description.Comments.ShouldEqual("Some action comments");
        }

        [Test]
        public void should_set_embedded_markdown_action_description()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.ActionDescription.EmbeddedDescriptionGetHandler>().ParentChain());
            description.Name.ShouldBeNull();
            description.Comments.ShouldEqual("<p><strong>An embedded action markdown description</strong></p>");
        }

        [Test]
        public void should_set_request_description_from_attribute()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.RequestDescription.AttributeGetHandler>().ParentChain());
            description.RequestComments.ShouldEqual("Some request description");
        }

        [Test]
        public void should_set_request_description_from_action_named_embedded_file()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.RequestDescription.EmbeddedDescriptionGetHandler>().ParentChain());
            description.RequestComments.ShouldEqual("<p><strong>An embedded action request markdown description</strong></p>");
        }

        [Test]
        public void should_set_request_description_from_handler_named_embedded_file()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.RequestDescription.EmbeddedHandlerDescriptionGetHandler>().ParentChain());
            description.RequestComments.ShouldEqual("<p><strong>An embedded handler request markdown description</strong></p>");
        }

        [Test]
        public void should_set_response_description_from_attribute()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.ResponseDescription.AttributeGetHandler>().ParentChain());
            description.ResponseComments.ShouldEqual("Some response description");
        }

        [Test]
        public void should_set_response_description_from_action_named_embedded_file()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.ResponseDescription.EmbeddedDescriptionGetHandler>().ParentChain());
            description.ResponseComments.ShouldEqual("<p><strong>An embedded action response markdown description</strong></p>");
        }

        [Test]
        public void should_set_response_description_from_handler_named_embedded_file()
        {
            var description = _endpointConvention.GetDescription(_graph.GetAction<EndpointDescriptions.ResponseDescription.EmbeddedHandlerDescriptionGetHandler>().ParentChain());
            description.ResponseComments.ShouldEqual("<p><strong>An embedded handler response markdown description</strong></p>");
        }
    }
}