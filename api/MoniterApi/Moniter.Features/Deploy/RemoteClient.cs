using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Renci.SshNet;

namespace Moniter.Features.Deploy
{
    public class RemoteClient : IRemoteClient
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public RemoteClient(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void Deploy(DeployInfo info)
        {
            try
            {
                var sshClient = new SshClient(info.Host, info.Port, info.UserName, info.Password);

                sshClient.Connect();
                var data = sshClient.RunCommand("ls /home/pi/Desktop");
                var arr = data.Result.Split('\n');
                if (arr.Contains("sp"))
                {
                    var rmCommand = sshClient.RunCommand("rm -rf /home/pi/Desktop/sp/");
                    Console.WriteLine(rmCommand.Result);
                }
                
                UploadSpCode(info, sshClient);
                var task = Task.Factory.StartNew(() =>
                {
                    sshClient.Disconnect();
                    sshClient.Dispose();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void UpdateConfig(DeployInfo info, ServerConfig newConfig)
        {
            using (var client = new SftpClient(info.Host, info.Port, info.UserName, info.Password))
            {
                client.Connect();

                var exists = client.Exists("/home/pi/Desktop/sp/config.json");
                if (exists)
                {
                    client.DeleteFile("/home/pi/Desktop/sp/config.json");
                }

                var json = JsonConvert.SerializeObject(newConfig);

                using (var fs = client.CreateText("/home/pi/Desktop/sp/config.json", Encoding.UTF8))
                {
                    fs.Write(json);
                }
            }
        }


        private void SaveUniqueCode(DeployInfo info, SftpClient sftpClient)
        {
            var config = new ServerConfig
            {
                FloorId = info.FloorId,
                MasterId = info.MasterId,
                Alias = info.ServerAlias
            };

            var json = JsonConvert.SerializeObject(config);

            using (var fs = sftpClient.CreateText("/home/pi/Desktop/sp/config.json", Encoding.UTF8))
            {
                fs.Write(json);
            }
        }

        private void UploadSpCode(DeployInfo info, SshClient sshClient)
        {
            var sftpClient = new SftpClient(info.Host, info.Port, info.UserName, info.Password);
            sftpClient.Connect();
            var path = Path.Combine(_hostingEnvironment.ContentRootPath, "MoniterPackage", "sp.zip");
            using (var fs = File.OpenRead(path))
            {
                sftpClient.UploadFile(fs, "/home/pi/Desktop/sp.zip");
                UnZip(info, sftpClient, sshClient);
            }
        }

        private void UnZip(DeployInfo info, SftpClient sftpClient, SshClient sshClient)
        {
            var lsCommand = sshClient.RunCommand("ls /home/pi/Desktop");
            var list = lsCommand.Result.Split('\n');
            if (list.Contains("sp.zip"))
            {
                var unzipCommand =
                    sshClient.RunCommand("unzip /home/pi/Desktop/sp.zip -d /home/pi/Desktop/");
                var rmZipCommand = sshClient.RunCommand("rm -rf /home/pi/Desktop/sp.zip");

                SaveUniqueCode(info, sftpClient);
            }
        }
    }
}