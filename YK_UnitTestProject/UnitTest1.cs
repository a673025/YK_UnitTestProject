using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ICSharpCode.SharpZipLib.Zip;

namespace YK_UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fileList = new List<string>
            {
                "C:\\Users\\Administrator\\Desktop\\新建文件夹 (3)",
                "C:\\Users\\Administrator\\Desktop\\QQ图片20181221115547.jpg",
                "C:\\Users\\Administrator\\Desktop\\kh\\KH-webpage-ok22A-path_76.jpg",
                "C:\\Users\\Administrator\\Desktop\\kh\\logo.png",
            };
            var zipPath = "/1.zip";

        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateZipFiles(List<string> fileList, string zipPath)
        {
            zipPath = System.IO.Directory.GetCurrentDirectory() + zipPath;
            ZipOutputStream zipStream;
            using (zipStream = new ZipOutputStream(File.Create(zipPath)))
            {
                zipStream.SetComment("版本1.0");//压缩文件描述
                zipStream.SetLevel(6); //设置CompressionLevel，压缩比
                foreach (var f in fileList)
                {
                    ZipMultiFiles(f, zipStream);//压缩
                }
            }
        }

        /// <summary>
        /// 添加文件和文件夹进入zip文件流
        /// </summary>
        /// <param name="file"></param>
        /// <param name="zipStream"></param>
        /// <param name="lastName"></param>
        private void ZipMultiFiles(string file, ZipOutputStream zipStream, string lastName = "")
        {
            if (File.Exists(file))      //是文件，压缩  
            {
                FileStream streamReader = null;
                using (streamReader = File.OpenRead(file))
                {
                    //处理file,如果不处理，直接new ZipEntry(file)的话,压缩出来是全路径
                    //如C:\Users\RD\Desktop\image\001.xml，压缩包里就是C:\Users\RD\Desktop\image\001.xml
                    //如果处理为image\001.xml压缩出来就是image\001.xml（这样才跟用工具压缩的是一样的效果）
                    string path = Path.GetFileName(file);
                    if (lastName != "")
                    {
                        path = lastName + "\\" + path;
                    }
                    var zipEntry = new ZipEntry(path)
                    {
                        DateTime = DateTime.Now,
                        Size = streamReader.Length
                    };

                    zipStream.PutNextEntry(zipEntry);//压入
                    int sourceCount = 0;
                    byte[] buffer = new byte[4096 * 1024];
                    while ((sourceCount = streamReader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        zipStream.Write(buffer, 0, sourceCount);
                    }
                }
            }
            else//是文件夹，递归
            {
                string[] filesArray = Directory.GetFileSystemEntries(file);
                string folderName = Regex.Match(file, @"[^\/:*\?\”“\<>|,\\]*$").ToString();//获取最里一层文件夹
                if (lastName != "")
                {
                    folderName = lastName + "\\" + folderName;
                }
                if (filesArray.Length == 0)//如果是空文件夹
                {
                    var zipEntry = new ZipEntry(folderName + "/");//加/才会当作文件夹来压缩
                    zipStream.PutNextEntry(zipEntry);
                }
                foreach (string f in filesArray)
                {
                    ZipMultiFiles(f, zipStream, folderName);
                }
            }
        }
    }

}
