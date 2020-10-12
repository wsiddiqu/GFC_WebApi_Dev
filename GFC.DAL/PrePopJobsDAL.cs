using GFC.DAL.Interfaces;
using GFC.Models;
using GFC.Utility.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace GFC.DAL
{
    public class PrePopJobsDAL : IPrePopJobsDAL
    {
        XDocument xDoc;

        /// <summary>
        /// This method takes the PrePopJob parameters and create the job in xml format
        /// </summary>
        /// <param name="jobParams"></param>
        public void CreatePrePopJob(PrePopulationJob jobParams)
        {
            string startTime = string.Empty;
            string stopTime = string.Empty;
            if (jobParams.StartTime != null)
            {
                startTime = DateConversion(jobParams.StartTime);
            }
            if (jobParams.StopTime != null)
            {
                stopTime = DateConversion(jobParams.StopTime);
            }
            LoadXML();
            xDoc.Root.Element("Jobs").Add(new XElement("Job",
                        new XElement("JobID", jobParams.JobID),
                        new XElement("CacheNames", jobParams.CacheName.Select(l => new XElement("CacheName", l))),
                        new XElement("PathFilters", new XElement("UNCPath", jobParams.Filters.UNCPath),
                                                   new XElement("Recursive", jobParams.Filters.Recursive),
                                                   new XElement("MetadataOnly", jobParams.Filters.MetadataOnly),
                                                   new XElement("ExtensionFlag", (int)jobParams.Filters.ExtensionFlag),
                                                   (int)jobParams.Filters.ExtensionFlag == 0 ? null : new XElement("Extensions", jobParams.Filters.Extensions),
                                                   new XElement("LastModifiedTime", jobParams.Filters.LastModifiedTime)
                                                    ),
                        new XElement("Frequency",
                       new XElement("Type", jobParams.Frequency.JobType),
                       new XElement("Value", jobParams.Frequency.Value),
                       new XElement("ExecuteAt", jobParams.Frequency.ExecuteAt)),
                        new XElement("StartTime", startTime),
                        new XElement("StopTime", stopTime)
                         ));

            UpdateTimeStamp();
            SaveXML();
        }

        /// <summary>
        /// Method is used to convert the valid date to MMddyyyyhhmmss Format
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private string DateConversion(DateTime startTime)
        {
            string dateTime = string.Empty;
            dateTime = startTime.ToString("MMddyyyyhhmmss");
            return dateTime;
        }

        /// <summary>
        /// This method is used to delete all the PrePopJobs from the destination location
        /// </summary>
        public void DeleteAllPrePopJobs()
        {
            LoadXML();

            var jobs = xDoc.Root.Elements("Jobs").Descendants("Job");
            if (jobs != null)
            {
                jobs.Remove();

                UpdateTimeStamp();
                SaveXML();
            }
        }

        /// <summary>
        /// This Method is used to delete the job based on JobId
        /// </summary>
        /// <param name="jobId"></param>
        public void DeletePrePopJobDetails(string jobId)
        {
            LoadXML();

            var xmlJob = QueryXMLJobById(jobId, "[Delete-PrePopJobDetails] Failed to delete Pre-Population Job.");
            if (xmlJob != null)
            {
                xmlJob.Remove();

                UpdateTimeStamp();
                SaveXML();
            }
        }

        /// <summary>
        /// This Method is used to Get the PrePopjob based on JobId
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public PrePopJobModel GetPrePopJobDetails(string jobId)
        {
            PrePopJobModel prepopJob = new PrePopJobModel();
            LoadXML();
            try
            {
                var xmlJob = QueryXMLJobById(jobId, "[Get-PrePopJobDetails] Job details fetching failed.");
                if (xmlJob != null)
                {
                    prepopJob = GetJobDetails(xmlJob);
                }
                else
                {
                    throw new Exception("No Record Found For JobId : " + jobId);
                }
            }
            catch (System.Xml.XmlException ex)
            {
                throw new Exception("No Record Found For JobId:\nSystem Error:\n" + ex.Message);
            }
            finally
            {
                UnloadXML();
            }
            return prepopJob;
        }

        /// <summary>
        /// Method is used to get all the prepopjob list  
        /// </summary>
        /// <returns></returns>
        public List<PrePopJobModel> GetPrePopJobs()
        {
            List<PrePopJobModel> prepopJobs = new List<PrePopJobModel>();
            LoadXML();
            IEnumerable<XElement> xmljobs = xDoc.Root.Elements().Elements();
            foreach (var xmlJob in xmljobs)
            {
                PrePopJobModel objJob = GetJobDetails(xmlJob);
                prepopJobs.Add(objJob);
            }
            return prepopJobs;
        }

        /// <summary>
        /// Method is used to update the job
        /// </summary>
        /// <param name="jobParams"></param>
        public void UpdatePrePopJob(PrePopulationJob jobParams)
        {
            string startTime = string.Empty;
            string stopTime = string.Empty;
            if (jobParams.StartTime != null)
            {
                startTime = DateConversion(jobParams.StartTime);
            }
            if (jobParams.StopTime != null)
            {
                stopTime = DateConversion(jobParams.StopTime);
            }
            // Check if the policy already exists.
            LoadXML();
            var xmlJob = QueryXMLJobById(jobParams.JobID, "[Update-PrePopJobDetails] Failed to update Pre-Population Job.");
            if (xmlJob != null)
            {
                xmlJob.Element("PathFilters").Element("UNCPath").ReplaceNodes(jobParams.Filters.UNCPath.ToString());
                xmlJob.Element("StartTime").ReplaceNodes(startTime);
                xmlJob.Element("StopTime").ReplaceNodes(stopTime);
                xmlJob.Element("PathFilters").Element("Recursive").ReplaceNodes(jobParams.Filters.Recursive.ToString());
                xmlJob.Element("PathFilters").Element("MetadataOnly").ReplaceNodes(jobParams.Filters.MetadataOnly.ToString());

                if (jobParams.Filters.MetadataOnly != 0)
                {
                    xmlJob.Element("PathFilters").Element("LastModifiedTime").ReplaceNodes(0);
                    xmlJob.Element("PathFilters").Element("ExtensionFlag").ReplaceNodes((int)ExtensionType.NoVal);
                    if (xmlJob.Element("PathFilters").Element("Extensions") != null)
                        xmlJob.Element("PathFilters").Element("Extensions").Remove();
                }
                else
                {
                    xmlJob.Element("PathFilters").Element("LastModifiedTime").ReplaceNodes(jobParams.Filters.LastModifiedTime.ToString());
                    xmlJob.Element("PathFilters").Element("ExtensionFlag").ReplaceNodes((int)jobParams.Filters.ExtensionFlag);
                    if (xmlJob.Element("PathFilters").Element("Extensions") != null)
                    {
                        if ((int)jobParams.Filters.ExtensionFlag != 0)
                            xmlJob.Element("PathFilters").Element("Extensions").ReplaceNodes(jobParams.Filters.Extensions);
                        else
                            xmlJob.Element("PathFilters").Element("Extensions").Remove();
                    }
                    else
                    {
                        if (jobParams.Filters.ExtensionFlag != ExtensionType.NoVal)
                        {
                            xmlJob.Element("PathFilters").Add(new XElement("Extensions", jobParams.Filters.Extensions));
                        }
                    }

                }

                var FreqType = xmlJob.Element("Frequency").Element("Type");
                if (FreqType != null)
                {
                    xmlJob.Element("Frequency").Element("Type").ReplaceNodes(jobParams.Frequency.JobType.ToString());
                    xmlJob.Element("Frequency").Element("Value").ReplaceNodes(jobParams.Frequency.Value);
                    xmlJob.Element("Frequency").Element("ExecuteAt").ReplaceNodes(jobParams.Frequency.ExecuteAt);
                }

                UpdateTimeStamp();
                SaveXML();
            }
        }

        /// <summary>
        /// This Method is used to load the xml from the target location
        /// </summary>
		void LoadXML()
        {
            string jobFilePath = CommonServiceHelper.GetDefaultInstallDir() + "\\policies\\server\\" + Constants.jobsXMLName;
            if (File.Exists(jobFilePath))
            {
                if (xDoc != null)
                    xDoc = null;

                xDoc = XDocument.Load(jobFilePath);
            }
            else
            {
                xDoc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("PrePopJobs",
                                     new XAttribute("version", "1.0"),
                                     new XAttribute("lastUpdated", DateTime.UtcNow.ToString("s")),
                                     new XElement("Jobs")));
                xDoc.Save(jobFilePath);
            }
        }

        /// <summary>
        /// to save the prepoJob in xml format this method is used
        /// </summary>
        /// <param name="loadAfterSave"></param>
		void SaveXML(bool loadAfterSave = false)
        {
            string jobFilePath = CommonServiceHelper.GetDefaultInstallDir() + "\\policies\\server\\" + Constants.jobsXMLName;
            xDoc.Save(jobFilePath);

            if (loadAfterSave)
            {
                xDoc = null;
                xDoc = XDocument.Load(jobFilePath);
            }
        }

        /// <summary>
        /// This method parses the xml element to PrePopulationJob object.
        /// </summary>
        /// <param name="xmlJob"></param>
        /// <param name="cacheName"></param>
        /// <returns>object of PrepopulationJob</returns>
        PrePopJobModel GetJobDetails(XElement xmlJob)
        {
            PrePopJobModel objJob;
            PathFilter objPFilter;

            if (xmlJob.Element("PathFilters").Element("ExtensionFlag").Value == "0")
            {
                objPFilter = new PathFilter(xmlJob.Element("PathFilters").Element("UNCPath").Value,
                        xmlJob.Element("PathFilters").Element("Recursive").Value,
                        xmlJob.Element("PathFilters").Element("MetadataOnly").Value,
                        xmlJob.Element("PathFilters").Element("ExtensionFlag").Value,
                        xmlJob.Element("PathFilters").Element("LastModifiedTime").Value);
            }
            else
            {
                objPFilter = new PathFilter(xmlJob.Element("PathFilters").Element("UNCPath").Value,
                    xmlJob.Element("PathFilters").Element("Recursive").Value,
                    xmlJob.Element("PathFilters").Element("MetadataOnly").Value,
                    xmlJob.Element("PathFilters").Element("ExtensionFlag").Value,
                    xmlJob.Element("PathFilters").Element("Extensions").Value,
                    xmlJob.Element("PathFilters").Element("LastModifiedTime").Value);
            }


            JobFrequencies objFrequencies;

            objFrequencies = new JobFrequencies(xmlJob.Element("Frequency").Element("Type").Value,
                    xmlJob.Element("Frequency").Element("Value").Value,
                    xmlJob.Element("Frequency").Element("ExecuteAt").Value);




            IEnumerable<XElement> cacheNames = xmlJob.Elements("CacheNames").Elements("CacheName");
            string[] EdgeServers = new string[cacheNames.Count()];
            int i = 0;
            foreach (var xCacheName in cacheNames)
            {
                EdgeServers[i] = xCacheName.Value;
                i++;
            }


            objJob = new PrePopJobModel(xmlJob.Element("JobID").Value,
                EdgeServers,
                objPFilter,
                objFrequencies,
                xmlJob.Element("StartTime").Value,
                xmlJob.Element("StopTime").Value);

            return objJob;
        }

        /// <summary>
        /// This method updates the time when any changes occures in XML.
        /// </summary>
        void UpdateTimeStamp()
        {
            try
            {
                var attr = xDoc.Root.Attribute("lastUpdated");
                attr.SetValue(DateTime.UtcNow.ToString("s"));
            }
            catch
            {
                //Silent Exception
            }
        }

        /// <summary>
        /// Query XML by job id
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="jobId"></param>
        /// <param name="notFoundErrorMsg"></param>
        /// <returns></returns>
        XElement QueryXMLJobById(string jobId, string notFoundErrorMsg)
        {
            var xJob = xDoc.Root.Elements("Jobs").Descendants("Job").FirstOrDefault(r =>
                r.Element("JobID").Value == jobId.ToString());

            if (xJob == null)
            {

            }

            return xJob;
        }

        private void UnloadXML()
        {
            xDoc = null;
        }
    }
}
