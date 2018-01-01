using System;

namespace Nice.Core.Log
{
    public interface ILogging
    {
        void Create(string sign, params string[] bizSigns);
        void Info(string info);
        void Error(Exception ex);
        void Error(Exception ex, string info);
        void Info(string content, string biz);
        void BizError(Exception ex, string biz);
        void BizError(Exception ex, string info, string biz);
    }
}
