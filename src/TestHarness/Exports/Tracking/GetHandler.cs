using System;
using System.Collections.Generic;
using FubuMVC.Swank.Description;

namespace TestHarness.Exports.Tracking
{
    public class TrackingRequest
    {
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public string TrackingId { get; set; }
    }

    [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
    public class TrackingEvent
    {
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public DateTime Timestamp { get; set; }
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public double Latitude { get; set; }
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public double Longitude { get; set; }
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public string Comments { get; set; }
    }

    public class AllGetHandler
    {
        [Description("Get Tracking Events", "In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        [ResponseComments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public List<TrackingEvent> Execute_TrackingId(TrackingRequest request)
        {
            return null;
        } 
    }
}