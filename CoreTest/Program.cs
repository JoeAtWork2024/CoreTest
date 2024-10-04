using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace FileMover
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddNLog();
            });

            try
            {
                logger.Info("開始執行");
                logger.Info("取得路徑設定");
                var config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .Build();

                string fromDir = config["FileSettings:DirectoryFrom"];
                string toDir = config["FileSettings:DirectoryTo"];

                if(fromDir == null || toDir == null)
                {
                    logger.Info("路徑設定沒設");
                    return;
                }

                if (!Directory.Exists(fromDir))
                {
                    Console.WriteLine("來源路徑不存在");
                    logger.Info("來源路徑不存在");
                    return;
                }

                if (!Directory.Exists(toDir))
                {
                    Console.WriteLine("目標路徑不存在");
                    logger.Info("目標路徑不存在");
                    Directory.CreateDirectory(toDir);
                }
             
                try
                {
                    var files = Directory.GetFiles(fromDir);
                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(toDir, fileName);

                        File.Copy(file, destFile);
                        Console.WriteLine($"檔案複製成功: {fileName}");
                        logger.Info($"檔案複製成功: {fileName}");
                        File.Delete(Path.Combine(fromDir, fileName));
                        Console.WriteLine($"檔案刪除成功: {fileName}");
                        logger.Info($"檔案刪除成功: {fileName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"檔案搬移錯誤: {ex.Message}");
                    logger.Error($"檔案搬移錯誤: {ex.Message}");
                }
            }
            catch (Exception ex){
                Console.WriteLine($"錯誤: {ex.Message}");
                logger.Error($"錯誤: {ex.Message}");
            }
            logger.Info("結束");

        }
    }
}