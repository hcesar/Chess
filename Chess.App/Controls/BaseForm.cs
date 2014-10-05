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

        public static  DialogResult OpenDialog<TForm>(this Form form) where TForm : Form, new()
        {
            using (var frm = new TForm())
            {
                return frm.ShowDialog(form);
            }
        }
    }
}
