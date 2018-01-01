using System;
using System.IO;

namespace Nice.Core.Log
{
    public class TextLog : ILog
    {
        #region 业务标识常量
        internal const string bizError = "errorlog";
        internal const string bizInfo = "infolog";
        internal const string bizWarning = "warning";
        internal const string bizSuccess = "success";
        internal const string bizDefault = "log";
        #endregion

        //private string fileName;
        private string content;
        private DateTime lastWriteTime;
        private string bizSign;
        private LogEnum logType;
        private TimeGranularity timeGranularity;
        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        /// <summary>
        /// 业务标识
        /// </summary>
        public string BizSign
        {
            get { return bizSign; }
            set { bizSign = value; }
        }
        public LogEnum LogType
        {
            get { return logType; }
            set { logType = value; }
        }
        public TimeGranularity TimeGranularity
        {
            get { return timeGranularity; }
            set { timeGranularity = value; }
        }
        public DateTime LastWriteTime
        {
            get { return lastWriteTime; }
            set { lastWriteTime = value; }
        }

        public TextLog(string _content)
        {
            logType = LogEnum.Error;
            timeGranularity = TimeGranularity.Daily;
            content = _content;
            lastWriteTime = DateTime.Now;
        }

        public TextLog(LogEnum _logType, string _content)
        {
            logType = _logType;
            timeGranularity = TimeGranularity.Daily;
            content = _content;
            lastWriteTime = DateTime.Now;
        }

        public TextLog(LogEnum _logType, TimeGranularity _timeGranularity, string _content)
        {
            logType = _logType;
            timeGranularity = _timeGranularity;
            content = _content;
            lastWriteTime = DateTime.Now;
        }
        /// <summary>
        /// 自定义业务日志
        /// </summary>
        /// <param name="biz">业务标识,如:登录-Login</param>
        /// <param name="_content">内容</param>
        /// <param name="_timeGranularity">时间粒度</param>
        public TextLog(string biz, string _content, TimeGranularity _timeGranularity)
        {
            this.logType = LogEnum.Customize;
            this.bizSign = biz;
            this.timeGranularity = _timeGranularity;
            this.content = _content;
            this.lastWriteTime = DateTime.Now;
        }

        public string GetFullname(string logDir)
        {
            string dir = null;

            switch (logType)
            {
                case LogEnum.Error:
                    dir = Path.Combine(logDir, bizError);
                    break;
                case LogEnum.Information:
                    dir = Path.Combine(logDir, bizInfo);
                    break;
                case LogEnum.Warning:
                    dir = Path.Combine(logDir, bizWarning);
                    break;
                case LogEnum.Success:
                    dir = Path.Combine(logDir, bizSuccess);
                    break;
                case LogEnum.Customize:
                    dir = Path.Combine(logDir, this.bizSign);
                    break;
                default:
                    dir = Path.Combine(logDir, bizDefault);
                    break;
            }

            string name = "log_";
            switch (timeGranularity)
            {
                case TimeGranularity.Daily:
                    name += lastWriteTime.ToString("yyyy-MM-dd");
                    break;
                case TimeGranularity.Weekly:
                    name += lastWriteTime.ToString("yyyy-MM-dd");
                    break;
                case TimeGranularity.Monthly:
                    name += lastWriteTime.ToString("yyyy-MM");
                    break;
                case TimeGranularity.Annually:
                    name += lastWriteTime.ToString("yyyy");
                    break;
                case TimeGranularity.Hour:
                    name += lastWriteTime.ToString("yyyy-MM-dd HH");
                    break;
                case TimeGranularity.Min10:
                    name += lastWriteTime.ToString("yyyy-MM-dd HHmm").Substring(0, 14) + "0";
                    break;
                default:
                    name += lastWriteTime.ToString("yyyy-MM-dd");
                    break;
            }

            return Path.Combine(dir, name + ".txt");
        }

        public override string ToString()
        {
            return content;
        }
    }


    /// <summary>
    /// 日志枚举类型,1-普通信息,2-警告,3-错误,4-成功,5-自定义
    /// </summary>
    public enum LogEnum
    {

        /// <summary>
        /// 指示普通信息类型的日志记录
        /// </summary>
        Information = 1,

        /// <summary>
        /// 指示警告信息类型的日志记录
        /// </summary>
        Warning = 2,

        /// <summary>
        /// 指示错误信息类型的日志记录
        /// </summary>
        Error = 3,

        /// <summary>
        /// 指示成功信息类型的日志记录
        /// </summary>
        Success = 4,
        /// <summary>
        /// 自定义
        /// </summary>
        Customize = 5,
    }
    /// <summary>
    /// 日志时间粒度，1-天,2-周,3-月,4-年,5-小时,6-10分钟
    /// </summary>
    public enum TimeGranularity
    {
        /// <summary>
        /// 天
        /// </summary>
        Daily = 1,

        /// <summary>
        /// 周
        /// </summary>
        Weekly = 2,

        /// <summary>
        /// 月
        /// </summary>
        Monthly = 3,

        /// <summary>
        /// 年
        /// </summary>
        Annually = 4,
        /// <summary>
        /// 小时
        /// </summary>
        Hour = 5,
        /// <summary>
        /// 10分钟
        /// </summary>
        Min10 = 6
    }
}
