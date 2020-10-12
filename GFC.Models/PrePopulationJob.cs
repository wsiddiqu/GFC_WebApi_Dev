using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static GFC.Utility.Common.Helper;

namespace GFC.Models
{
    public enum ExtensionType { NoVal, Include, Exclude }
    public enum ModifiedIn { Minutes, Hours, Days }

    public class PrePopulationJob
    {
        public string JobID { get; set; }
        public string[] CacheName { get; set; }
        public PathFilters Filters { get; set; }
        public JobFrequency Frequency { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; } 
        public Boolean isFilterByModifiedTime { get; set; }
        public Int32 ModifiedBy { get; set; }
        public ModifiedIn ModifiedIn { get; set; }

        public PrePopulationJob()
        {
        }

        public PrePopulationJob(string jobid, string[] cacheName, PathFilters filters, JobFrequency frequency, DateTime startTime, DateTime stopTime)
        {
            JobID = jobid;
            CacheName = cacheName;
            Filters = filters;
            Frequency = frequency;
            StartTime = startTime;
            StopTime = stopTime;;
        }

        public PrePopulationJob(string jobid, string[] cacheName, PathFilters filters, DateTime startTime, DateTime stopTime)
        {
            JobID = jobid;
            CacheName = cacheName;
            Filters = filters;
            StartTime = startTime;
            StopTime = stopTime;
        }
    }

    public class PathFilters
    {
        public string UNCPath { get; set; }
        public uint Recursive { get; set; }
        public uint MetadataOnly { get; set; }
        public ExtensionType ExtensionFlag { get; set; }
        public string Extensions { get; set; }
        public ulong LastModifiedTime { get; set; }

        public PathFilters()
        {
        }

        public PathFilters(string uNCPath, string recursive, string metadataOnly, string extensionFlag, string extensions, string lastModifiedTime)
        {
            UNCPath = uNCPath;
            Recursive = Convert.ToUInt16(recursive);
            MetadataOnly = Convert.ToUInt16(metadataOnly);
            ExtensionFlag = (ExtensionType)Enum.Parse(typeof(ExtensionType), extensionFlag, true);
            Extensions = extensions;
            LastModifiedTime = Convert.ToUInt64(lastModifiedTime);
        }

        public PathFilters(string uNCPath, string recursive, string metadataOnly, string extensionFlag, string lastModifiedTime)
        {
            UNCPath = uNCPath;
            Recursive = Convert.ToUInt16(recursive);
            MetadataOnly = Convert.ToUInt16(metadataOnly);
            ExtensionFlag = (ExtensionType)Enum.Parse(typeof(ExtensionType), extensionFlag, true);
            Extensions = null;
            LastModifiedTime = Convert.ToUInt64(lastModifiedTime);
        }
    }

    public class JobFrequency
    {
        public uint JobType { get; set; }
        public string Value { get; set; }
        public string ExecuteAt { get; set; }

        public JobFrequency()
        {
            
        }
        public JobFrequency(string type, string value, string executeAt)
        {
            JobType = Convert.ToUInt16(type);
            Value = value;
            ExecuteAt = executeAt;

        }
    }

}
