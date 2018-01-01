using System;
using System.IO;
using System.Text;

namespace Nice.Core.Log
{
    public class TextLogging : Logging, ILogging
    {
        #region Format
        private const string FORMAT_DATETIME = "yyyy-MM-dd HH:mm:ss.fff";
        private const string FORMAT_LOG_INF = "{0}==>{1}\r\n";
        private const string FORMAT_LOG_ERR = "{0}==>{1}\r\n";
        private const string FORMAT_LOG_EXC = "DateTime:{0}\r\nMessage:{1}\r\nStackTrace:{2}\r\n";
        private const string FORMAT_LOG_BIZ_EXC = "DateTime:{0}\r\nMessage:{1}\r\nStackTrace:{2}\r\n";
        private const string FORMAT_LOG_EXC_INF = "DateTime:{0}\r\nMessage:{1}\r\nStackTrace:{2}\r\nInfo:{3}\r\n";
        #endregion

        private string logDir = null;

        public void Create(string _logDir, params string[] bizSigns)
        {
            logDir = _logDir;
            CreateDirectoryRecursively(_logDir);
            string path = Path.Combine(logDir, TextLog.bizInfo);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(logDir, TextLog.bizError);
            if (!Directory.Exists(Path.Combine(path)))
                Directory.CreateDirectory(path);
            if (bizSigns == null || bizSigns.Length == 0) return;
            foreach (string bizFlag in bizSigns)
            {
                string logname = Path.Combine(logDir, bizFlag);
                if (!Directory.Exists(logname))
                    Directory.CreateDirectory(logname);
            }
        }

        public void Info(string info)
        {
            base.Write(new TextLog(LogEnum.Information, string.Format(FORMAT_LOG_INF, DateTime.Now.ToString(FORMAT_DATETIME), info)));
        }
        public void Error(Exception ex)
        {
            base.Write(new TextLog(LogEnum.Error, string.Format(FORMAT_LOG_EXC, DateTime.Now.ToString(FORMAT_DATETIME), ex.Message, ex.StackTrace)));
        }
        public void Error(Exception ex, string info)
        {
            base.Write(new TextLog(LogEnum.Error, string.Format(FORMAT_LOG_EXC_INF, DateTime.Now.ToString(FORMAT_DATETIME), ex.Message, ex.StackTrace, info)));
        }
        public void Info(string content, string biz)
        {
            base.Write(new TextLog(biz, string.Format(FORMAT_LOG_INF, DateTime.Now.ToString(FORMAT_DATETIME), content), TimeGranularity.Daily));
        }
        public void BizError(Exception ex, string biz)
        {
            base.Write(new TextLog(biz, string.Format(FORMAT_LOG_BIZ_EXC, DateTime.Now.ToString(FORMAT_DATETIME), ex.Message, ex.StackTrace), TimeGranularity.Daily));
        }
        public void BizError(Exception ex, string info, string biz)
        {
            base.Write(new TextLog(biz, string.Format(FORMAT_LOG_BIZ_EXC, DateTime.Now.ToString(FORMAT_DATETIME), ex.Message, ex.StackTrace), TimeGranularity.Daily));
        }

        public override void OnWrite(ILog log)
        {
            FileStream fs = null;
            try
            {
                fs = File.Open(log.GetFullname(logDir), FileMode.Append, FileAccess.Write);
                fs.Position = fs.Length;
                byte[] bytes = Encoding.UTF8.GetBytes(log.Content);
                fs.Write(bytes, 0, bytes.Length);
            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException)
            {
                DirecotoryNotFound(log);
            }
            catch (IOException) { }
            catch (Exception) { }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

            }
        }

        private void DirecotoryNotFound(ILog log)
        {
            string filename = log.GetFullname(logDir);
            string dir = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                    this.OnWrite(log);
                }
                catch (UnauthorizedAccessException) { }
                catch (IOException) { }
                catch (Exception) { }

            }
        }

        public static void CreateDirectoryRecursively(string path)
        {
            string[] pathParts = path.TrimEnd('\\').Split('\\');
            for (var i = 0; i < pathParts.Length; i++)
            {
                if (i == 0 && pathParts[i].Contains(":"))
                {
                    pathParts[i] = pathParts[i] + "\\";
                }
                else if (i == pathParts.Length - 1 && pathParts[i].Contains("."))
                {
                    return;
                }
                if (i > 0)
                {
                    pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);
                }
                if (!Directory.Exists(pathParts[i]))
                {
                    Directory.CreateDirectory(pathParts[i]);
                }
            }
        }
    }
}
