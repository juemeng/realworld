using System;
using System.Reflection;
using Moniter.Models;

namespace Moniter.Features.Alert
{
    public class SaveResultEnvelope
    {
        public SaveResultEnvelope()
        {
            
        }
        
        public SaveResultEnvelope(Alert alertViewModel,SaveResult result)
        {
            AlertViewModel = alertViewModel;
            Result = result;
        }
        public bool Skipped { get; set; }
        public bool Authenticated { get; set; }
        public Alert AlertViewModel { get; set; }
        public SaveResult Result { get; set; }
    }
    
    public class SaveResult
    {
        public Guid AlertId { get; set; }
        public string Description { get; set; }
        public DateTime AlertTime { get; set; }
        public SaveAlertStatus Status{ get; set; }
        public string Message { get; set; }
    }
}