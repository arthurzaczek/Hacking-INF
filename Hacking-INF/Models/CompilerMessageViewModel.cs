using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YamlDotNet.Serialization;

namespace Hacking_INF.Models
{
    public class CompilerMessageViewModel
    {
        public string Message { get; set; }
        public string Hint { get; set; }

        public CompilerMessageViewModel()
        {

        }

        public CompilerMessageViewModel(CompilerMessage obj)
        {
            Refresh(obj);
        }

        public void Refresh(CompilerMessage obj)
        {
            var target = this;
            var source = obj;

            target.Message = source.Message;
            target.Hint = source.Hint;
        }
    }
}