using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.App
{
    public static class FormExtensions
    {
        public static void Invoke(this Form form, Action action)
        {
            form.Invoke((Delegate)action);
        }

        public static TReturn Invoke<TReturn>(this Form form, Func<TReturn> func, params object[] args)
        {
            return (TReturn)form.Invoke((Delegate)func, args);
        }

        public static TResult OpenDialog<TForm, TResult>(this Form form) where TForm : Form, IDialogForm<TResult>, new()
        {
            using (var frm = new TForm())
            {
                var result = frm.ShowDialog(form);
                return ((IDialogForm<TResult>)frm).DialogResult;
            }
        }
    }
}
