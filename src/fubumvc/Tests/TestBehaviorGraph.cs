using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;

namespace Tests
{
    public static class TestBehaviorGraph
    {
        public static BehaviorGraph Create()
        {
            var graph = new BehaviorGraph();
            graph.AddActionFor("api/action1/{input}/", typeof(Actions));
            return graph;
        }
    }

    public class Request
    {
        
    }

    public class Response
    {
        
    }

    public class Actions
    {
            
    }
}
