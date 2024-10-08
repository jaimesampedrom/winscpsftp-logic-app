//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
//------------------------------------------------------------

//using System.Text.Json.Serialization;
using  Newtonsoft.Json;

namespace winscp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Extensions.Workflows;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using WinSCP;

    /// <summary>
    /// Represents the winscpsftp flow invoked function.
    /// </summary>
    public class winscpsftp
    {
        private readonly ILogger<winscpsftp> logger;

        public winscpsftp(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<winscpsftp>();
        }

        /// <summary>
        /// Executes the logic app workflow.
        /// </summary>
        /// <param name="zipCode">The zip code.</param>
        /// <param name="temperatureScale">The temperature scale (e.g., Celsius or Fahrenheit).</param>
        [FunctionName("winscpsftp")]
        public string Run([WorkflowActionTrigger]string hostname, string username, string password, string fingerprint, string filePath)
        {
           
            this.logger.LogInformation("Starting winscpsftp...");
            try
            {
                
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = hostname, //"sftpstorageaccjsm.blob.core.windows.net",
                    UserName = username, //"sftpstorageaccjsm.jaimesampedro",
                    Password = password, //"4YiaYyU1g6g8a0mwdgaERAF1upaNwoRX",
                    SshHostKeyFingerprint = fingerprint //= "ecdsa-sha2-nistp256 256 7Lrxb5z3CnAWI8pr2LK5eFHwDCl/Gtm/fhgGwB3zscw"
                };
    
                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    Stream streamdownloaded =  session.GetFile(filePath,transferOptions);

                    var memoryStream = new MemoryStream();
                    streamdownloaded.CopyTo(memoryStream);

                    byte[] byteArray = memoryStream.ToArray();

                    string base64resp = Convert.ToBase64String(byteArray);

                    // Root jsoncs = new Root { Contenttype = "application/octet-stream", Content = base64resp };

                    // string jsonString = JsonConvert.SerializeObject(jsoncs);

                    // return jsoncs;

                    return base64resp;


                                                      
                }

                           
              
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                this.logger.LogInformation("There has been an exception executing the SFTP WinSCP action...");
                this.logger.LogInformation(e.Message.ToString());
               
                return e.InnerException.ToString();
            }
    }
          
        


        

        /// <summary>
        /// Represents the weather information for winscpsftp.
        /// </summary>
        public class SFTP
        {
            /// <summary>
            /// Gets or sets the zip code.
            /// </summary>
            public int HostName { get; set; }

            /// <summary>
            /// Gets or sets the current weather.
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// Gets or sets the low temperature for the day.
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Gets or sets the high temperature for the day.
            /// </summary>
            public string SshHostKeyFingerprint { get; set; }
        }
    }
}

// Root myDeserializedClass = JsonConverter.DeserializeObject<Root>(myJsonResponse);
    public class Root
    {
        [JsonProperty("$Content-Type")]
        public string Contenttype { get; set; }

        [JsonProperty("$Content")]
        public string Content { get; set; }
    }

