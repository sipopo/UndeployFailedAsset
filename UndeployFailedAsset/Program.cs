using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BranchVODWS;
//using vodBackendWS;
using System.Net;
using System.IO;
using System.Threading;



namespace UndeployFailedAsset
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            OssVodBranchWS VodMan = new OssVodBranchWS();

            try
            {
                VodMan.Credentials = new NetworkCredential("IPTVServices", "P23R@vor", "BRMSK");
                VodMan.Url = "http://78.107.199.132/ossVodBranchWS/branch.asmx";

                FailedResourceInformation frInform = new FailedResourceInformation(); 


                frInform = VodMan.GetFailedResourceInformation();

                sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Number of FAILED assets: " + frInform.AssetServerMaps.Count());


                int i = 0;
                foreach (AssetServerMap assetServerMap in frInform.AssetServerMaps)
                {
                                      
                    sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + i + " " + assetServerMap.ProviderAssetId + " " + assetServerMap.ProviderId);
                    Console.WriteLine(i + " " + assetServerMap.ProviderAssetId +" " + assetServerMap.ProviderId);
                    if (assetServerMap.ProviderId == "CatchUp")
                    {
                        i++;
                        Console.WriteLine("Undeploy " + assetServerMap.ProviderAssetId);
                        sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Undeploy " + assetServerMap.ProviderAssetId);

                        VodMan.AddClusterJob(assetServerMap.Title,
                            assetServerMap.ProviderAssetId,
                            assetServerMap.ProviderId,
                            assetServerMap.VServerDiskInformation.VServerInformation.ClusterId,
                            "",
                            "",
                            JobType.Delete,
                            DateTime.Now.AddMinutes(1),
                            null
                            );
                    }
                    Thread.Sleep(1000);
                    if (i > 50) break;
                }
               
                //Console.ReadLine();
           
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.ToString());
                sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Error " + e.ToString());
            }
            finally {
                string filePath = @".\Logfile_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                
                File.AppendAllText(filePath, sb.ToString());
                sb.Clear();
                Console.WriteLine("filepath " + filePath);
                //Console.ReadLine();
            }
        }
    }
}
