using System;
using System.Collections.Generic;
using System.Xml.Serialization;
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
        [XmlArrayItem("Comment")]
        public List<string> Comments { get; set; }
        public List<int> Codes { get; set; }
        [XmlArrayItem("Approval")]
        public List<TransferApproval> Approvals { get; set; }
        public List<TransferApproval> PendingApprovals { get; set; }
    }

    [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
    public class TransferApproval
    {
        [Comments("In volutpat tortor quis mauris blandit non viverra tellus mollis. Nam ac fermentum augue.")]
        public DateTime Timestamp { get; set; }
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