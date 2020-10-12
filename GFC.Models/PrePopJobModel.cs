using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GFC.Models
{
    public class PrePopJobModel
    {
        public string JobID { get; set; }
        public string[] CacheName { get; set; }
        public PathFilter Filter { get; set; }
        public JobFrequencies Frequency { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }

        public PrePopJobModel()
        {
        }

        public PrePopJobModel(string jobid, string[] cacheName, PathFilter filter, JobFrequencies frequencies, string startTime, string stopTime)
        {
            JobID = jobid;
            CacheName = cacheName;
            Filter = filter;
            Frequency = frequencies;
            StartTime = startTime;
            StopTime = stopTime; ;
        }

        public PrePopJobModel(string jobid, string[] cacheName, PathFilter filter, string startTime, string stopTime)
        {
            JobID = jobid;
            CacheName = cacheName;
            Filter = filter;
            StartTime = startTime;
            StopTime = stopTime;
        }
    }

    public class PathFilter
    {
        public string UNCPath { get; set; }
        public uint Recursive { get; set; }
        public uint MetadataOnly { get; set; }
        public ExtensionType ExtensionFlag { get; set; }
        public string Extensions { get; set; }
        public ulong LastModifiedTime { get; set; }

        public PathFilter()
        {
        }

        public PathFilter(string uNCPath, string recursive, string metadataOnly, string extensionFlag, string extensions, string lastModifiedTime)
        {
            UNCPath = uNCPath;
            Recursive = Convert.ToUInt16(recursive);
            MetadataOnly = Convert.ToUInt16(metadataOnly);
            ExtensionFlag = (ExtensionType)Enum.Parse(typeof(ExtensionType), extensionFlag, true);
            Extensions = extensions;
            LastModifiedTime = Convert.ToUInt64(lastModifiedTime);
        }

        public PathFilter(string uNCPath, string recursive, string metadataOnly, string extensionFlag, string lastModifiedTime)
        {
            UNCPath = uNCPath;
            Recursive = Convert.ToUInt16(recursive);
            MetadataOnly = Convert.ToUInt16(metadataOnly);
            ExtensionFlag = (ExtensionType)Enum.Parse(typeof(ExtensionType), extensionFlag, true);
            Extensions = null;
            LastModifiedTime = Convert.ToUInt64(lastModifiedTime);
        }
    }

    public class JobFrequencies
    {
        public uint JobType { get; set; }
        public string Value { get; set; }
        public string ExecuteAt { get; set; }

        public JobFrequencies()
        {

        }
        public JobFrequencies(string type, string value, string executeAt)
        {
            JobType = Convert.ToUInt16(type);
            Value = value;
            ExecuteAt = executeAt;

        }
    }

}
