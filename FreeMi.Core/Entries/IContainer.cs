using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMi.Core.Entries
{
    interface IContainer
    {
        Entry GetContent(bool isFreeboxV5);
    }
}
