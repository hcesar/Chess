using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App
{

    public interface IDialogForm<TDialogResult>
    {
        TDialogResult DialogResult { get; }
    }
}
